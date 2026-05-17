using FluentAssertions;
using Moq;
using Users.Core.Application.UseCases.Users.GetUserById;
using Users.Core.Domain;
using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;

namespace Users.UnitTests;

public class GetUserByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_UsuarioExistente_DeveRetornarUsuarioComJogos()
    {
        var repository = new Mock<IGetUsersRepository>();
        var user = new User(1, "Arthur", "arthur", "hash", "arthur@test.com", DateTime.UtcNow, EnumProfile.Usuario);
        repository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
        repository.Setup(x => x.GetUserGamesAsync(1)).ReturnsAsync(new[] { new UserGame { IdGame = 99, Name = "Game 99" } });
        var sut = new GetUserByIdUseCase(repository.Object);

        var result = await sut.ExecuteAsync(1);

        result.Should().NotBeNull();
        result!.User.Login.Should().Be("arthur");
        result.Games.Should().ContainSingle(x => x.IdGame == 99);
    }

    [Fact]
    public async Task ExecuteAsync_UsuarioInexistente_DeveRetornarNulo()
    {
        var repository = new Mock<IGetUsersRepository>();
        repository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync((User?)null);
        var sut = new GetUserByIdUseCase(repository.Object);

        var result = await sut.ExecuteAsync(1);

        result.Should().BeNull();
        repository.Verify(x => x.GetUserGamesAsync(It.IsAny<int>()), Times.Never);
    }
}
