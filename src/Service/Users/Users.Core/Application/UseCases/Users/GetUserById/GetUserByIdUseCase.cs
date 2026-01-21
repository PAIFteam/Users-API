using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;

namespace Users.Core.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdUseCase
{
    private readonly IGetUsersRepository _getUsersRepository;

    public GetUserByIdUseCase(IGetUsersRepository getUsersRepository)
    {
        _getUsersRepository = getUsersRepository;
    }

    public async Task<GetUserByIdOutput?> ExecuteAsync(int idUser)
    {
        var user = await _getUsersRepository.GetUserByIdAsync(idUser);
        if (user is null)
            return null;

        var games = await _getUsersRepository.GetUserGamesAsync(idUser);
        return new GetUserByIdOutput(user, games);
    }
}

public sealed record GetUserByIdOutput(User User, IEnumerable<UserGame> Games);
