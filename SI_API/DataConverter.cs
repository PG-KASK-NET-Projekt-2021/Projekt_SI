using System;
using MongoDB.Bson;

namespace SI_API
{
    public class DataConverter
    {
        public DateTime Date { get; set; }

        public int SensorId { get; set; }

        public int SensorType { get; set; }

        public int Float { get; set; }

        public static SensorDataModel Convert(SensorData sensorData)
        {
            var sensorDataModel = new SensorDataModel();
            sensorDataModel.Date = sensorData.Date;
            sensorDataModel.SensorId = sensorData.SensorId;
            sensorDataModel.SensorType = sensorData.SensorType;
            sensorDataModel._id = ObjectId.Empty;
            sensorDataModel.Value = ConvertValue((Sensor) sensorData.SensorId, sensorData.Value);

            return sensorDataModel;
        }

        private static float ConvertValue(Sensor sensorId, int value)
        {
            // Tuple<int, int> interval = MapInterval(sensorId);
            // int start = interval.Item1;
            // int end = interval.Item2;
            return (float) value / 100;
        }


        private static Tuple<int, int> MapInterval(Sensor sensorId)
        {
            return sensorId switch
            {
                Sensor.Termometr => new Tuple<int, int>(-4000, 5000),
                Sensor.Barometr => new Tuple<int, int>(96000, 106000),
                Sensor.Higrometr => new Tuple<int, int>(0, 10000),
                Sensor.Fotometr => new Tuple<int, int>(2000, 11000000),
                _ => null
            };
        }

        private enum Sensor
        {
            Termometr, //SensorType = 0
            Barometr, //SensorType = 1
            Higrometr, //SensorType = 2
            Fotometr //SensorType = 3
        }
    }
}