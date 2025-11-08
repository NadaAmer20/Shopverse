using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopverse.Application.Features.Identity.Queries.GetUserById;
using Shopverse.Application.Features.Identity.Queries.GetUsers;

namespace Shopverse.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) { _mediator = mediator; }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var user = await _mediator.Send(new GetUserByIdQuery(Guid.Parse(userIdClaim)));
            return user == null ? NotFound() : Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 0, [FromQuery] bool isPaginated = true)
        {
            var result = await _mediator.Send(new GetUsersQuery(page, pageSize, isPaginated));
            return Ok(result);
        }
    }

}
