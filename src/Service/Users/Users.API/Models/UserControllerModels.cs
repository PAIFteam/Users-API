namespace PAIFGAMES.FCG.Api.Models;

public sealed class LoginModel
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class RefreshTokenModel
{
    public string RefreshToken { get; set; } = string.Empty;
}
