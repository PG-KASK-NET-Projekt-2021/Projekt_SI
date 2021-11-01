using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;

namespace si.Services
{
    public class QueueService : IQueueService
    {
        private const string ServerName = "queue";
        private const int ServerPort = 1883;
        private readonly ILogger<QueueService> _logger;

        private readonly MqttFactory _mqttFactory;

        public QueueService(ILogger<QueueService> logger)
        {
            _logger = logger;
            _mqttFactory = new MqttFactory();
        }

        public async void Publish(object data, string topic)
        {
            var jsonData = JsonConvert.SerializeObject(data);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(jsonData)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            var mqttClient = await CreateConnection();
            await mqttClient.PublishAsync(message, CancellationToken.None);

            _logger.Log(LogLevel.Information, "Publish to queue: {JsonData}", jsonData);
        }

        private async Task<IMqttClient> CreateConnection()
        {
            var mqttClient = _mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(ServerName, ServerPort)
                .Build();

            await mqttClient.ConnectAsync(options, CancellationToken.None);
            return mqttClient;
        }
    }
}