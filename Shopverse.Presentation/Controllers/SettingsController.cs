using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopverse.Application.Features.Settings.Commands.CreateSetting;
using Shopverse.Application.Features.Settings.Commands.UpdateSetting;
using Shopverse.Application.Features.Settings.Queries.GetAllSettings;
using Shopverse.Application.Features.Settings.Queries.GetSettingById;

namespace Shopverse.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllSettingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSettingByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateSettingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateSettingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

       
    }
}