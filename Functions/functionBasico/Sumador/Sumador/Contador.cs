using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Sumador;

public class Contador
{
    private readonly ILogger<Contador> _logger;
    static int contador = 0;

    public Contador(ILogger<Contador> logger)
    {
        _logger = logger;
    }

    [Function("Contador")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        contador++;
        return new OkObjectResult($"Has entrado {contador} veces");
    }
}