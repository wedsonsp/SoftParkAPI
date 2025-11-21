using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Cadastramento.Core;

namespace Cadastramento.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            using var conn = new SqlConnection(_connectionString);
            string sqlData = @"SELECT u.id AS Id,
                                      u.nome AS Username,
                                      CAST(u.status_fl AS bit) AS Status,
                                      up.perfil AS Perfil
                               FROM usuario u
                               LEFT JOIN usuario_perfil up ON u.id = up.usuarioid
                               ORDER BY u.id
                               OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            string sqlCount = "SELECT COUNT(*) FROM usuario";

            var lookup = new Dictionary<int, User>();
            var users = await conn.QueryAsync<User, string, User>(sqlData,
                (user, perfil) =>
                {
                    if (!lookup.TryGetValue(user.Id, out var u))
                    {
                        u = user;
                        u.Perfis = new List<string>();
                        lookup.Add(u.Id, u);
                    }
                    if (perfil != null) u.Perfis.Add(perfil);
                    return u;
                },
                new { Offset = (page - 1) * pageSize, PageSize = pageSize }
            );
            var totalCount = await conn.ExecuteScalarAsync<int>(sqlCount);
            return (lookup.Values, totalCount);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            string sql = @"SELECT u.id AS Id,
                                  u.nome AS Username,
                                  CAST(u.status_fl AS bit) AS Status,
                                  up.perfil AS Perfil
                           FROM usuario u
                           LEFT JOIN usuario_perfil up ON u.id = up.usuarioid
                           WHERE u.id = @Id";
            var lookup = new Dictionary<int, User>();
            await conn.QueryAsync<User, string, User>(sql,
                (user, perfil) =>
                {
                    if (!lookup.TryGetValue(user.Id, out var u))
                    {
                        u = user;
                        u.Perfis = new List<string>();
                        lookup.Add(u.Id, u);
                    }
                    if (perfil != null) u.Perfis.Add(perfil);
                    return u;
                }, new { Id = id });
            return lookup.Values.FirstOrDefault();
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                string insertUser = "INSERT INTO usuario (nome, status_fl) VALUES (@Username, @Status); SELECT SCOPE_IDENTITY();";
                var newId = await conn.ExecuteScalarAsync<int>(insertUser, new { user.Username, Status = user.Status ? 1 : 0 }, tx);
                string insertPerfil = "INSERT INTO usuario_perfil (usuarioid, perfil) VALUES (@UserId, @Perfil);";
                foreach (var perfil in user.Perfis)
                {
                    await conn.ExecuteAsync(insertPerfil, new { UserId = newId, Perfil = perfil }, tx);
                }
                tx.Commit();
                return newId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                string updateUser = "UPDATE usuario SET nome=@Username, status_fl=@Status WHERE id=@Id";
                await conn.ExecuteAsync(updateUser, new { user.Username, Status = user.Status ? 1 : 0, user.Id }, tx);
                string deletePerfis = "DELETE FROM usuario_perfil WHERE usuarioid=@Id";
                await conn.ExecuteAsync(deletePerfis, new { user.Id }, tx);
                string insertPerfil = "INSERT INTO usuario_perfil (usuarioid, perfil) VALUES (@UserId, @Perfil);";
                foreach (var perfil in user.Perfis)
                {
                    await conn.ExecuteAsync(insertPerfil, new { UserId = user.Id, Perfil = perfil }, tx);
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }

    public interface IUserRepository
    {
        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<User> GetByIdAsync(int id);
        Task<int> CreateAsync(User user);
        Task UpdateAsync(User user);
    }
}
