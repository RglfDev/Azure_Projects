using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace ProcessImages;

public class ProcessImageFunction
{
    private readonly ILogger<ProcessImageFunction> _logger;

    // Inyección de dependencias a través del constructor
    public ProcessImageFunction(ILogger<ProcessImageFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessImageUpload")]
    [TableOutput("ImageText", Connection = "StorageConnection")]
    public async Task<ImageContent> Run(
        [BlobTrigger("imageanalysis/{name}", Connection = "StorageConnection")] Stream myBlob,
        string name)
    {
        _logger.LogInformation($"Procesando imagen subida: {name}");

        // Obtener configuraciones del entorno
        string subscriptionKey = Environment.GetEnvironmentVariable("ComputerVisionKey");
        string endpoint = Environment.GetEnvironmentVariable("ComputerVisionEndpoint");
        string storageAccountName = Environment.GetEnvironmentVariable("StorageAccountName");
        string imgUrl = $"https://{storageAccountName}.blob.core.windows.net/imageanalysis/{name}";

        ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
        {
            Endpoint = endpoint
        };

        // Obtener el contenido analizado de la imagen
        var textContext = await AnalyzeImageContent(client, imgUrl);

        _logger.LogInformation($"Análisis completado para: {name}");

        // El objeto retornado se guarda automáticamente en Table Storage
        return new ImageContent
        {
            PartitionKey = "Images",
            RowKey = Guid.NewGuid().ToString(),
            Text = textContext
        };
    }

    public class ImageContent
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Text { get; set; }
    }

    static async Task<string> AnalyzeImageContent(ComputerVisionClient client, string urlFile)
    {
        // Analizar el archivo usando Computer Vision Client
        var textHeaders = await client.ReadAsync(urlFile);
        string operationLocation = textHeaders.OperationLocation;

        // CORRECCIÓN: Usar Task.Delay en lugar de Thread.Sleep en métodos asíncronos
        await Task.Delay(2000);

        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        // Leer los resultados de la petición de análisis
        ReadOperationResult results;
        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));

            // Pausa breve entre consultas para no saturar la API
            if (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted)
            {
                await Task.Delay(1000);
            }
        }
        while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

        var textUrlFileResults = results.AnalyzeResult.ReadResults;

        // Ensamblar en un string legible
        StringBuilder text = new StringBuilder();
        foreach (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.ReadResult page in textUrlFileResults)
        {
            foreach (Line line in page.Lines)
            {
                text.AppendLine(line.Text);
            }
        }

        return text.ToString();
    }
}