using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopverse.Application.Features.Identity.Commands.RegisterUser;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Features.Identity.Commands.ForgotPassword;
using Shopverse.Application.Features.Identity.Commands.LoginUser;
using Shopverse.Application.Features.Identity.Commands.ResetPassword;
using Shopverse.Application.Features.Identity.Commands.VerifyOtp;

namespace Shopverse.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public IdentityController(IMediator mediator) { _mediator = mediator; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var result = await _mediator.Send(new RegisterUserCommand(dto));

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var result = await _mediator.Send(new LoginUserCommand(dto));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest dto)
        {
            var result = await _mediator.Send(new VerifyOtpCommand(dto));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> Forgot([FromBody] ForgotPasswordRequest dto)
        {
            var result = await _mediator.Send(new ForgotPasswordCommand(dto));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest dto)
        {
            var result = await _mediator.Send(new ResetPasswordCommand(dto));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
