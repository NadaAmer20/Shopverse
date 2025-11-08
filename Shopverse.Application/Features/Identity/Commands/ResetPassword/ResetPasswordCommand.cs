using MediatR;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<ApiResponse<string>>;

}
