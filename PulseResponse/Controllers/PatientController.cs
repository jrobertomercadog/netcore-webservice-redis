using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PulseResponse.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class PatientController : ControllerBase
    {
        private readonly RedisService _redisService;
        public PatientController(RedisService redisService)
        {
            _redisService = redisService;
        }
        
        [HttpGet]
        [Route("ws/[controller]")]
        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            IList<Patient> file = new List<Patient>();
            var patients = await _redisService.HGetAll("patients");
            if (patients != null && patients.Length > 0)
            {
                foreach (HashEntry p in patients)
                {
                    Patient pat = JsonConvert.DeserializeObject<Patient>(p.Value);
                    file.Add(pat);
                }
            }
            return file;
        }
        
        [HttpGet]
        [Route("ws/[controller]/{id}")]
        public async Task<object> GetSinglePatient(string Id)
        {
            var pat = await _redisService.HGet("patients", Id);

            Patient serializedPat = null;
            if (!pat.IsNullOrEmpty)
            {
                serializedPat = JsonConvert.DeserializeObject<Patient>(pat.ToString());
            }
           

            if (serializedPat == null)
            {
                return new object();
            }
            else
            {
                return serializedPat;
            }
        }

        [HttpPost]
        [Route("ws/[controller]")]

        public async Task<object> CreatePatient([FromBody] Patient pat)
        {
            long id = _redisService.GetAndIncrement("patientid");
            pat.Id = (int)id;
            try
            {
                string serialized = JsonConvert.SerializeObject(pat);
                await _redisService.HSet("patients", pat.Id.ToString(), serialized);
            }
            catch (Exception ex)
            {
                return new object();
            }
            return pat;
        }
    }
}
