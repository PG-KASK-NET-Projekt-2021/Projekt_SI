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
        static void Main(string[] args)
        {
    
            int numberOfSensors;
            int delayOnX;
            int delayOnY;
            int delayOnZ;
            int delayOnA;
            int numberOfRepetitions;

            if (Environment.GetEnvironmentVariable("SI_NUMBER_OF_SENSORS") != null)
            {
                numberOfSensors = int.Parse(Environment.GetEnvironmentVariable("SI_NUMBER_OF_SENSORS"));
            }
            else
            {
                numberOfSensors = 30;
            }
            if (Environment.GetEnvironmentVariable("SI_DELAY_ON_X") != null)
            {
                delayOnX = int.Parse(Environment.GetEnvironmentVariable("SI_DELAY_ON_X"));
            }
            else
            {
                delayOnX = 2;
            }
            if (Environment.GetEnvironmentVariable("SI_DELAY_ON_Y") != null)
            {
                delayOnY = int.Parse(Environment.GetEnvironmentVariable("SI_DELAY_ON_Y"));
            }
            else
            {
                delayOnY = 3;
            }
            if (Environment.GetEnvironmentVariable("SI_DELAY_ON_Z") != null)
            {
                delayOnZ = int.Parse(Environment.GetEnvironmentVariable("SI_DELAY_ON_Z"));
            }
            else
            {
                delayOnZ = 2;
            }
            if (Environment.GetEnvironmentVariable("SI_DELAY_ON_A") != null)
            {
                delayOnA = int.Parse(Environment.GetEnvironmentVariable("SI_DELAY_ON_A"));
            }
            else
            {
                delayOnA = 2;
            }
            if (Environment.GetEnvironmentVariable("SI_NUMBER_OF_REPETITIONS") != null)
            {
                numberOfRepetitions = int.Parse(Environment.GetEnvironmentVariable("SI_NUMBER_OF_REPETITIONS"));
            }
            else
            {
                numberOfRepetitions = -1;
            }

            Random random = new Random();
            Sensor sensor;
            Thread[] threads = new Thread[numberOfSensors];


            for (int i = 0; i < numberOfSensors; i++)
            {
                /*
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
                */
                if(i < numberOfSensors / 4)
                {
                    threads[i] = new Thread(new Termometr(i, 0, numberOfRepetitions, delayOnX).sendData);
                }
                else if(i< numberOfSensors / 2)
                {
                    threads[i] = new Thread(new Barometr(i, 1, numberOfRepetitions, delayOnY).sendData);
                }
                else if (i < 3* numberOfSensors / 4)
                {
                    threads[i] = new Thread(new Higrometr(i, 2, numberOfRepetitions, delayOnZ).sendData);
                }
                else
                {
                    threads[i] = new Thread(new Fotometr(i, 3, numberOfRepetitions, delayOnA).sendData);
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

    }


    public class HttpHandler
    {
        private const string Topic = "SENSOR_TOPIC";
        private const string ServerName = "queue";
        private const int ServerPort = 1883;
        private readonly MqttFactory _mqttFactory = new MqttFactory();
        private readonly MqttFactory _mqttClient;
        private int TermMin;
        private int TermMax;
        private int BarMin;
        private int BarMax;
        private int HigrMin;
        private int HigrMax;
        private int FotMin;
        private int FotMax;
        public HttpHandler() {
            if (Environment.GetEnvironmentVariable("SI_TERMOMETER_MIN") != null)
            {
                TermMin = int.Parse(Environment.GetEnvironmentVariable("SI_TERMOMETER_MIN"));
            }
            else
            {
                TermMin = -4000;
            }
            if (Environment.GetEnvironmentVariable("SI_TERMOMETER_MAX") != null)
            {
                TermMax = int.Parse(Environment.GetEnvironmentVariable("SI_TERMOMETER_MAX"));
            }
            else
            {
                TermMax = 5000;
            }
            if (Environment.GetEnvironmentVariable("SI_BAROMETER_MIN") != null)
            {
                BarMin = int.Parse(Environment.GetEnvironmentVariable("SI_BAROMETER_MIN"));
            }
            else
            {
                BarMin = 96000;
            }
            if (Environment.GetEnvironmentVariable("SI_BAROMETER_MAX") != null)
            {
                BarMax = int.Parse(Environment.GetEnvironmentVariable("SI_BAROMETER_MAX"));
            }
            else
            {
                BarMax = 106000;
            }
            if (Environment.GetEnvironmentVariable("SI_HYGROMETER_MIN") != null)
            {
                HigrMin = int.Parse(Environment.GetEnvironmentVariable("SI_HYGROMETER_MIN"));
            }
            else
            {
                HigrMin = 0;
            }
            if (Environment.GetEnvironmentVariable("SI_HYGROMETER_MAX") != null)
            {
                HigrMax = int.Parse(Environment.GetEnvironmentVariable("SI_HYGROMETER_MAX"));
            }
            else
            {
                HigrMax = 10000;
            }
            if (Environment.GetEnvironmentVariable("SI_PHOTOMETER_MIN") != null)
            {
                FotMin = int.Parse(Environment.GetEnvironmentVariable("SI_PHOTOMETER_MIN"));
            }
            else
            {
                FotMin = 2000;
            }
            if (Environment.GetEnvironmentVariable("SI_PHOTOMETER_MAX") != null)
            {
                FotMax = int.Parse(Environment.GetEnvironmentVariable("SI_PHOTOMETER_MAX"));
            }
            else
            {
                FotMax = 11000000;
            }
        }

        public static async Task postSensorDataAsync(int sensorId,int sensorType)
        {
            //Console.WriteLine(sensorId);
            Random random = new Random();
            //int value = random.Next(0, 1000);
            HttpHandler handler = new HttpHandler();
            int value;
            switch (sensorType)
            {
                case 0:
                    value = random.Next(handler.TermMin, handler.TermMax);
                    break;
                case 1:
                    value = random.Next(handler.BarMin, handler.BarMax);
                    break;
                case 2:
                    value = random.Next(handler.HigrMin, handler.HigrMax);
                    break;
                case 3:
                    value = random.Next(handler.FotMin, handler.FotMax);
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

            Console.WriteLine(jsonData);

            var message = new MqttApplicationMessageBuilder()
                     .WithTopic(Topic)
                     .WithPayload(jsonData)
                     .WithExactlyOnceQoS()
                     .WithRetainFlag()
                     .Build();

            //HttpHandler handler2 = new HttpHandler();
            var _mqttClient = await handler.CreateConnection();
            await _mqttClient.PublishAsync(message, CancellationToken.None);

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
