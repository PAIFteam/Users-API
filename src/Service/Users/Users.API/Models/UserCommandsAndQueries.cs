namespace PAIFGAMES.FCG.Domain.Users.Commands;

public sealed class RegisterUserCommand
{
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class AttachRoleUserCommand
{
    public Guid UserUId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public sealed class DetachRoleUserCommand
{
    public Guid UserUId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public sealed class AttachGameUserCommand
{
    public Guid UserUId { get; set; }
    public Guid GameUId { get; set; }
}

public sealed class DetachGameUserCommand
{
    public Guid UserUId { get; set; }
    public Guid GameUId { get; set; }
}
