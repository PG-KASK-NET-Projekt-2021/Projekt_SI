using System.Collections.Generic;
using System.Linq;

namespace SI_API.Services
{
    public class AverageDataService
    {
        private readonly SensorDataService _sensorDataService;

        public AverageDataService(SensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }
        
        public List<AverageDataModel> GetLast()
        {
            var ids = _sensorDataService.GetListOfIds();
            List<AverageDataModel> list = new List<AverageDataModel>();
            List<SensorDataModel> data = _sensorDataService.Get();

            foreach (var id in ids)
            {
                List<SensorDataModel> x = data.FindAll(x => x.SensorId == id);

                x.Sort((o1, o2) => o2.Date.CompareTo(o1.Date));
                var y = x.Count >= 100 ? x.GetRange(0, 100) : x;
                //var y = x.OrderByDescending(o => o.Date).Take(100);
                AverageDataModel avg = new AverageDataModel();
                    SensorDataModel last = y.First();
                    avg.Last = last.Value;
                    avg.SensorId = id;
                    avg.SensorType = last.SensorType;
                    foreach(var d in y)
                        avg.Average += d.Value;
                    avg.Average /= y.Count;
                    list.Add(avg);
            }

            return list;
        }
    }
}