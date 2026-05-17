using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Users.Core.Application.UseCases.Users.PutUser;
using Users.Core.Domain;
using Users.Core.Domain.Interfaces;
using Users.Core.Domain.Security;

namespace Users.UnitTests;

public class PutUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_EmailInvalido_DeveRetornarFalha()
    {
        var repository = new Mock<IPutUserRepository>();
        var sut = CreateSut(repository);

        var result = await sut.ExecuteAsync(CreateInput(email: "invalido"));

        result.Result.Should().BeFalse();
        result.Message.Should().Be("E-mail inválido");
        repository.Verify(x => x.PutUserAsync(It.IsAny<Users.Core.Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PasswordSaltAusente_DeveRetornarFalha()
    {
        var repository = new Mock<IPutUserRepository>();
        var configuration = new ConfigurationBuilder().Build();
        var sut = new PutUserUseCase(repository.Object, Mock.Of<ILogger<PutUserUseCase>>(), configuration);

        var result = await sut.ExecuteAsync(CreateInput());

        result.Result.Should().BeFalse();
        result.Message.Should().Contain("PasswordSalt");
    }

    [Fact]
    public async Task ExecuteAsync_LoginDuplicado_DeveRetornarFalha()
    {
        var repository = new Mock<IPutUserRepository>();
        repository.Setup(x => x.PutLoginExisteAsync(It.IsAny<string>())).Returns(true);
        repository.Setup(x => x.PutEmailExisteAsync(It.IsAny<string>())).Returns(false);
        var sut = new PutUserUseCase(repository.Object, Mock.Of<ILogger<PutUserUseCase>>(), BuildConfiguration());

        var result = await sut.ExecuteAsync(CreateInput());

        result.Result.Should().BeFalse();
        result.Message.Should().Be("Login já cadastrado");
    }

    [Fact]
    public async Task ExecuteAsync_EmailDuplicado_DeveRetornarFalha()
    {
        var repository = new Mock<IPutUserRepository>();
        repository.Setup(x => x.PutLoginExisteAsync(It.IsAny<string>())).Returns(false);
        repository.Setup(x => x.PutEmailExisteAsync(It.IsAny<string>())).Returns(true);
        var sut = new PutUserUseCase(repository.Object, Mock.Of<ILogger<PutUserUseCase>>(), BuildConfiguration());

        var result = await sut.ExecuteAsync(CreateInput());

        result.Result.Should().BeFalse();
        result.Message.Should().Be("E-mail já cadastrado");
    }

    [Fact]
    public async Task ExecuteAsync_SenhaSemCaracteresEspeciais_DeveRetornarFalha()
    {
        var repository = new Mock<IPutUserRepository>();
        var sut = CreateSut(repository);

        var result = await sut.ExecuteAsync(CreateInput(password: "Senha1234"));

        result.Result.Should().BeFalse();
        result.Message.Should().Contain("2 caracteres especiais");
    }

    [Fact]
    public async Task ExecuteAsync_EntradaValida_DevePersistirSenhaHasheada()
    {
        var repository = new Mock<IPutUserRepository>();
        Users.Core.Domain.Entities.User? persistedUser = null;
        repository.Setup(x => x.PutLoginExisteAsync(It.IsAny<string>())).Returns(false);
        repository.Setup(x => x.PutEmailExisteAsync(It.IsAny<string>())).Returns(false);
        repository.Setup(x => x.PutUserAsync(It.IsAny<Users.Core.Domain.Entities.User>()))
            .Callback<Users.Core.Domain.Entities.User>(user => persistedUser = user)
            .ReturnsAsync(123);

        const string salt = "salt-global";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Security:PasswordSalt"] = salt })
            .Build();

        var sut = new PutUserUseCase(repository.Object, Mock.Of<ILogger<PutUserUseCase>>(), configuration);
        var input = CreateInput(password: "Senha123!!");

        var result = await sut.ExecuteAsync(input);

        result.Result.Should().BeTrue();
        result.IdUser.Should().Be(123);
        persistedUser.Should().NotBeNull();
        persistedUser!.Password.Should().NotBe("Senha123!!");
        PasswordHasher.VerifyPassword("Senha123!!", persistedUser.Password, salt).Should().BeTrue();
    }

    private static PutUserUseCase CreateSut(Mock<IPutUserRepository> repository)
    {
        repository.Setup(x => x.PutLoginExisteAsync(It.IsAny<string>())).Returns(false);
        repository.Setup(x => x.PutEmailExisteAsync(It.IsAny<string>())).Returns(false);

        return new PutUserUseCase(repository.Object, Mock.Of<ILogger<PutUserUseCase>>(), BuildConfiguration());
    }

    private static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Security:PasswordSalt"] = "salt-global" })
            .Build();

    private static PutUserInput CreateInput(string email = "user@test.com", string password = "Senha123!!")
        => new()
        {
            IdUser = 0,
            Name = "User Test",
            Login = "usertest",
            Password = password,
            Email = email,
            DateBirth = new DateTime(2000, 1, 1),
            IdProfile = EnumProfile.Usuario
        };
}
