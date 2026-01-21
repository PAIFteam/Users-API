
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Users.Core.Domain.Entities.Base;
using Users.Core.Domain.Entities.RabbitMQ;
using Users.Core.Domain.Interfaces;
using Users.Core.Domain.Security;


namespace Users.Core.Application.UseCases.Users.PutUser
{
    public class PutUserUseCase:IPutUserUseCase
    {
        private readonly IPutUserRepository _putUserRepository;
        private readonly ILogger<PutUserUseCase> _logger;
        private readonly IConfiguration _configuration;
        public PutUserUseCase(
            IPutUserRepository putUserRepository,
            ILogger<PutUserUseCase> logger,
            IConfiguration configuration

        )
        {
            _putUserRepository = putUserRepository;
            _logger = logger;
            _configuration = configuration;
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

                var globalSalt = _configuration["Security:PasswordSalt"];
                if (string.IsNullOrWhiteSpace(globalSalt))
                {
                    return new PutUserOutPut
                    {
                        Result = false,
                        Message = "Configuração de segurança inválida (PasswordSalt).",
                        Exception = null
                    };
                }

                input.Password = PasswordHasher.HashPassword(input.Password, globalSalt);

                int idUser = await _putUserRepository.PutUserAsync(input.MapToUser());

                PutUserOutPut outPut = new PutUserOutPut
                {
                    IdUser = idUser,
                    Name = input.Name,
                    Login = input.Login,
                    Password = null,
                    Email = input.Email,
                    DateBirth = input.DateBirth,
                    IdProfile = input.IdProfile,
                    Result = true,
                    Message = "Usuário registrado com sucessso!",
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
                outPut.Result = false;
                outPut.Message = GetPasswordValidationMessage(input.Password);
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
            bool hasUpper = password.Any(char.IsLetter);
            // Pelo menos um número
            bool hasDigit = password.Any(char.IsDigit);
            // Pelo menos dois caracteres especiais
            int specialCount = password.Count(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasDigit && specialCount >= 2;
        }

        private string GetPasswordValidationMessage(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password é obrigatório.";

            var errors = new List<string>();

            if (password.Length < 8)
                errors.Add("deve ter no mínimo 8 caracteres");

            if (!password.Any(char.IsLetter))
                errors.Add("deve conter ao menos 1 letra");

            if (!password.Any(char.IsDigit))
                errors.Add("deve conter ao menos 1 número");

            var specialCount = password.Count(c => !char.IsLetterOrDigit(c));
            if (specialCount < 2)
                errors.Add("deve conter ao menos 2 caracteres especiais");

            return errors.Count == 0
                ? "Password com formato inválido."
                : $"Password inválido: {string.Join(", ", errors)}.";
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
