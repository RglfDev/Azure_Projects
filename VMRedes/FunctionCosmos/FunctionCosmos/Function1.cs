using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CosmosFuncDemo;

public class Function1

{

    private readonly ILogger<Function1> _logger;

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

    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)

    {

        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string connectionString =


        return new OkObjectResult("Welcome to Azure Functions!");

    }

}
