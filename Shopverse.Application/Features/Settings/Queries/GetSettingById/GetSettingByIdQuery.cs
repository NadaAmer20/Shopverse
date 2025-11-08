using MediatR;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Queries.GetSettingById
{
    public class GetSettingByIdQuery : IRequest<ApiResponse<SettingDto>>
    {
        public Guid Id { get; set; }

        public GetSettingByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
