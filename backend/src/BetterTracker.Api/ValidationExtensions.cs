using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BetterTracker.Api;

internal static class ValidationExtensions
{
    public static IEndpointConventionBuilder WithValidation<TType>(
        this IEndpointConventionBuilder builder)
    {
        builder.AddEndpointFilter(async static (context, next) =>
        {
            var services = context.HttpContext.RequestServices;
            var validator = services.GetRequiredService<IValidator<TType>>();
            var cancellationToken = context.HttpContext.RequestAborted;

            foreach (var argument in context.Arguments)
            {
                if (argument is not TType)
                {
                    continue;
                }

                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext, cancellationToken);
                if (validationResult.IsValid)
                {
                    continue;
                }

                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            return await next(context);
        });
        return builder;
    }
}
