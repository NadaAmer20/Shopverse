using MediatR;
using Microsoft.AspNetCore.Mvc;
using PNUStudentPortal.Application.DTOs.Roles;
using Shopverse.Application.Features.Roles.Commands.AssignRolesToUser;
using Shopverse.Application.Features.Roles.Commands.CreateRole;
using Shopverse.Application.Features.Roles.Commands.DeleteRole;
using Shopverse.Application.Features.Roles.Commands.RemoveRolesFromUser;
using Shopverse.Application.Features.Roles.Commands.UpdateRole;
using Shopverse.Application.Features.Roles.Queries.GetRoleById;
using Shopverse.Application.Features.Roles.Queries.GetRoles;

namespace Shopverse.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetRolesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var result = await _mediator.Send(new CreateRoleCommand(request));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, request));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            return Ok(result);
        }

        [HttpPost("users/{userId}/assign-roles")]
        public async Task<IActionResult> AssignRolesToUser(Guid userId, [FromBody] Guid roleId)
        {
            var result = await _mediator.Send(new AssignRolesToUserCommand(userId, roleId));
            return Ok(result);
        }

        [HttpPost("users/{userId}/remove-roles")]
        public async Task<IActionResult> RemoveRolesFromUser(Guid userId, [FromBody]Guid roleId)
        {
            var result = await _mediator.Send(new RemoveRolesFromUserCommand(userId, roleId));
            return Ok(result);
        }
    }
}
