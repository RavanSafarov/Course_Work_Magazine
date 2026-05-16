using AutoMapper;
using Course_Work_Magazine.Mappings;

namespace Course_Work_Magazine.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
