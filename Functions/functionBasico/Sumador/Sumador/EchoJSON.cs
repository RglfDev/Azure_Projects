using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sumador;

public class EchoJSON
{
    private readonly ILogger<EchoJSON> _logger;

    public EchoJSON(ILogger<EchoJSON> logger)
    {
        _logger = logger;
    }

    [Function("EchoJSON")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("SOLICITUD POST");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        if (string.IsNullOrEmpty(requestBody))
        {
            return new BadRequestObjectResult("Por favor envía un JSON en el body de la petición.");
        }

        dynamic data;
        try
        {
            data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(requestBody);
        }
        catch
        {
            return new BadRequestObjectResult("JSON inválido.");
        }


        return new OkObjectResult(new
        {
            mensaje = "Echo de tu JSON recibido:",
            contenido = data
        });

    }
}