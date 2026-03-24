using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using ServicesBusApplication.Models;
using System.Diagnostics;
using System.Text;

namespace ServicesBusApplication.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _queueName;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly string _connectionString;

        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = configuration["AzureServiceBus:ConnectionString"]!;
            _queueName = configuration["AzureServiceBus:QueueName"]!;
            _topicName = configuration["AzureServiceBus:TopicName"]!;
            _subscriptionName = configuration["AzureServiceBus:SubscriptionName"]!;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendMessageToQueue(string message)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);
            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
            await sender.SendMessageAsync(busMessage);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToTopic(string message)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_topicName);
            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
            await sender.SendMessageAsync(busMessage);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ReceiveMessageFromQueue()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(_queueName);
            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();
            return Content(message != null ? Encoding.UTF8.GetString(message.Body) : "No messages");
        }

        public async Task<IActionResult> ReceiveMessageFromSubscription()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(_topicName, _subscriptionName);
            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();
            return Content(message != null ? Encoding.UTF8.GetString(message.Body) : "No messages");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
