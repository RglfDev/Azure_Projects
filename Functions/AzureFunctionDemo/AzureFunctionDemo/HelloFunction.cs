using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace AzureFunctionDemo;

public class HelloFunction
{
    private readonly ILogger<HelloFunction> _logger;

    public HelloFunction(ILogger<HelloFunction> logger)
    {
        _logger = logger;
    }

    // ─────────────────────────────────────────────
    // GET  /api/hello             → saludo genérico
    // GET  /api/hello?name=Maria  → saludo personalizado
    // POST /api/hello  body: { "name": "Maria" }
    // ─────────────────────────────────────────────
    [Function("HelloFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "hello")] HttpRequestData req)
    {
        _logger.LogInformation("HelloFunction ejecutada: {time}", DateTime.UtcNow);

        // 1. Intentar obtener el nombre desde query string
        string? name = req.Query["name"];

        // 2. Si no viene en query, intentar desde el body JSON
        if (string.IsNullOrWhiteSpace(name))
        {
            try
            {
                var body = await JsonDocument.ParseAsync(req.Body);
                if (body.RootElement.TryGetProperty("name", out var nameProp))
                    name = nameProp.GetString();
            }
            catch { /* body vacío o no es JSON */ }
        }

        // 3. Construir respuesta
        var payload = new
        {
            message = string.IsNullOrWhiteSpace(name)
                ? "¡Hola, mundo! Pasa ?name=TuNombre para un saludo personalizado."
                : $"¡Hola, {name}! Bienvenido/a a Azure Functions 🎉",
            timestamp = DateTime.UtcNow,
            status = "ok"
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(payload));

        return response;
    }
}
