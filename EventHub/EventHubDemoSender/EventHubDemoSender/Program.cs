using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;

string connectionString = "Endpoint=sb://clickstreamnslobo.servicebus.windows.net/;SharedAccessKeyName=sendpolitic;SharedAccessKey=ubFsO1MCp2jNU9r4cy9DCVz2rTaP/qTC9+AEhMobC6w=";
//Sustituye <CONNECTION_STRING_SEND> por la cadena de conexión de sendpolicy.
string eventHubName = "clicks";

EventHubProducerClient producer = new(connectionString, eventHubName);

Console.WriteLine("Enviando eventos de clics...");

for (int i = 1; i <= 10; i++)
{
    string message = $"{{ \"clickId\": {i}, \"timestamp\": \"{DateTime.UtcNow:o}\" }}";
    EventData eventData = new(Encoding.UTF8.GetBytes(message));

    await producer.SendAsync(new[] { eventData });
    Console.WriteLine($"Enviado: {message}");

    await Task.Delay(500); // Simula un clic cada medio segundo
}

await producer.DisposeAsync();
Console.WriteLine("Finalizado.");