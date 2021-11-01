using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using si.Services;

namespace si.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SensorController
    {
        private const string Topic = "SENSOR_TOPIC";
        private readonly ILogger<SensorController> _logger;

        public SensorController(ILogger<SensorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.Log(LogLevel.Information, "GET request");
            return "OK";
        }

        [HttpPost]
        public async void Post([FromBody] SensorData sensorData, [FromServices] IQueueService queueService)
        {
            _logger.Log(LogLevel.Information, "POST request");

            var data = new SensorQueueData
            {
                SensorId = sensorData.SensorId,
                SensorType = sensorData.SensorType,
                Value = sensorData.Value,
                Date = DateTime.Now
            };

            queueService.Publish(data, Topic);
        }
    }
}