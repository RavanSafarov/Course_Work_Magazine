using AutoMapper;
using Course_Work_Magazine.DTO.Auth_DTOs;
using Course_Work_Magazine.DTO.Basket_DTOs;
using Course_Work_Magazine.DTO.Category_DTOs;
using Course_Work_Magazine.DTO.Customer_DTOs;
using Course_Work_Magazine.DTO.Order_DTOs;
using Course_Work_Magazine.DTO.OrderItem_DTOs;
using Course_Work_Magazine.DTO.Product_DTOs;
using Course_Work_Magazine.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace Course_Work_Magazine.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Seller, SellerReadDto>();
        CreateMap<SellerCreateUpdateDto, Seller>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());


        CreateMap<Product, ProductReadDto>();
        CreateMap<ProductCreateUpdateDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Seller, opt => opt.Ignore());


        CreateMap<Order, OrderReadDto>()
           .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.SellerId))
           .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));


        CreateMap<OrderCreateUpdateDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());


        CreateMap<OrderItemCreateUpdateDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Sum, opt => opt.Ignore());

        CreateMap<OrderItem, OrderItemReadDto>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src =>
                    src.Product != null ? src.Product.NameOfProduct : null));


        CreateMap<Basket, BasketReadDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.NameOfProduct : null))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0));

        CreateMap<BasketCreateUpdateDto, Basket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());


        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Sellers, opt => opt.Ignore());

        CreateMap<User, AuthReadDto>()
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiredAt, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());


        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));

        CreateMap<Category, CategoryReadDto>();
        CreateMap<CategoryCreateUpdateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());
    }
}