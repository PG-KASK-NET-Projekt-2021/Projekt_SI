using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MQTTnet.Client.Subscribing;

namespace SI_API.Services
{
    public class QueueHandlerService : BackgroundService
    {
        private readonly SensorDataService _sensorDataService;
        private readonly MqttFactory mqttFactory;
        private readonly IMqttClient mqttClient;
        private const string topic = "SENSOR_TOPIC";
        private readonly ILogger<QueueHandlerService> _logger;
        private int waitTime = 2000;
        private const string ServerName = "queue";
        private const int ServerPort = 1883;

        public QueueHandlerService(ILogger<QueueHandlerService> logger, SensorDataService sensorDataService)
        {
            _logger = logger;
            _sensorDataService = sensorDataService;

            mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            
            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build());
                Console.WriteLine("### SUBSCRIBED ###");
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                SensorData data =
                    JsonSerializer.Deserialize<SensorData>($"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                data._id = ObjectId.Empty;
                sensorDataService.Create(data);
                Console.WriteLine("### RECEIVED DATA ###");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine();
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("QueueHandler Started.");
            
            _logger.LogDebug("Connecting");
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(ServerName, ServerPort)
                .Build();
            await mqttClient.ConnectAsync(options, CancellationToken.None);
            _logger.LogDebug("Connected");
        }
    }
}