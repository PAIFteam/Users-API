using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Core.Application.UseCases.Users.PutUser;
using Users.Core.Domain.Entities;

namespace Users.Core.Domain.Interfaces
{
    public interface IPutUserUseCase
    {
        public Task<PutUserOutPut> ExecuteAsync(PutUserInput input);
    }
}
