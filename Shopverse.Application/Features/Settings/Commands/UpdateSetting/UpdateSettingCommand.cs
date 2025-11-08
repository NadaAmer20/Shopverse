using MediatR;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Commands.UpdateSetting
{
    public class UpdateSettingCommand : IRequest<ApiResponse<SettingDto>>
    {
        public Guid Id { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public List<string>? Values { get; set; }
        public IFormFile? Attachment { get; set; }


    }
}
