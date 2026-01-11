using MediatR;
using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;


namespace Users.Core.Application.UseCases.Users.GetUsers
{
    public class GetUsersUseCase
    {
        private readonly IGetUsersRepository _getUsersRepository;
        private readonly ILogger<GetUsersUseCase> _logger;

        public GetUsersUseCase(
            IGetUsersRepository getUsersRepository,
            ILogger<GetUsersUseCase> logger
        )
        {
            _getUsersRepository = getUsersRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> ExecuteAsync(GetUsersInput input)
        {
            return await _getUsersRepository.GetUsersAsync(input.Login, input.Password, input.Email);
        }
    }
}
