using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cadastramento.Infrastructure
{
    public interface IRedisSessionService
    {
        Task<bool> SessionExistsAsync(string sessionId);
        Task<Dictionary<string, string>> GetSessionHashAsync(string sessionId);
        Task SaveSessionAsync(string sessionId, Dictionary<string, string> values, int ttlSeconds = 3600);
    }
}
