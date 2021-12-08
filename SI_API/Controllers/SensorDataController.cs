using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SI_API.Services;

namespace SI_API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SensorDataController
    {
        private readonly SensorDataService _sensorDataService;
        private readonly ILogger<SensorDataController> _logger;

        public SensorDataController(ILogger<SensorDataController> logger, SensorDataService sensorDataService)
        {
            _logger = logger;
            _sensorDataService = sensorDataService;
        }
        
        [Route("/api/[controller]/pages")]
        [HttpGet]
        public int GetPages(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] String type,
            [FromQuery] String sensor)
        {
            List<int> typesInt = new List<int>(), sensorsInt  = new List<int>();
            
            if(to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;
            
            if (type != null)
            {
                String[] types;
                types = type.Split(',');
                typesInt = types.Select(x => Int32.Parse(x)).ToList();
            }
            
            if (sensor != null)
            {
                String[] sensors;
                sensors = sensor.Split(',');
                sensorsInt = sensors.Select(x => Int32.Parse(x)).ToList();
            }
            
            return _sensorDataService.GetPagesNumber(from, to, typesInt, sensorsInt);
        }
        
        [Route("/api/[controller]/sensorIdList")]
        [HttpGet]
        public string GetIds()
        {
            return JsonSerializer.Serialize(_sensorDataService.GetListOfIds());
        }
        
        [HttpGet]
        public ActionResult<List<SensorDataModel>> Get(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] String type,
            [FromQuery] String sensor,
            [FromQuery] String sortBy,
            [FromQuery] String order,
            [FromQuery] int page
        )
        {
            List<int> typesInt = new List<int>(), sensorsInt  = new List<int>();
            
            if(to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;
            
            if (type != null)
            {
                String[] types;
                types = type.Split(',');
                typesInt = types.Select(x => Int32.Parse(x)).ToList();
            }
            
            if (sensor != null)
            {
                String[] sensors;
                sensors = sensor.Split(',');
                sensorsInt = sensors.Select(x => Int32.Parse(x)).ToList();
            }

            if (page == 0)
                page = 1;

            return _sensorDataService.Get(from, to, typesInt, sensorsInt, sortBy, order, page);
        }
        
        [Route("/api/[controller]/csv")]
        [HttpGet]
        public string GetCsv(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] String type,
            [FromQuery] String sensor,
            [FromQuery] String sortBy,
            [FromQuery] String order,
            [FromQuery] int page)
        {
            List<int> typesInt = new List<int>(), sensorsInt  = new List<int>();
            
            if(to.Equals(DateTime.MinValue))
                to = DateTime.MaxValue;
            
            if (type != null)
            {
                String[] types;
                types = type.Split(',');
                typesInt = types.Select(x => Int32.Parse(x)).ToList();
            }
            
            if (sensor != null)
            {
                String[] sensors;
                sensors = sensor.Split(',');
                sensorsInt = sensors.Select(x => Int32.Parse(x)).ToList();
            }

            if (page == 0)
                page = 1;
            
            return _sensorDataService.GetCsv(from, to, typesInt, sensorsInt, sortBy, order, page);
        }

        
    }
}