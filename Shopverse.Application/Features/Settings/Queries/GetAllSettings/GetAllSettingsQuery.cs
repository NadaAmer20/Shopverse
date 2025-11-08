using MediatR;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Queries.GetAllSettings
{
    public class GetAllSettingsQuery : IRequest<ApiResponse<List<SettingDto>>>
    {
        public string? Key { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public bool DisablePagination { get; set; } = false;
    }
}
