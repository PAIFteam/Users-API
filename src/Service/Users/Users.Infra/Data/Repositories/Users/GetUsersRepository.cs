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

        public async Task<IEnumerable<User>> GetUsersAsync(string login,  string password)
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
                               WHERE login = @Login";

                return await connection.QueryAsync<User>(sql, new { Login = login });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users count.");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int idUser)
        {
            try
            {
                using var connection = CreateConnection();
                const string sql = @"SELECT   id_user     as IdUser,
                                            name        as Name,
                                            login       as Login,
                                            password    as Password,
                                            email       as Email,
                                            date_birth  as DateBirth,
                                            id_profile as IdProfile
                                   FROM dbo.access_users
                                   WHERE id_user = @IdUser";

                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { IdUser = idUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by id.");
                throw;
            }
        }

        public async Task<IEnumerable<UserGame>> GetUserGamesAsync(int idUser)
        {
            try
            {
                using var connection = CreateConnection();

                // Relacionamento real: sale (id_user -> id_sale) -> sale_item (id_sale -> id_game) -> games
                const string sql = @"SELECT DISTINCT
                                           g.id_game as IdGame,
                                           g.name as Name
                                    FROM dbo.sale s
                                    INNER JOIN dbo.sale_item si ON si.id_sale = s.id_sale
                                    INNER JOIN dbo.games g ON g.id_game = si.id_game
                                    WHERE s.id_user = @IdUser";

                return await connection.QueryAsync<UserGame>(sql, new { IdUser = idUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user games.");
                // Se a tabela não existir ainda, retorna vazio para não quebrar o endpoint.
                return Array.Empty<UserGame>();
            }
        }
    }
}
