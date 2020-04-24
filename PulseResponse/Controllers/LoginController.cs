using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PulseResponse.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        private readonly RedisService _redisService;
        public LoginController(RedisService redisService)
        {
            _redisService = redisService;
        }


        [HttpPost]
        [Route("ws/[controller]")]

        public async Task<object> ExecLogin([FromBody] Login user)
        {
            user.HashSalt = "";
            user.LastLoginDate = "";

            var redislogin = await _redisService.HGet("login", user.Username.Trim());

            Login serializedLogin = null;
            if (!redislogin.IsNullOrEmpty)
            {
                serializedLogin = JsonConvert.DeserializeObject<Login>(redislogin.ToString());
            }

            string localHash = ComputePBKDF2(serializedLogin.HashPassword, serializedLogin.HashSalt);

            if (localHash.Equals(user.HashPassword))
            {
                user.HashSalt = RandomToken();
                user.LastLoginDate = GetEpoch();
            }

            return user;
        }

        private string ComputePBKDF2(string input, string salt)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(input), Encoding.ASCII.GetBytes(salt), 100000);
            byte[] dat =  pbkdf2.GetBytes(32);
            return Convert.ToBase64String(dat).ToLower();
        }

        private string RandomToken()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(10, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(6, false));
            return builder.ToString();
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private string GetEpoch()
        {
            var utcDate = DateTime.Now.ToUniversalTime();
            long baseTicks = 621355968000000000;
            long tickResolution = 10000000;
            long epoch = (utcDate.Ticks - baseTicks) / tickResolution;
            long epochTicks = (epoch * tickResolution) + baseTicks;
            var date = new DateTime(epochTicks, DateTimeKind.Utc);
            return epoch.ToString();
        }

        private string ComputeSha256Hash(string cleartext)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(cleartext));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().ToLower();
            }
        }
    }
}
