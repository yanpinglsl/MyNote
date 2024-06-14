using FluentValidation;
using MediatR;
using ValidationException = YY.Zhihu.UseCases.Common.Exceptions.ValidationException;

namespace YY.Zhihu.UseCases.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validatorResults = await Task.WhenAll(
                    validators.Select(l => l.ValidateAsync(context, cancellationToken)));
                var failures = validatorResults.Where(l=>l.Errors.Count != 0)
                    .SelectMany(l=>l.Errors)
                    .ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }
           return await next();   
        }
    }
}
