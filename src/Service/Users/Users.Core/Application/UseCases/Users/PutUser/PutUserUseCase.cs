using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Users.Core.Domain.Entities.Base;
using Users.Core.Domain.Entities.RabbitMQ;
using Users.Core.Domain.Interfaces;
using Users.Core.Entities.RabbitMq;


namespace Users.Core.Application.UseCases.Users.PutUser
{
    public class PutUserUseCase:IPutUserUseCase
    {
        private readonly IPutUserRepository _putUserRepository;
        private readonly ILogger<PutUserUseCase> _logger;
        private readonly IPublisher _publisher;
        private readonly RabbitMqConfigurationSettings _rabbitMqConfigurationSettings;
        public PutUserUseCase(
            IPutUserRepository putUserRepository,
            IPublisher publisher,
            RabbitMqConfigurationSettings rabbitMqConfigurationSettings,
            ILogger<PutUserUseCase> logger

        )
        {
            _putUserRepository = putUserRepository;
            _publisher = publisher;
            _rabbitMqConfigurationSettings = rabbitMqConfigurationSettings;
            _logger = logger;
        }

        public async Task<PutUserOutPut> ExecuteAsync(PutUserInput input)
        {

            _logger.LogInformation("Starting PutUserUseCase.ExecuteAsync");

            try
            {
                
                OutPutBase outPutBase = ValidateInput(input);

                if (!outPutBase.Result)
                {
                    return new PutUserOutPut
                    {
                        Result = false,
                        Message = outPutBase.Message,
                        Exception = outPutBase.Exception
                    };
                }

                int idUser = await _putUserRepository.PutUserAsync(input.MapToUser());

                var message = new WelcomeCustomerMessage(input.Name, input.Login, input.Email);

                await _publisher.Publish(message, _rabbitMqConfigurationSettings.GetQueueAdress());

                PutUserOutPut outPut = new PutUserOutPut
                {
                    IdUser = idUser,
                    Name = input.Name,
                    Login = input.Login,
                    Password = input.Password,
                    Email = input.Email,
                    DateBirth = input.DateBirth,
                    IdProfile = input.IdProfile,
                    Result = true,
                    Message = "User insert successfully",
                    Exception = null

                };

                return outPut;
            }
            catch (Exception ex)
            {
                return new PutUserOutPut
                {
                    Result = false,
                    Message = "Ocorreu umm erro de Runtime Interno",
                    Exception = ex
                };
                
            }

        }

        private OutPutBase ValidateInput(PutUserInput input)
        {
            
            OutPutBase outPut = new OutPutBase();
            if (!IsValidEmail(input.Email)) 
            {
                // Email é inválido
                outPut.Result = false;
                outPut.Message = "E-mail inválido";
                return outPut;

            }
            if (!IsValidPassword(input.Password))
            {
                // Senha é inválida
                // Email é inválido
                outPut.Result = false;
                outPut.Message = "Password com formato inválido";
                return outPut;
            }

            if (IsLoginExistente(input.Login))
            {
                // Senha é inválida
                // Email é inválido
                outPut.Result = false;
                outPut.Message = "Login já cadastrado";
                return outPut;
            }

            if (IsEmailExistente(input.Email))
            {
                // Senha é inválida
                // Email é inválido
                outPut.Result = false;
                outPut.Message = "E-mail já cadastrado";
                return outPut;
            }

            outPut.Result = true;
            // Implement validation logic here
            return outPut;
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Regex simples para validação de e-mail
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            // Pelo menos uma letra maiúscula
            bool hasUpper = password.Any(char.IsUpper);
            // Pelo menos um número
            bool hasDigit = password.Any(char.IsDigit);
            // Pelo menos dois caracteres especiais
            int specialCount = password.Count(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasDigit && specialCount >= 2;
        }
        private bool IsEmailExistente(string email)
        {
            return _putUserRepository.PutEmailExisteAsync(email);
        }
        private bool IsLoginExistente(string login)
        {
            return _putUserRepository.PutLoginExisteAsync(login);
        }

    }
}
