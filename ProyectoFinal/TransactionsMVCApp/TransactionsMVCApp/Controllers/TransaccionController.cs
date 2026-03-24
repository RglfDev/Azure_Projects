using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using TransactionsMVCApp.Data;
using TransactionsMVCApp.Models;

namespace TransactionsMVCApp.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _queueName;
        private readonly string _topicName;
        private readonly string _suscriptionName;
        private readonly string _connectionString;
        private readonly ILogger<TransaccionController> _logger;

        public TransaccionController(ApplicationDbContext context,
            ILogger<TransaccionController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;

            _connectionString = configuration["AzureServiceBus:ConnectionString"]!;
            _queueName = configuration["AzureServiceBus:QueueName"]!;
            _topicName = configuration["AzureServiceBus:TopicName"]!;
            _suscriptionName = configuration["AzureServiceBus:SubscriptionName"]!;
        }

        public async Task<IActionResult> Index(string? estado)
        {
            ViewBag.TotalPendientes = await _context.Transacciones.CountAsync(t => t.Estado == "Pendiente");
            ViewBag.TotalExitosas = await _context.Transacciones.CountAsync(t => t.Estado == "Exitosa");
            ViewBag.TotalFallidas = await _context.Transacciones.CountAsync(t => t.Estado == "Fallida");
            ViewBag.EstadoActual = estado;

            var query = _context.Transacciones
                .Include(t => t.Eventos)
                .Include(t => t.Notificacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(t => t.Estado == estado);
            }

            var transacciones = await query
                .OrderByDescending(t => t.TransaccionId)
                .ToListAsync();

            return View(transacciones);
        }

        public async Task<IActionResult> ReceiveMessageFromQueue()
        {
            try
            {
                await using var client = new ServiceBusClient(_connectionString);
                ServiceBusReceiver receiver = client.CreateReceiver(_queueName);

                ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(
                    maxWaitTime: TimeSpan.FromSeconds(5)
                );

                if (message == null)
                {
                    TempData["Mensaje"] = "No hay mensajes en la cola.";
                    return RedirectToAction("Index");
                }

                string body = Encoding.UTF8.GetString(message.Body);
                var data = JsonSerializer.Deserialize<ColaTransaccionesPendientes>(body);

                if (data == null)
                {
                    await receiver.DeadLetterMessageAsync(message, "Error de deserialización");
                    TempData["Mensaje"] = "Error al deserializar el mensaje.";
                    return RedirectToAction("Index");
                }

                var transaccion = await _context.Transacciones.FindAsync(data.TransaccionId);

                if (transaccion == null)
                {
                    await receiver.DeadLetterMessageAsync(message, "Transacción no encontrada en BD");
                    TempData["Mensaje"] = "Transacción no encontrada.";
                    return RedirectToAction("Index");
                }

                bool esExitosa = transaccion.Monto <= 10000;

                if (!esExitosa)
                {
                    transaccion.Estado = "Fallida";
                    transaccion.FechaProcesamiento = DateTime.Now;
                    await _context.SaveChangesAsync();
                    await receiver.DeadLetterMessageAsync(message, "Monto excede límite permitido");
                }
                else
                {
                    transaccion.Estado = "Exitosa";
                    transaccion.FechaProcesamiento = DateTime.Now;
                    await _context.SaveChangesAsync();
                    await receiver.CompleteMessageAsync(message);
                }

                _context.EventosTransaccion.Add(new EventoTransaccion
                {
                    TransaccionId = transaccion.TransaccionId,
                    TipoEvento = esExitosa ? "Transacción aprobada" : "Transacción rechazada",
                    Descripcion = $"Transacción procesada como {transaccion.Estado}",
                    FechaEvento = DateTime.Now
                });
                await _context.SaveChangesAsync();

                if (esExitosa)
                {
                    _context.Notificaciones.Add(new Notificacion
                    {
                        TransaccionId = transaccion.TransaccionId,
                        EmailCliente = "cliente@email.com",
                        EstadoNotificacion = "Enviada",
                        FechaEnvio = DateTime.Now
                    });

                    transaccion.FechaNotificacion = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                await SendMessageToTopic(
                    $"Transacción {transaccion.TransaccionId} procesada como {transaccion.Estado}"
                );

                TempData["Mensaje"] = $"Transacción #{transaccion.TransaccionId} procesada como {transaccion.Estado}.";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        private async Task SendMessageToTopic(string message)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_topicName);

            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
            await sender.SendMessageAsync(busMessage);
        }

        public async Task<IActionResult> ReceiveFromSubscription()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(_topicName, _suscriptionName);

            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

            if (message == null)
            {
                TempData["Mensaje"] = "No hay eventos en la suscripción.";
                return RedirectToAction("Index");
            }

            string evento = Encoding.UTF8.GetString(message.Body);

            await receiver.CompleteMessageAsync(message);

            TempData["Mensaje"] = "Evento recibido: " + evento;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MonitorDLQ()
        {
            await using var client = new ServiceBusClient(_connectionString);

            ServiceBusReceiver dlqReceiver = client.CreateReceiver(_queueName, new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });

            var mensajesAzure = await dlqReceiver.PeekMessagesAsync(maxMessages: 10);

            var listaErrores = mensajesAzure.Select(msg => {
                var cuerpoString = Encoding.UTF8.GetString(msg.Body);
                return new
                {
                    Cuerpo = JsonSerializer.Deserialize<ColaTransaccionesPendientes>(cuerpoString),
                    Motivo = msg.DeadLetterReason,
                    IdMensaje = msg.MessageId,
                    Fecha = msg.EnqueuedTime.DateTime
                };
            }).ToList();

            return View(listaErrores);
        }

        public async Task<IActionResult> EliminarDeDLQ()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusReceiver dlqReceiver = client.CreateReceiver(_queueName, new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });

            ServiceBusReceivedMessage message = await dlqReceiver.ReceiveMessageAsync();

            if (message != null)
            {
                await dlqReceiver.CompleteMessageAsync(message);
                TempData["Mensaje"] = "Error procesado y eliminado de la DLQ.";
            }

            return RedirectToAction("MonitorDLQ");
        }
    }

}

