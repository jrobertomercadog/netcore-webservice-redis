using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PulseResponse.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class VitalsController : ControllerBase
    {
        private readonly RedisService _redisService;
        public VitalsController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet]
        [Route("ws/[controller]/{id}")]
        public async Task<object> GetSinglePatientVitals(string Id)
        {
            var vit = await _redisService.HGet("patientvitals", Id);

            VitalSigns serializedVit = null;
            if (!vit.IsNullOrEmpty)
            {
                serializedVit = JsonConvert.DeserializeObject<VitalSigns>(vit.ToString());
            }


            if (serializedVit == null)
            {
                return new object();
            }
            else
            {
                return serializedVit;
            }
        }

    }
}
