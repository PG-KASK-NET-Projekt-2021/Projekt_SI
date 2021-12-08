using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

namespace SI_API.Services
{
    public class QueueHandlerService : BackgroundService
    {
        private const string topic = "SENSOR_TOPIC";
        private const string ServerName = "queue";
        private const int ServerPort = 1883;
        private readonly ILogger<QueueHandlerService> _logger;
        private readonly SensorDataService _sensorDataService;
        private readonly IMqttClient mqttClient;
        private readonly MqttFactory mqttFactory;
        private int waitTime = 2000;

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
                var data =
                    JsonSerializer.Deserialize<SensorData>($"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

                sensorDataService.Create(DataConverter.Convert(data));

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