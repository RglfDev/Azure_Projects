
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ConsumidoresFunctions;

public class EmailFunction
{
    private readonly ILogger<EmailFunction> _logger;

    public EmailFunction(ILogger<EmailFunction> logger)
    {
        _logger = logger;
    }

    [FunctionName("EmailFunction")]
    public void Run(
        [ServiceBusTrigger(
            topicName: "pedidos",
            subscriptionName: "email-sub",
            Connection = "ServiceBusConnection")]
        string message,
        ILogger log)
    {
        log.LogInformation($"Email recibido: {message}");
    }
}