using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Microsoft.AspNetCore.Identity;

namespace Course_Work_Magazine.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 5;
        }).AddEntityFrameworkStores<OrderFlowDbContext>().AddDefaultTokenProviders();

        return services;
    }
}
