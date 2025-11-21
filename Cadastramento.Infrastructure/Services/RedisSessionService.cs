using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;

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
    }
}
