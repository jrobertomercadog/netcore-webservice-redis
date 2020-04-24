using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace PulseResponse
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;
        private ConnectionMultiplexer _redis;

        public RedisService()
        {
            _redisHost = "127.0.0.1";
            _redisPort = 6379;
        }

        public void Connect()
        {
            try
            {
                var configString = $"{_redisHost}:{_redisPort},connectRetry=5";
                _redis = ConnectionMultiplexer.Connect(configString);
            }
            catch (RedisConnectionException err)
            {
                //Log.Error(err.ToString());
                throw err;
            }
            //Log.Debug("Connected to Redis");
        }

        public async Task<bool> Set(string key, string value)
        {
            var db = _redis.GetDatabase();
            return await db.StringSetAsync(key, value);
        }

        public async Task<string> Get(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public long GetAndIncrement(string key)
        {
            var db = _redis.GetDatabase();
            return db.StringIncrement(key);
        }

        public async Task<RedisValue[]> LRange(string label)
        {
            var db = _redis.GetDatabase();
            return await db.ListRangeAsync(label, 0, 50);
        }

        public async Task<HashEntry[]> HGetAll(string label)
        {
            var db = _redis.GetDatabase();
            return await db.HashGetAllAsync(label);
        }

        public async Task<RedisValue> HGet(string label, string key)
        {
            var db = _redis.GetDatabase();
            return await db.HashGetAsync(label, key);
        }

        public async Task HSet(string label, string key, string value)
        {
            var db = _redis.GetDatabase();
            HashEntry[] e = new HashEntry[] { new HashEntry(key, value) };
            await db.HashSetAsync(label, e);
        }
    }
}
