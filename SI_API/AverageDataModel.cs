using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SI_API
{
    public class AverageDataModel
    {
        public int SensorId { get; set; }

        public int SensorType { get; set; }

        public float Average { get; set; }
        
        public float Last { get; set; }
        
        public override string ToString()
        {
            return $", {SensorId}, {SensorType}, {Average},{Last}";
        }
    }
}