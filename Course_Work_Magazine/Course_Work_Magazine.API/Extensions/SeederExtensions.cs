using Course_Work_Magazine.Data;

namespace Course_Work_Magazine.Extensions;

public static class SeederExtensions
{
    public static async Task SeedRolesAndDatasAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        await RoleSeeder.SeedRolesAsync(services);
        await DataSeeder.SeedDataAsync(services);
    }
}