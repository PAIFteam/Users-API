using Users.API.Enums;

namespace Users.API.Models; 

public class UserModel
{
    public int IdUser { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime DateBirth { get; set; }
    public EnumProfile IdProfile { get; set; }

    public UserModel() { }

    public UserModel(int idUser, string name, string login, string password, string email, DateTime dateBirth, EnumProfile idProfile)
    {
        IdUser = idUser;
        Name = name;
        Login = login;
        Password = password;
        Email = email;
        DateBirth = dateBirth;
        IdProfile = idProfile;
    }
}
