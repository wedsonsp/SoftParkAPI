using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using System;

namespace Cadastramento.Infrastructure
{
    public class RedisSessionService : IRedisSessionService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisSessionService(IConnectionMultiplexer muxer)
        {
            _connectionMultiplexer = muxer;
            _db = _connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> SessionExistsAsync(string sessionId)
        {
            string redisKey = $"sessionEntrevista:{sessionId}";
            return await _db.KeyExistsAsync(redisKey);
        }

        public async Task<Dictionary<string, string>> GetSessionHashAsync(string sessionId)
        {
            string redisKey = $"sessionEntrevista:{sessionId}";
            var entries = await _db.HashGetAllAsync(redisKey);
            var dict = new Dictionary<string, string>();
            foreach (var e in entries)
            {
                dict[e.Name] = e.Value;
            }
            return dict;
        }

        public async Task SaveSessionAsync(string sessionId, Dictionary<string, string> values, int ttlSeconds = 3600)
        {
            string redisKey = $"sessionEntrevista:{sessionId}";
            var hashEntries = new List<HashEntry>();
            foreach (var kv in values)
            {
                hashEntries.Add(new HashEntry(kv.Key, kv.Value));
            }
            if (hashEntries.Count > 0)
            {
                await _db.HashSetAsync(redisKey, hashEntries.ToArray());
            }
            if (ttlSeconds > 0)
            {
                await _db.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(ttlSeconds));
            }
        }
    }
}
