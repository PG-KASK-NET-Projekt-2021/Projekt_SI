using System;

namespace SI_API
{
    public class SensorData
    {
        public DateTime Date { get; set; }
        
        public int SensorId { get; set; }

        public int SensorType { get; set; }

        public int Value { get; set; }
    }
}