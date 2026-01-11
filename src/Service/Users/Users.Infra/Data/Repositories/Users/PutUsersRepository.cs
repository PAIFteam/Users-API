using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Users.Infra.Data.Repositories.Users
{
    public class PutUserRepository: IPutUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PutUserRepository> _logger;
        public PutUserRepository(IConfiguration confiuration, ILogger<PutUserRepository> logger)
        {
            _connectionString = confiuration.GetConnectionString("DB_SQL_PAIF_GAMES")
                                    ?? throw new InvalidOperationException("Connection string 'DB_SQL_PAIF_GAMES' not found.");

            _logger = logger;
        }

        private IDbConnection CreateConnection()=> new SqlConnection(_connectionString);

        public async Task<int> PutUserAsync(User user)
        {
            try
            {
                using var connection = CreateConnection();
                string sql = @"INSERT INTO dbo.access_users (name, login, password, email, date_birth, id_profile) 
                               VALUES (@Name, @Login, @Password, @Email, @DateBirth, @IdProfile) SELECT SCOPE_IDENTITY();";

                var result = await connection.ExecuteScalarAsync(sql, user);
                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Falha ao inserir usuário: SCOPE_IDENTITY retornou nulo.");

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users count.");
                throw;
            }
        }
        public bool PutLoginExisteAsync(string login)
        {
            try
            {
                using var connection = CreateConnection();
                string sql = "SELECT 1 FROM dbo.access_users WHERE login = @Login";

                var result = connection.ExecuteScalarAsync(sql, new {Login = login});
                if (result.Result == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users count.");
                throw;
            }
        }
        public  bool PutEmailExisteAsync(string email)
        {
            try
            {
                using var connection = CreateConnection();
                string sql = "SELECT 1 FROM dbo.access_users WHERE email = @Email";

                var result = connection.ExecuteScalarAsync(sql, new { Email = email });
                if (result.Result == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users count.");
                throw;
            }
        }
    }
}
