namespace Users.Core.Application.UseCases.Users.GetUsers
{
    public class GetUsersInput
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
