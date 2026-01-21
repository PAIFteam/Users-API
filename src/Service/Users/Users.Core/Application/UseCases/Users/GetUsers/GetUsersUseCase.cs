using MediatR;
using Users.Core.Domain.Entities;
using Users.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Users.Core.Domain.Security;


namespace Users.Core.Application.UseCases.Users.GetUsers
{
    public class GetUsersUseCase
    {
        private readonly IGetUsersRepository _getUsersRepository;
        private readonly ILogger<GetUsersUseCase> _logger;
        private readonly IConfiguration _configuration;

        public GetUsersUseCase(
            IGetUsersRepository getUsersRepository,
            ILogger<GetUsersUseCase> logger,
            IConfiguration configuration
        )
        {
            _getUsersRepository = getUsersRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> ExecuteAsync(GetUsersInput input)
        {
            var users = await _getUsersRepository.GetUsersAsync(input.Login, input.Password);
            var user = users.FirstOrDefault();
            if (user is null)
                return Array.Empty<User>();

            var globalSalt = _configuration["Security:PasswordSalt"] ?? string.Empty;
            return PasswordHasher.VerifyPassword(input.Password, user.Password, globalSalt)
                ? new[] { user }
                : Array.Empty<User>();
        }
    }
}
