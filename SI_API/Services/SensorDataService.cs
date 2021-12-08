using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SI_API.Services
{
    public class SensorDataService
    {
        const int PageSize = 50;
        private readonly IMongoCollection<SensorDataModel> _sensorDataModel;

        public SensorDataService()
        {
            var mongoClientSettings = new MongoClientSettings();
            mongoClientSettings.Server = new MongoServerAddress(Environment.GetEnvironmentVariable("MONGO_IP"),
                Int32.Parse(Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017"));
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

        private Tuple<FilterDefinition<SensorDataModel>, SortDefinition<SensorDataModel>> buildFilter(DateTime from, DateTime to, List<int> type, List<int> sensor, String sortBy, String order)
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

            return new Tuple<FilterDefinition<SensorDataModel>, SortDefinition<SensorDataModel>>(filter, sort);
        }
        
        public List<SensorDataModel> Get(DateTime from, DateTime to, List<int> type, List<int> sensor, String sortBy, String order)
        {
            var filterTuple = buildFilter(from, to, type, sensor, sortBy, order);
            var filter = filterTuple.Item1;
            var sort = filterTuple.Item2;
            
            if (sort is null)
                return _sensorDataModel.Find(filter).ToList();
            
            return _sensorDataModel.Find(filter).Sort(sort).ToList();
        }
        
        public List<SensorDataModel> GetPage(DateTime from, DateTime to, List<int> type, List<int> sensor, String sortBy, String order, int page)
        {
            var filterTuple = buildFilter(from, to, type, sensor, sortBy, order);
            var filter = filterTuple.Item1;
            var sort = filterTuple.Item2;
            
            if (sort is null)
                return _sensorDataModel.Find(filter).Skip((page-1) * PageSize).Limit(PageSize).ToList();
            
            return _sensorDataModel.Find(filter).Sort(sort).Skip((page-1) * PageSize).Limit(PageSize).ToList();
        }


        public string GetCsv(DateTime from, DateTime to, List<int> type, List<int> sensor, String sortBy, String order)
        {
            List<SensorDataModel> list = Get(from, to, type, sensor, sortBy, order);
            string csv = "Date, SensorType, SensorId, Value\n";
            return csv + String.Join("\n", list.Select(x => x.ToString()).ToArray());
        }

        public int GetPagesNumber(DateTime from, DateTime to, List<int> type, List<int> sensor)
        {
            var filterTuple = buildFilter(from, to, type, sensor, null, null);
            var filter = filterTuple.Item1;

            long n = _sensorDataModel.CountDocuments(filter);
            if (n % PageSize == 0)
                return (int)(n / PageSize);
            else
                return (int)(n / PageSize + 1);
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