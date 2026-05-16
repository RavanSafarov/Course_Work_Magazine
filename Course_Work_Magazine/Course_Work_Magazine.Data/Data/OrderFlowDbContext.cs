using Course_Work_Magazine.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Data;

public class OrderFlowDbContext : IdentityDbContext<User>
{
    public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options) : base(options)
    { }
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Seller>(seller =>
        {
            seller.HasKey(s => s.Id);
            seller.Property(s => s.Name).IsRequired().HasMaxLength(200);
            seller.Property(s => s.Address).HasMaxLength(1000);
            seller.Property(s => s.Email).HasMaxLength(200);
            seller.Property(s => s.PhoneNumber).HasMaxLength(50);
            seller.Property(s => s.CreatedAt).IsRequired();
            seller.HasIndex(s => s.Email).IsUnique();


            seller.HasMany(s => s.Orders)
                .WithOne(o => o.Seller)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            seller.HasMany(s => s.Products)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            seller.HasOne(s => s.User)
                .WithMany(u => u.Sellers)
                .HasForeignKey(s => s.UserId);
        });
        modelBuilder.Entity<Product>(product =>
        {
            product.HasKey(p => p.Id);
            product.Property(p => p.NameOfProduct).HasMaxLength(250).IsRequired();
            product.Property(p => p.Description).HasMaxLength(2000);
            product.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
            product.Property(p => p.ImageUrl).HasMaxLength(1000);


            product.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Order>(order =>
        {
            order.HasKey(o => o.Id);
            order.Property(o => o.CreatedAt).IsRequired();
            order.Property(o => o.Status).IsRequired();

            order.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            order.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<OrderItem>(item =>
        {
            item.HasKey(i => i.Id);
            item.Property(i => i.Service).HasMaxLength(250);
            item.Property(i => i.Rate).HasColumnType("decimal(18,2)");
            item.Property(i => i.Sum).HasColumnType("decimal(18,2)");


            item.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Category>(category =>
        {
            category.HasKey(c => c.Id);
            category.Property(c => c.NameOfCategory).HasMaxLength(200).IsRequired();
        });
        modelBuilder.Entity<Basket>(basket =>
        {
            basket.HasKey(b => b.Id);


            basket.HasOne(b => b.User)
                .WithMany(u => u.Baskets)
                .HasForeignKey(b => b.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            basket.HasOne(b => b.Product)
                .WithMany()
                .HasForeignKey(b => b.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<RefreshToken>(
           refreshToken =>
           {
               refreshToken.HasKey(rt => rt.Id);
               refreshToken.HasIndex(rt => rt.JwtId).IsUnique();
               refreshToken.Property(rt => rt.JwtId)
                   .IsRequired()
                   .HasMaxLength(64);
               refreshToken.Property(rt => rt.UserId)
                   .IsRequired()
                   .HasMaxLength(64);
               refreshToken.Property(rt => rt.Token)
                    .HasMaxLength(2000);

           }
           );
    }
}
