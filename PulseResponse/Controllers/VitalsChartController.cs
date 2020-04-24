using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace PulseResponse.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class VitalsChartController : ControllerBase
    {
        private readonly RedisService _redisService;
        public VitalsChartController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet]
        [Route("ws/[controller]/{id}")]
        public async Task<object> GetSinglePatientChart(string Id)
        {
            var chart = await _redisService.LRange("patientchart-"+Id);

            List<SimpleVitals> lst = new List<SimpleVitals>();
            if (chart.Length > 0)
            {
                foreach (RedisValue item in chart)
                {
                    string[] dat = item.ToString().Split(';');
                    SimpleVitals v = new SimpleVitals
                    {
                        Sp02 = Convert.ToInt32(dat[0]),
                        Pulse = Convert.ToInt32(dat[1]),
                        Epoch = Convert.ToInt64(dat[2])
                    };
                    lst.Add(v);
                }
            }

            VitalsChart vc = new VitalsChart();
            if (lst.Count == 0)
            {
                vc.Id = 0;
                vc.Data = new SimpleVitals[] { };
            }
            else
            {
                vc = new VitalsChart
                {
                    Id = Convert.ToInt32(Id),
                    Data = lst.ToArray()
                };
            }

            return vc;
        }
    }
}
