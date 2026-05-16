using Course_Work_Magazine.Data;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OrderFlowDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnectionString")));

        return services;
    }
}
