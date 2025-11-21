using System.Threading.Tasks;
using StackExchange.Redis;

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
    }
}
