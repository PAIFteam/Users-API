using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Users.Core.Application.UseCases.Users.GetUsers;
using Users.Core.Domain;
using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;
using Users.Core.Domain.Security;

namespace Users.UnitTests;

public class GetUsersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_LoginExistenteSenhaCorreta_DeveRetornarUsuario()
    {
        const string salt = "salt-global";
        var user = new User(1, "Arthur", "arthur", PasswordHasher.HashPassword("Senha123!!", salt), "arthur@test.com", DateTime.UtcNow, EnumProfile.Usuario);
        var repository = new Mock<IGetUsersRepository>();
        repository.Setup(x => x.GetUsersAsync("arthur", "Senha123!!")).ReturnsAsync(new[] { user });

        var sut = new GetUsersUseCase(repository.Object, Mock.Of<ILogger<GetUsersUseCase>>(), BuildConfiguration(salt));

        var result = await sut.ExecuteAsync(new GetUsersInput { Login = "arthur", Password = "Senha123!!" });

        result.Should().ContainSingle().Which.Login.Should().Be("arthur");
    }

    [Fact]
    public async Task ExecuteAsync_LoginExistenteSenhaIncorreta_DeveRetornarVazio()
    {
        const string salt = "salt-global";
        var user = new User(1, "Arthur", "arthur", PasswordHasher.HashPassword("Senha123!!", salt), "arthur@test.com", DateTime.UtcNow, EnumProfile.Usuario);
        var repository = new Mock<IGetUsersRepository>();
        repository.Setup(x => x.GetUsersAsync("arthur", "errada")).ReturnsAsync(new[] { user });

        var sut = new GetUsersUseCase(repository.Object, Mock.Of<ILogger<GetUsersUseCase>>(), BuildConfiguration(salt));

        var result = await sut.ExecuteAsync(new GetUsersInput { Login = "arthur", Password = "errada" });

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_LoginInexistente_DeveRetornarVazio()
    {
        var repository = new Mock<IGetUsersRepository>();
        repository.Setup(x => x.GetUsersAsync("arthur", "Senha123!!")).ReturnsAsync(Array.Empty<User>());

        var sut = new GetUsersUseCase(repository.Object, Mock.Of<ILogger<GetUsersUseCase>>(), BuildConfiguration("salt-global"));

        var result = await sut.ExecuteAsync(new GetUsersInput { Login = "arthur", Password = "Senha123!!" });

        result.Should().BeEmpty();
    }

    private static IConfiguration BuildConfiguration(string salt)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Security:PasswordSalt"] = salt })
            .Build();
}
