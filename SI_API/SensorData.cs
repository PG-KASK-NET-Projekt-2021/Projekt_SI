using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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