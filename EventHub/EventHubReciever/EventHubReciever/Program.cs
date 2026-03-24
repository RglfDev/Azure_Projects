using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Text;

string ehConnection = "Endpoint=sb://clickstreamnslobo.servicebus.windows.net/;SharedAccessKeyName=listenpolicy;SharedAccessKey=ki4U/nGakSMMVZopLt1/3MsNUgzzhs9jf+AEhFbyqPg=";
//<CONNECTION_STRING_LISTEN>: clave de listenpolicy.
string storageConnection = "DefaultEndpointsProtocol=https;AccountName=storageeventhublobo;AccountKey=DqUt7HpPLOOW0BI0XtsZPoT+/1lZoZu22gy7wa/NqPJ3Mxitoawevy3/fBcg1redYJ2Fhx8BXWaq+AStONXBuw==;EndpointSuffix=core.windows.net";
//<STORAGE_CONNECTION_STRING>: clave de conexión de tu cuenta de almacenamiento.
string eventHubName = "clicks";
string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

BlobContainerClient blobClient = new(storageConnection, "eventhub-checkpoints");

EventProcessorClient processor = new(blobClient, consumerGroup, ehConnection, eventHubName);

processor.ProcessEventAsync += async eventArgs =>
{
    string body = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
    Console.WriteLine($"Evento recibido: {body}");

    await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
};

processor.ProcessErrorAsync += async errorArgs =>
{
    Console.WriteLine($"Error: {errorArgs.Exception.Message}");
    await Task.CompletedTask;
};

Console.WriteLine("Esperando eventos...");
await processor.StartProcessingAsync();

Console.ReadLine();
await processor.StopProcessingAsync();