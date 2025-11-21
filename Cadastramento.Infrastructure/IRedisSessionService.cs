using System.Threading.Tasks;

namespace Cadastramento.Infrastructure
{
    public interface IRedisSessionService
    {
        Task<bool> SessionExistsAsync(string sessionId);
    }
}
