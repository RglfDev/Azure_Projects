using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using TransactionsMVCApp.Data;
using TransactionsMVCApp.Models;

namespace TransactionsMVCApp.Controllers
{
    public class FormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly string _queueName;

        public FormController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration["AzureServiceBus:ConnectionString"]!;
            _queueName = configuration["AzureServiceBus:QueueName"]!;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaccion transaccion)
        {
            if (!ModelState.IsValid) return View(transaccion);

            transaccion.Estado = "Pendiente";
            transaccion.FechaCreacion = DateTime.Now;

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            await EnviarAColaAsync(transaccion);

            return RedirectToAction("Index", "Transaccion");
        }

        private async Task EnviarAColaAsync(Transaccion transaccion)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);

            var payload = new ColaTransaccionesPendientes
            {
                TransaccionId = transaccion.TransaccionId,
                Monto = transaccion.Monto,
                TipoTransaccion = transaccion.TipoTransaccion!,
                Estado = transaccion.Estado!
            };

            string json = JsonSerializer.Serialize(payload);

            ServiceBusMessage mensaje = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                ContentType = "application/json",
                MessageId = transaccion.TransaccionId.ToString()
            };

            await sender.SendMessageAsync(mensaje);
        }
    }

}
