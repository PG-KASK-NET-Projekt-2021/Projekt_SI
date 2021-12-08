using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SI_API
{
    public class SensorData
    {
        [BsonId]
        public ObjectId _id { get; set; }
        
        [BsonElement("Date")] public DateTime Date { get; set; }

        [BsonElement("SensorId")] public int SensorId { get; set; }

        [BsonElement("SensorType")] public int SensorType { get; set; }

        [BsonElement("Value")] public int Value { get; set; }
    }
}