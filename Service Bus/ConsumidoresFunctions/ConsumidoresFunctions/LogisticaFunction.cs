
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ConsumidoresFunctions;

public class LogisticaFunction
{
    private readonly ILogger<LogisticaFunction> _logger;

    public LogisticaFunction(ILogger<LogisticaFunction> logger)
    {
        _logger = logger;
    }

    [FunctionName("LogisticaFunction")]
    public void Run(
        [ServiceBusTrigger("pedidos","logistica-sub", Connection = "ServiceBusConnection")]
    string message,
        ILogger log)
    {
        log.LogInformation($"Envío preparado: {message}");
    }
}