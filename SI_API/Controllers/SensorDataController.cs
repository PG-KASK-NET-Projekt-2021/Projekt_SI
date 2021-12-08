using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SI_API.Services;

namespace SI_API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SensorDataController
    {
        private readonly ILogger<SensorDataController> _logger;
        private readonly SensorDataService _sensorDataService;
        private readonly AverageDataService _averageDataService;

        public SensorDataController(ILogger<SensorDataController> logger, SensorDataService sensorDataService, AverageDataService averageDataService)
        {
            _logger = logger;
            _sensorDataService = sensorDataService;
            _averageDataService = averageDataService;
        }

        private List<int> parseInts(string type)
        {
            if (type != null)
            {
                string[] types;
                types = type.Split(',');
                return types.Select(x => int.Parse(x)).ToList();
            }

            return new List<int>();
        }

        [Route("/api/[controller]/pages")]
        [HttpGet]
        public int GetPages([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string type,
            [FromQuery] string sensor)
        {
            List<int> typesInt = parseInts(type), sensorsInt = parseInts(sensor);

            if (to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;

            return _sensorDataService.GetPagesNumber(from, to, typesInt, sensorsInt);
        }

        [Route("/api/[controller]/sensorIdList")]
        [HttpGet]
        public string GetIds()
        {
            return JsonSerializer.Serialize(_sensorDataService.GetListOfIds());
        }

        [HttpGet]
        public ActionResult<List<SensorDataModel>> Get([FromQuery] DateTime from, [FromQuery] DateTime to,
            [FromQuery] string type, [FromQuery] string sensor, [FromQuery] string sortBy, [FromQuery] string order,
            [FromQuery] int page
        )
        {
            List<int> typesInt = parseInts(type), sensorsInt = parseInts(sensor);

            if (to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;

            if (page == 0)
                page = 1;

            return _sensorDataService.GetPage(from, to, typesInt, sensorsInt, sortBy, order, page);
        }

        [Route("/api/[controller]/json")]
        [HttpGet]
        public List<SensorDataModel> GetJson([FromQuery] DateTime from, [FromQuery] DateTime to,
            [FromQuery] string type, [FromQuery] string sensor, [FromQuery] string sortBy, [FromQuery] string order)
        {
            List<int> typesInt = parseInts(type), sensorsInt = parseInts(sensor);

            if (to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;

            return _sensorDataService.Get(from, to, typesInt, sensorsInt, sortBy, order);
        }

        [Route("/api/[controller]/csv")]
        [HttpGet]
        public FileContentResult GetCsv([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string type,
            [FromQuery] string sensor, [FromQuery] string sortBy, [FromQuery] string order)
        {
            List<int> typesInt = parseInts(type), sensorsInt = parseInts(sensor);

            if (to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;

            return new FileContentResult(
                System.Text.Encoding.Unicode.GetBytes(_sensorDataService.GetCsv(from, to, typesInt, sensorsInt, sortBy, order)), "text/csv");
        }
        
        
        [Route("/api/[controller]/dashboard")]
        [HttpGet]
        public List<AverageDataModel> GetDashboard()
        {
            return _averageDataService.GetLast();
        }
        
    }
}