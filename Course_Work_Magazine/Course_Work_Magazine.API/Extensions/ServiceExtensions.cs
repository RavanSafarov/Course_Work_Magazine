using Course_Work_Magazine.BLL.Services;
using Course_Work_Magazine.BLL.Services.Interfaces;
using Course_Work_Magazine.Services;
using Course_Work_Magazine.Services.Interfaces;

namespace Course_Work_Magazine.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISellerService, SellerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBasketService, BasketService>();
      //  services.AddScoped<IStorageService, S3StorageService>();

        return services;
    }
}