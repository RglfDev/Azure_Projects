using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Product_Store_ServiceBus.Models;
using System.Text.Json;

namespace Product_Store_ServiceBus.Controllers
{
    public class PedidoController : Controller
    {
        private readonly string connectionString = "Endpoint=sb://mitajamarnamespacelobo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=JpSsecDAs1OFEEJPsapZy1gwonnztZG1J+ASbI/0fz4=";
        private readonly string topicName = "pedidos";
        public async Task<IActionResult> Crear(
            int pedidoId = 1,
            string clienteEmail = "juan@example.com",
            string producto = "Laptop",
            decimal precio = 1200)
            {
                var pedido = new Pedido
                {
                    PedidoId = pedidoId,
                    ClienteEmail = clienteEmail,
                    Producto = producto,
                    Precio = precio
                };

            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(topicName);

            string json = JsonSerializer.Serialize(pedido);
            await sender.SendMessageAsync(new ServiceBusMessage(json));

            return Content($"Pedido enviado: {json}");
        }
        public IActionResult Index()
        {
            return View();
        }




    }
}
