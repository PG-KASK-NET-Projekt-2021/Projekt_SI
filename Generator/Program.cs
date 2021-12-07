using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Newtonsoft.Json;

namespace Generator
{
    enum Sensor
    {
        Termometr, //SensorType = 0
        Barometr, //SensorType = 1
        Higrometr, //SensorType = 2
        Fotometr //SensorType = 3
    }

    public class SensorData
    {
        public int SensorId { get; set; }

        public int SensorType { get; set; }

        public int Value { get; set; }

        public DateTime Date { get; set; }
    }

    class Program
    {
        private const string ServerName = "queue";
        private const int ServerPort = 1883;

        public static readonly IMqttClient mqttClient;
        static void Main(string[] args)
        {
    
            int numberOfSensors = 30;
            int delayOnX =50;
            int delayOnY =  55;
            int delayOnZ =  60;
            int delayOnA = 45;
            int numberOfRepetitions = -1;
            Random random = new Random();
            Sensor sensor;
            Thread[] threads = new Thread[numberOfSensors];

            /*
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId("Dev.To")
                                        .WithTcpServer("queue", 1883);
            MqttFactory mqttFactory = new MqttFactory();
            IManagedMqttClient mqttClient = mqttFactory.CreateManagedMqttClient();
            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                        .WithClientOptions(builder.Build())
                        .Build();
            mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
            mqttClient.StartAsync(options).GetAwaiter().GetResult();
            */

            for (int i = 0; i < numberOfSensors; i++)
            {
                sensor = getRandomSensor(random);
                switch (sensor)
                {
                    case Sensor.Termometr:
                        threads[i] = new Thread(new Termometr(i, 0, numberOfRepetitions, delayOnX).sendData);
                        break;
                    case Sensor.Barometr:
                        threads[i] = new Thread(new Barometr(i, 1, numberOfRepetitions, delayOnY).sendData);
                        break;
                    case Sensor.Higrometr:
                        threads[i] = new Thread(new Higrometr(i, 2, numberOfRepetitions, delayOnZ).sendData);
                        break;
                    case Sensor.Fotometr:
                        threads[i] = new Thread(new Fotometr(i, 3, numberOfRepetitions, delayOnA).sendData);
                        break;
                }
                threads[i].Start();
            }
        }

        static Sensor getRandomSensor(Random random)
        {
            Array values = Enum.GetValues(typeof(Sensor));
            Sensor sensor = (Sensor)values.GetValue(random.Next(values.Length));
            return sensor;
        }

        public static void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Log.Logger.Information("Successfully connected.");
        }

        public static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Log.Logger.Warning("Couldn't connect to broker.");
        }

        public static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Log.Logger.Information("Successfully disconnected.");
        }

    }


    public class HttpHandler
    {
        private const string Topic = "SENSOR_TOPIC";
        private const string ServerName = "queue";
        private const int ServerPort = 1883;
        private readonly MqttFactory _mqttFactory = new MqttFactory();

        public static async Task postSensorDataAsync(int sensorId,int sensorType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api/api/sensor");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            Random random = new Random();
            //int value = random.Next(0, 1000);
            int value;
            switch (sensorType)
            {
                case 0:
                    value = random.Next(-40, 50);
                    break;
                case 1:
                    value = random.Next(960, 1060);
                    break;
                case 2:
                    value = random.Next(0, 100);
                    break;
                case 3:
                    value = random.Next(20, 110000);
                    break;
                default:
                    value = -100;
                    break;
            }

            var data = new SensorData
            {
                SensorId = sensorId,
                SensorType = sensorType,
                Value = value,
                Date = DateTime.Now
            };

            var jsonData = JsonConvert.SerializeObject(data);

            var message = new MqttApplicationMessageBuilder()
                     .WithTopic(Topic)
                     .WithPayload(jsonData)
                     .WithExactlyOnceQoS()
                     .WithRetainFlag()
                     .Build();

            HttpHandler handler = new HttpHandler();
            var _mqttClient = await handler.CreateConnection();
            await _mqttClient.PublishAsync(message, CancellationToken.None);

            /*
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"sensorId\":\"" + sensorId + "\"," +
                              "\"sensorType\":\"" + sensorType + "\"," +
                              "\"value\":\"" + value + "\"}";
                mqttClient.PublishAsync(json);
                //streamWriter.Write(json);
            }*/

            /*
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
            */
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

    public class Termometr
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;

        public Termometr(int sensorId,int sensorType,int repeats,int delay)
        {
            this.sensorId = sensorId;
            this.sensorType = sensorType;
            this.repeats = repeats;
            this.delay = delay;
        }

        public void sendData()
        {
            if (repeats != -1)
            {
                for (int i = 0; i < repeats; i++)
                {
                    Console.WriteLine("Termometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Termometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }

    public class Barometr
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;


        public Barometr(int sensorId, int sensorType, int repeats, int delay)
        {
            this.sensorId = sensorId;
            this.sensorType = sensorType;
            this.repeats = repeats;
            this.delay = delay;
        }

        public void sendData()
        {
            if (repeats != -1)
            {
                for (int i = 0; i < repeats; i++)
                {
                    Console.WriteLine("Barometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Barometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }

    public class Higrometr
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;


        public Higrometr(int sensorId, int sensorType, int repeats, int delay)
        {
            this.sensorId = sensorId;
            this.sensorType = sensorType;
            this.repeats = repeats;
            this.delay = delay;
        }

        public void sendData()
        {
            if (repeats != -1)
            {
                for (int i = 0; i < repeats; i++)
                {
                    Console.WriteLine("Higrometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Higrometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }

    public class Fotometr
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;


        public Fotometr(int sensorId, int sensorType, int repeats, int delay)
        {
            this.sensorId = sensorId;
            this.sensorType = sensorType;
            this.repeats = repeats;
            this.delay = delay;
        }

        public void sendData()
        {
            if (repeats != -1)
            {
                for (int i = 0; i < repeats; i++)
                {
                    Console.WriteLine("Fotometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Fotometr - ID : " + sensorId);
                    HttpHandler.postSensorDataAsync(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }
}
