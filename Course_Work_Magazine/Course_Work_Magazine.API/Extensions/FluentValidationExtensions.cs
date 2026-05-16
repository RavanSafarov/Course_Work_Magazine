using FluentValidation;
using FluentValidation.AspNetCore;

namespace Course_Work_Magazine.Extensions;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}
