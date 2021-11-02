using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;


namespace Generator
{
    enum Sensor
    {
        X,
        Y,
        Z,
        A
    }

    class Program
    {
        static void Main(string[] args)
        {
    
            int numberOfSensors = 30;
            int delayOnX =50;
            int delayOnY =  55;
            int delayOnZ =  60;
            int delayOnA = 45;
            int numberOfRepetitions = 5;
            Random random = new Random();
            Sensor sensor;
            Thread[] threads = new Thread[numberOfSensors];

            for (int i = 0; i < numberOfSensors; i++)
            {
                sensor = getRandomSensor(random);
                switch (sensor)
                {
                    case Sensor.X:
                        threads[i] = new Thread(new SensorX(i, 0, numberOfRepetitions, delayOnX).sendData);
                        break;
                    case Sensor.Y:
                        threads[i] = new Thread(new SensorY(i, 1, numberOfRepetitions, delayOnY).sendData);
                        break;
                    case Sensor.Z:
                        threads[i] = new Thread(new SensorZ(i, 2, numberOfRepetitions, delayOnZ).sendData);
                        break;
                    case Sensor.A:
                        threads[i] = new Thread(new SensorA(i, 3, numberOfRepetitions, delayOnA).sendData);
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
 
    }

    public class HttpHandler
    {
        public static void postSensorData(int sensorId,int sensorType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api/api/sensor");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            Random random = new Random();
            int value = random.Next(0, 1000);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"sensorId\":\"" + sensorId + "\"," +
                              "\"sensorType\":\"" + sensorType + "\"," +
                              "\"value\":\"" + value + "\"}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

        }
    }

    public class SensorX
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;

        public SensorX(int sensorId,int sensorType,int repeats,int delay)
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
                    Console.WriteLine("Sensor X - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Sensor X - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }

    public class SensorY
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;

        public SensorY(int sensorId, int sensorType, int repeats, int delay)
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
                    Console.WriteLine("Sensor Y - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Sensor Y - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }

    public class SensorZ
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;

        public SensorZ(int sensorId, int sensorType, int repeats, int delay)
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
                    Console.WriteLine("Sensor Z - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Sensor Z - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }

    public class SensorA
    {
        int sensorId;
        int sensorType;
        int repeats;
        int delay;

        public SensorA(int sensorId, int sensorType, int repeats, int delay)
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
                    Console.WriteLine("Sensor A - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Sensor A - ID : " + sensorId);
                    HttpHandler.postSensorData(sensorId, sensorType);
                    Thread.Sleep(1000 * delay);
                }
            }
        }
    }
}
