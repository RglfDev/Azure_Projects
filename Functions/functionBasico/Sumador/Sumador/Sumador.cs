using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Sumador;

public class Sumador
{
    private readonly ILogger<Sumador> _logger;

    public Sumador(ILogger<Sumador> logger)
    {
        _logger = logger;
    }

    [Function("Sumador")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string aStr = req.Query["a"]!;
        string bStr = req.Query["b"]!;

        _logger.LogInformation($"Solicitud de suma: a={aStr}, b={bStr}");

        if (!int.TryParse(aStr, out int a) || !int.TryParse(bStr, out int b)) {

            return new OkObjectResult("$Por favor pasa los numeros enteros en a y en b");

        }

        int result = a + b;

        return new OkObjectResult($"La suma de {a} + {b} es {result}");


        
    }
}