using Course_Work_Magazine.API.Extensions;
using Course_Work_Magazine.Data;
using Course_Work_Magazine.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddDatabase(builder.Configuration)
    .AddIdentityConfiguration()
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationServices()
    .AddRepositories()
    .AddCorsPolicy()
    .AddSwaggerServices()
    .AddFluentValidationConfiguration()
    .AddAutoMapperConfiguration();


builder.Services.AddControllers();


var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
    db.Database.Migrate();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});


await app.SeedRolesAndDatasAsync();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerServices();
}


app.UseCors("AllowReactApp");


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();
