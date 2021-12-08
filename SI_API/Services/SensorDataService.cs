using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SI_API.Services
{
    public class SensorDataService
    {
        private readonly IMongoCollection<SensorDataModel> _sensorDataModel;

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
            _sensorDataModel = database.GetCollection<SensorDataModel>(Environment.GetEnvironmentVariable("MONGO_COLLECTION"));
        }

        public List<SensorDataModel> Get()
        {
            return _sensorDataModel.Find(sensorDataModel => true).ToList();
        }
        
        public List<SensorDataModel> Get(DateTime from, DateTime to, List<int> type, List<int> sensor, String sortBy, String order)
        {
            
            var builder = Builders<SensorDataModel>.Filter;
            var filter = builder.Lte("Date", to) &
                         builder.Gte("Date", from);
            if (type.Count > 0)
                filter &= builder.In("SensorType", type);
            if(sensor.Count > 0)
                filter &= builder.In("SensorId", sensor);

            SortDefinition<SensorDataModel> sort = null;

            if (sortBy is not null)
            {
                if (sortBy.Equals("Value"))
                {
                    if (order.Equals("asc"))
                        sort = Builders<SensorDataModel>.Sort.Ascending("Value");
                    else
                        sort = Builders<SensorDataModel>.Sort.Descending("Value");
                }
                else
                {
                    if (order.Equals("asc"))
                        sort = Builders<SensorDataModel>.Sort.Ascending("Date");
                    else
                        sort = Builders<SensorDataModel>.Sort.Descending("Date");
                }
            }


            if (sort is null)
                return _sensorDataModel.Find(filter).ToList();
            
            return _sensorDataModel.Find(filter).Sort(sort).ToList();
        }
        

        public SensorDataModel Get(int sensorId)
        {
            return _sensorDataModel.Find(sensorDataModel => sensorDataModel.SensorId == sensorId).FirstOrDefault();
        }

        public SensorDataModel Create(SensorDataModel sensorDataModel)
        {
            _sensorDataModel.InsertOne(sensorDataModel);
            return sensorDataModel;
        }

        public void Remove(SensorDataModel sensorDataModelIn)
        {
            _sensorDataModel.DeleteOne(sensorDataModel => sensorDataModel.SensorId == sensorDataModelIn.SensorId);
        }

        public void Remove(int id)
        {
            _sensorDataModel.DeleteOne(sensorDataModel => sensorDataModel.SensorId == id);
        }

        public IEnumerable<int> GetListOfIds()
        {
            IEnumerable<int> ids = _sensorDataModel.Find(sensorDataModel => true).ToList().Select(o => o.SensorId).ToList().Distinct();
            return ids;
        }
    }
}