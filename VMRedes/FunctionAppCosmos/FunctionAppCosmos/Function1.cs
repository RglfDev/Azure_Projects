using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CosmosFuncDemo;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    //Creamos el cliente de tipo CosmosClient(se puede recuperar a traves de la dependecia instalada de Cosmos)
    private static CosmosClient? cosmosClient;

    //Tambien creamos una variable de tipo Container (Cosmos) para guardar en ella el container
    private static Container? container;


    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    //Creamos una clase para tratar los datos del cliente que vamos a traer de la BBDD de Cosmos,
    //el cual tiene id, UserId y email
    public class UserItem
    {

        public string? id { get; set; }
        public string? userId { get; set; }
        public string? email { get; set; }
    }


    //La función (se llama Function1
    [Function("Function1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        //Una información de Log simple 
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        //Recuperamos la cadena de conexión a traves del entorno (local.settings.json)
        string connectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");

        //En esta variable, definimos el TIPO de conexión con la que vamos a conectar a Cosmos (Gateway)
        var options = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway
        };

        //Creamos un nuevo objeto CosmosClient con la conexion y el modo, y lo guardamos en la variable creada antes
        cosmosClient = new CosmosClient(connectionString, options);
        //Recuperamos, a traves del cliente de Cosmos, el contenedor apuntando a su nombre y a su documento o particion
        container = cosmosClient.GetContainer("MyDatabase", "Items");

        //De manera simple (normalmente no se hace asi) creamos las variables del id y de la PartitionKey,
        //las cuales son identicas a los datos guardados del usuario en Cosmos (njormalmente lo recuperariamos de alli)
        string id = "item001";
        string pk = "user123";

        //Si alguno de los dos anteriores esta vacio, exception
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pk))
        {
            return new BadRequestObjectResult("Missing 'id' or 'pk'");
        }

        //Si no, leemos el item del container, diciendole cual es a traves del id y de la PartitionKEy
        try
        {
            var response = await container.ReadItemAsync<UserItem>(id, new PartitionKey(pk));
            //Y retornamos el objeto como respuesta
            return new OkObjectResult(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new NotFoundObjectResult("Item not found.");
        }

    }
}