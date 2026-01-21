using Users.Core.Domain.Entities; 

namespace Users.Core.Domain.Interfaces
{
    public interface IGetUsersRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(string login, string password);

        Task<User?> GetUserByIdAsync(int idUser);

        Task<IEnumerable<UserGame>> GetUserGamesAsync(int idUser);

    }
}

namespace Users.Core.Domain.Entities
{
    public sealed class UserGame
    {
        public int IdGame { get; set; }
        public string? Name { get; set; }
    }
}
