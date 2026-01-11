using Users.Core.Domain.Entities; 

namespace Users.Core.Domain.Interfaces
{
    public interface IPutUserRepository
    {
        Task<int> PutUserAsync(User user);
        bool PutLoginExisteAsync(string login);
        bool PutEmailExisteAsync(string email);

    }
}
