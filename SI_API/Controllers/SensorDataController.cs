using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SI_API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SensorDataController
    {
        private readonly ILogger<SensorDataController> _logger;
        private readonly Memory _mem;

        public SensorDataController(ILogger<SensorDataController> logger, Memory mem)
        {
            _logger = logger;
            _mem = mem;
        }
        
        [HttpGet]
        public string Get()
        {
            _logger.Log(LogLevel.Information, "GET request");
            return JsonSerializer.Serialize(_mem.Mem);
        }
    }
}