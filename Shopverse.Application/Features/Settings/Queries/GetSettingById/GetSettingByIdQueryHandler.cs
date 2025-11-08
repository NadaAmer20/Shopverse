using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Queries.GetSettingById
{
    public class GetSettingByIdQueryHandler : IRequestHandler<GetSettingByIdQuery, ApiResponse<SettingDto>>
    {
        private readonly IRepository<Setting> _repo;

        public GetSettingByIdQueryHandler(IRepository<Setting> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<SettingDto>> Handle(GetSettingByIdQuery request, CancellationToken ct)
        {
            var setting = await _repo.GetQueryable()
                                     .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

            if (setting == null)
                return ApiResponse<SettingDto>.Fail("Setting not found");

            var dto = new SettingDto(setting);
            return ApiResponse<SettingDto>.Success(dto);
        }
    }
}
