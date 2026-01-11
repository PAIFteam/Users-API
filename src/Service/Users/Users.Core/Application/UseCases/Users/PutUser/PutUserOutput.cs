using Users.Core.Domain;
using Users.Core.Domain.Entities;
using Users.Core.Domain.Entities.Base;

namespace Users.Core.Application.UseCases.Users.PutUser
{
    public class PutUserOutPut:OutPutBase
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; }
        public EnumProfile IdProfile { get; set; }
        public User MapToUser()
        {
            return new User(
                IdUser,
                Name,
                Login,
                Password,
                Email,
                DateBirth,
                IdProfile
                );
            }
        }
    
    }
