using MediatR;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Queries.GetAllSettings
{
    public class GetAllSettingsQueryHandler : IRequestHandler<GetAllSettingsQuery, ApiResponse<List<SettingDto>>>
    {
        private readonly IRepository<Setting> _repo;

        public GetAllSettingsQueryHandler(IRepository<Setting> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<SettingDto>>> Handle(GetAllSettingsQuery request, CancellationToken ct)
        {
            var query = _repo.GetQueryable();

            if (!string.IsNullOrEmpty(request.Key))
            {
                query = query.Where(s => s.Key.Contains(request.Key));
            }

            int totalCount = 0;
            Pagination? pagination = null;

            if (!request.DisablePagination)
            {
                totalCount = query.Count();
                query = query.Skip((request.Page - 1) * request.PageSize)
                             .Take(request.PageSize);
            }

            var settings = query.ToList();
            var dtos = settings.Select(s => new SettingDto(s)).ToList();

            if (!request.DisablePagination)
            {
                pagination = new Pagination
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    CurrentRecords = dtos.Count
                };
            }

            return ApiResponse<List<SettingDto>>.Success(dtos, pagination: pagination);
        }
    }
}
