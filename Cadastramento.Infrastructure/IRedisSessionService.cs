using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cadastramento.Infrastructure
{
    public interface IRedisSessionService
    {
        Task<bool> SessionExistsAsync(string sessionId);
        Task<Dictionary<string, string>> GetSessionHashAsync(string sessionId);
    }
}
