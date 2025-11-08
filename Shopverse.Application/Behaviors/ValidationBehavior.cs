using FluentValidation;
using MediatR;
using Shopverse.Application.Responses;
using System.Linq;

namespace Behaviors.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var failures = new List<string>();

                foreach (var validator in _validators)
                {
                    var result = await validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken);
                    failures.AddRange(result.Errors.Where(f => f != null).Select(f => f.ErrorMessage));
                }

                if (failures.Any())
                {
                    var responseType = typeof(TResponse);

                    if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                    {
                        var dataType = responseType.GetGenericArguments()[0];
                        var apiResponseType = typeof(ApiResponse<>).MakeGenericType(dataType);
                        var failMethod = apiResponseType.GetMethod("Fail", new[] { typeof(List<string>) });

                        if (failMethod == null)
                        {
                            failMethod = apiResponseType.GetMethod("Fail", new[] { typeof(string), typeof(List<string>) });
                            if (failMethod != null)
                            {
                                var apiResponse = failMethod.Invoke(null, new object[] { "Validation Failed", failures });
                                return (TResponse)apiResponse!;
                            }
                        }
                        else
                        {
                            var apiResponse = failMethod.Invoke(null, new object[] { failures });
                            return (TResponse)apiResponse!;
                        }
                    }
                }
            }

            return await next();
        }

    }
}
