using MediatR;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Commands.CreateSetting
{
    public class CreateSettingCommand : IRequest<ApiResponse<SettingDto>>
    {
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string>? Values { get; set; }
        public IFormFile? Attachment { get; set; }

    }
}
