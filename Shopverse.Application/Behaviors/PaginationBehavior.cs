using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
namespace Shopverse.Application.Behaviors
{
    public class PaginationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IRepository<Setting> _settingService;

        public PaginationBehavior(IRepository<Setting> settingService)
        {
            _settingService = settingService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestType = typeof(TRequest);
            var pageProp = requestType.GetProperty("Page");
            var pageSizeProp = requestType.GetProperty("PageSize");

            if (pageProp != null && pageSizeProp != null)
            {
                var currentPageSize = (int?)pageSizeProp.GetValue(request) ?? 0;

                if (currentPageSize <= 0)
                {
                    var defaultSetting = await _settingService
                        .GetQueryable()
                        .FirstOrDefaultAsync(s => s.Key == "DefaultResultsPerPage", cancellationToken);

                    if (defaultSetting != null && int.TryParse(defaultSetting.Value, out var defaultPageSize))
                    {
                        pageSizeProp.SetValue(request, defaultPageSize);
                    }
                    else
                    {
                        pageSizeProp.SetValue(request, 10);
                    }
                }
            }

            return await next();
        }

    }
}
