using Bogus;
using Course_Work_Magazine.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Course_Work_Magazine.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<OrderFlowDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();


        if (!context.Categories.Any())
        {
            var categoryNames = new[]
            {
                "Electronics", "Clothing", "Food", "Books", "Home",
                "Sports", "Beauty", "Toys", "Automotive", "Garden"
            };

            var categories = categoryNames.Select(name => new Category
            {
                NameOfCategory = name
            }).ToList();

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        var categoriesList = context.Categories.ToList();


        var normalUsersCount = (await userManager.GetUsersInRoleAsync("User")).Count;
        var usersToCreate = 10 - normalUsersCount;

        if (usersToCreate > 0)
        {
            var emailsSet = new HashSet<string>();
            var userFaker = new Faker<User>()
                .RuleFor(u => u.UserName, f =>
                {
                    var email = f.Internet.Email();
                    while (!emailsSet.Add(email)) email = f.Internet.Email();
                    return email;
                })
                .RuleFor(u => u.Email, (f, u) => u.UserName)
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.Balance, f => f.Finance.Amount(100, 5000))
                .RuleFor(u => u.CreatedAt, f => DateTimeOffset.UtcNow)
                .RuleFor(u => u.UpdatedAt, f => DateTimeOffset.UtcNow);

            var users = userFaker.Generate(usersToCreate);

            foreach (var user in users)
            {
                var result = await userManager.CreateAsync(user, "User123!");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Couldn't create {user.Email}: {errors}");
                }


                await userManager.AddToRoleAsync(user, "User");
            }
        }

        var usersList = userManager.Users.ToList();
        if (!usersList.Any())
        {
            throw new Exception("Users weren't created");
        }


        var sellerFaker = new Faker<Seller>()
            .RuleFor(s => s.Name, f => f.Company.CompanyName())
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.Address, f => f.Address.FullAddress())
            .RuleFor(s => s.CreatedAt, f => f.Date.PastOffset())
            .RuleFor(s => s.UpdatedAt, f => f.Date.RecentOffset());

        var existingUserIds = context.Sellers.Select(s => s.UserId).ToHashSet();
        var sellersToAdd = usersList
            .Where(u => !existingUserIds.Contains(u.Id))
            .Select(user =>
            {
                var seller = sellerFaker.Generate();
                seller.UserId = user.Id;
                return seller;
            })
            .ToList();

        if (sellersToAdd.Any())
        {
            context.Sellers.AddRange(sellersToAdd);
            await context.SaveChangesAsync();
        }

        var sellerList = context.Sellers.ToList();


        var categoryProducts = new Dictionary<string, string[]>
        {
            ["Electronics"] = new[] { "Laptop", "Smartphone", "Headphones", "Monitor", "Keyboard", "Mouse", "Tablet", "Smartwatch", "Camera", "Speaker", "Microphone", "Drone", "Charger", "Power Bank", "Router" },
            ["Clothing"] = new[] { "T-Shirt", "Jeans", "Jacket", "Sneakers", "Hoodie", "Shorts", "Dress", "Coat", "Sweater", "Cap", "Socks", "Suit", "Skirt", "Boots", "Scarf" },
            ["Food"] = new[] { "Pizza", "Burger", "Pasta", "Salad", "Sushi", "Steak", "Sandwich", "Soup", "Fries", "Ice Cream", "Cake", "Donut", "Chocolate", "Cheese", "Bread" },
            ["Books"] = new[] { "Novel", "Science Book", "Biography", "Comics", "Dictionary", "Fantasy Book", "History Book", "Programming Book", "Math Book", "Poetry", "Thriller", "Romance Novel", "Horror Book", "Guide Book", "Encyclopedia" },
            ["Home"] = new[] { "Chair", "Table", "Sofa", "Lamp", "Bed", "Wardrobe", "Shelf", "Carpet", "Curtains", "Mirror", "Desk", "Pillow", "Blanket", "Clock", "Drawer" },
            ["Sports"] = new[] { "Football", "Basketball", "Tennis Racket", "Running Shoes", "Dumbbells", "Yoga Mat", "Bicycle", "Helmet", "Gloves", "Fitness Tracker", "Skipping Rope", "Punching Bag", "Swimsuit", "Water Bottle", "Backpack" },
            ["Beauty"] = new[] { "Lipstick", "Perfume", "Foundation", "Mascara", "Shampoo", "Conditioner", "Face Cream", "Body Lotion", "Nail Polish", "Eyeliner", "Hair Dryer", "Straightener", "Brush Set", "Serum", "Makeup Kit" },
            ["Toys"] = new[] { "Action Figure", "Doll", "Puzzle", "Lego Set", "Toy Car", "Board Game", "Plush Toy", "RC Car", "Train Set", "Robot Toy", "Ball", "Kite", "Water Gun", "Blocks", "Drone Toy" },
            ["Automotive"] = new[] { "Car Cover", "Seat Cover", "Steering Wheel", "Car Vacuum", "Dash Cam", "GPS Navigator", "Motor Oil", "Tire Pump", "Car Charger", "Battery", "Headlights", "Brake Pads", "Air Filter", "Toolkit", "Car Mat" },
            ["Garden"] = new[] { "Shovel", "Rake", "Lawn Mower", "Water Hose", "Plant Pot", "Garden Chair", "Seeds", "Fertilizer", "Gloves", "Sprinkler", "Wheelbarrow", "Hedge Trimmer", "Soil", "Table", "Lantern" }
        };

        if (!context.Products.Any())
        {
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.CategoryId, f => categoriesList[f.Random.Int(0, categoriesList.Count - 1)].Id)
                .RuleFor(p => p.NameOfProduct, (f, p) =>
                {
                    var category = categoriesList.First(c => c.Id == p.CategoryId);
                    if (categoryProducts.TryGetValue(category.NameOfCategory, out var products))
                        return $"{f.PickRandom(products)} {f.UniqueIndex}";
                    return f.Commerce.ProductName();
                })
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Random.Decimal(10, 2000))
                .RuleFor(p => p.ImageUrl, (f, p) =>
                {
                    var category = categoriesList.First(c => c.Id == p.CategoryId);
                    var seed = category.NameOfCategory.Replace(" ", "").ToLower();
                    return $"https://picsum.photos/seed/{seed}/300/300";
                })
                .RuleFor(p => p.SellerId, f => sellerList[f.Random.Int(0, sellerList.Count - 1)].Id);

            var products = productFaker.Generate(25);
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
