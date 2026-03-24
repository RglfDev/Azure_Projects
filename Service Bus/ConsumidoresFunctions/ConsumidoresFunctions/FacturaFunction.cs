
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ConsumidoresFunctions;

public class FacturaFunction
{
    private readonly ILogger<FacturaFunction> _logger;

    public FacturaFunction(ILogger<FacturaFunction> logger)
    {
        _logger = logger;
    }

    [FunctionName("FacturaFunction")]
    public void Run(
        [ServiceBusTrigger("pedidos", "factura-sub", Connection = "ServiceBusConnection")]
        string message,
        ILogger log)
    {
        log.LogInformation($"[Factura] Generando factura para pedido: {message}");
        // Aquí podrías guardar la factura en base de datos o generar PDF
    }
}