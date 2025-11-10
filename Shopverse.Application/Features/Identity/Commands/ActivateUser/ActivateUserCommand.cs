using MediatR;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.ActivateUser
{
    public class ActivateUserCommand : IRequest<ApiResponse<string>>
    {
        public string Token { get; set; }

        public ActivateUserCommand(string token)
        {
            Token = token;
        }
    }

}
