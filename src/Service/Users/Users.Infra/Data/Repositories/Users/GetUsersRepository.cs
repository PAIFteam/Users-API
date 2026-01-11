using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace Users.Infra.Data.Repositories.Users
{
    public class GetUsersRepository: IGetUsersRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<GetUsersRepository> _logger;
        public GetUsersRepository(IConfiguration confiuration, ILogger<GetUsersRepository> logger)
        {
            _connectionString = confiuration.GetConnectionString("DB_SQL_PAIF_GAMES")
                                    ?? throw new InvalidOperationException("Connection string 'DB_SQL_PAIF_GAMES' not found.");

            _logger = logger;
        }

        private IDbConnection CreateConnection()=> new SqlConnection(_connectionString);

        public async Task<IEnumerable<User>> GetUsersAsync(string login,  string password, string email)
        {
            try
            {
                using var connection = CreateConnection();
                string sql = @"SELECT   id_user     as IdUser,
                                        name        as Name, 
                                        login       as Login, 
                                        password    as Password, 
                                        email       as Email, 
                                        date_birth  as DateBirth, 
                                        id_profile as IdProfile
                               FROM dbo.access_users 
                               WHERE login = @Login AND email = @Email AND password = @Password";
                
                return await connection.QueryAsync<User>(sql, new { Login = login, Email = email, Password = password });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users count.");
                throw;
            }
        }
    }
}
