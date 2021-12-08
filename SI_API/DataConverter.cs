using System;
using MongoDB.Bson;

namespace SI_API
{
    public class DataConverter
    {

        public static SensorDataModel Convert(SensorData sensorData)
        {
            SensorDataModel sensorDataModel = new SensorDataModel();
            sensorDataModel.Date = sensorData.Date;
            sensorDataModel.SensorId = sensorData.SensorId;
            sensorDataModel.SensorType = sensorData.SensorType;
            sensorDataModel.Value = sensorData.Value;
            sensorDataModel._id = ObjectId.Empty;

            return sensorDataModel;
        }
        
        private static Tuple<int, int> MapInterval(Sensor sensorId)
        {
            return sensorId switch
            {
                Sensor.Termometr => new Tuple<int, int>(-40, 50),
                Sensor.Barometr => new Tuple<int, int>(960, 1060),
                Sensor.Higrometr => new Tuple<int, int>(0, 100),
                Sensor.Fotometr => new Tuple<int, int>(20, 110000),
                _ => null
            };
        }
        
        enum Sensor
        {
            Termometr, //SensorType = 0
            Barometr, //SensorType = 1
            Higrometr, //SensorType = 2
            Fotometr //SensorType = 3
        }

        public DateTime Date { get; set; }

        public int SensorId { get; set; }

        public int SensorType { get; set; }

        public int Float { get; set; }
    }
}