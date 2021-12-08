using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SI_API
{
    public class SensorDataModel
    {
        [BsonId][JsonIgnore] public ObjectId _id { get; set; }
        [BsonElement("Date")] public DateTime Date { get; set; }
        [BsonElement("SensorId")] public int SensorId { get; set; }
        [BsonElement("SensorType")] public int SensorType { get; set; }
        [BsonElement("Value")] public float Value { get; set; }
        public override string ToString()
        {
            return $"{Date}, {SensorId}, {SensorType}, {Value}";
        }
    }
}