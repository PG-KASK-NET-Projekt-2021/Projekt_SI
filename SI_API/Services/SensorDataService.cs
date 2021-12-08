using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SI_API.Services
{
    public class SensorDataService
    {
        private readonly IMongoCollection<SensorData> _sensorData;

        public SensorDataService()
        {
            var mongoClientSettings = new MongoClientSettings();
            mongoClientSettings.Server = new MongoServerAddress("mongo", 27017);
            mongoClientSettings.Credential = MongoCredential.CreateCredential(
                Environment.GetEnvironmentVariable("MONGO_DB_NAME"),
                Environment.GetEnvironmentVariable("MONGO_USERNAME"),
                Environment.GetEnvironmentVariable("MONGO_PASSWORD")
            );
            
            var client = new MongoClient(mongoClientSettings);
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DB_NAME"));
            _sensorData = database.GetCollection<SensorData>(Environment.GetEnvironmentVariable("MONGO_COLLECTION"));
        }

        public List<SensorData> Get()
        {
            return _sensorData.Find(sensorData => true).ToList();
        }
        
        public List<SensorData> Get(DateTime from, DateTime to, List<int> type, List<int> sensor, String sortBy, String order)
        {
            
            var builder = Builders<SensorData>.Filter;
            var filter = builder.Lte("Date", to) &
                         builder.Gte("Date", from);
            if (type.Count > 0)
                filter &= builder.In("SensorType", type);
            if(sensor.Count > 0)
                filter &= builder.In("SensorId", sensor);

            SortDefinition<SensorData> sort = null;
            
            if (sortBy.Equals("Value"))
            {
                if (order.Equals("asc"))
                    sort = Builders<SensorData>.Sort.Ascending("Value");
                else
                    sort = Builders<SensorData>.Sort.Descending("Value");
            }
            else
            {
                if (order.Equals("asc"))
                    sort = Builders<SensorData>.Sort.Ascending("Date");
                else
                    sort = Builders<SensorData>.Sort.Descending("Date");
            }

            if (sort is null)
                return _sensorData.Find(filter).ToList();
            
            return _sensorData.Find(filter).Sort(sort).ToList();
        }
        

        public SensorData Get(int sensorId)
        {
            return _sensorData.Find(sensorData => sensorData.SensorId == sensorId).FirstOrDefault();
        }

        public SensorData Create(SensorData sensorData)
        {
            _sensorData.InsertOne(sensorData);
            return sensorData;
        }

        public void Remove(SensorData sensorDataIn)
        {
            _sensorData.DeleteOne(sensorData => sensorData.SensorId == sensorDataIn.SensorId);
        }

        public void Remove(int id)
        {
            _sensorData.DeleteOne(sensorData => sensorData.SensorId == id);
        }

        public IEnumerable<int> GetListOfIds()
        {
            IEnumerable<int> ids = _sensorData.Find(sensorData => true).ToList().Select(o => o.SensorId).ToList().Distinct();
            return ids;
        }
    }
}