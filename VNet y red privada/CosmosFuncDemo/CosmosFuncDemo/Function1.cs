using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;

namespace CosmosFuncDemo;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private static CosmosClient? cosmosClient;
    private static Container? container;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    public class UserItem
    {
        public string? id { get; set; }
        public string? userId { get; set; }
        public string? email { get; set; }
    }

    [Function("Function1")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
                                            FunctionContext context)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Endpoint de tu Cosmos DB
        string cosmosEndpoint = Environment.GetEnvironmentVariable("COSMOS_DB_URI")!;
        if (string.IsNullOrEmpty(cosmosEndpoint))
        {
            throw new InvalidOperationException("Falta la variable de entorno 'COSMOS_DB_URI'");
        }

        // Inicializar cliente Cosmos con Managed Identity
        cosmosClient ??= new CosmosClient(
            accountEndpoint: cosmosEndpoint,
            tokenCredential: new DefaultAzureCredential(), // usa Managed Identity
            new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway }
        );

        container ??= cosmosClient.GetContainer("MyDatabase", "Items");

        string id = "item001";
        string pk = "user123";

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pk))
        {
            var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Missing 'id' or 'pk'");
            return badResponse;
        }

        try
        {
            var response = await container.ReadItemAsync<UserItem>(id, new PartitionKey(pk));

            var okResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await okResponse.WriteAsJsonAsync(response.Resource);
            return okResponse;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync("Item not found.");
            return notFoundResponse;
        }
        catch (CosmosException ex)
        {
            _logger.LogError($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.ServiceUnavailable);
            await errorResponse.WriteStringAsync($"Cosmos DB error: {ex.StatusCode}");
            return errorResponse;
        }
    }
}
