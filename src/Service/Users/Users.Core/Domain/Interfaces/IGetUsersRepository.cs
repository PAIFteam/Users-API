using Users.Core.Domain.Entities; 

namespace Users.Core.Domain.Interfaces
{
    public interface IGetUsersRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(string login, string password, string email);

    }
}
