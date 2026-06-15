using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwiftPick.API.Mappings;
using SwiftPick.Core.Entities;
using SwiftPick.Core.Interfaces;
using SwiftPick.Infrastructure.Data;
using SwiftPick.Infrastructure.Repositories;
using SwiftPick.Services.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SwiftPick",
        ValidAudience = jwtSettings["Audience"] ?? "SwiftPickClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// CORS
var frontendUrls = (builder.Configuration.GetValue<string>("FrontendUrl") ?? "http://localhost:5173")
    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrls)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SwiftPick API",
        Version = "v1",
        Description = "API for SwiftPick online store"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwiftPick API v1");
    });
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Database initialization and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply migrations
    await context.Database.MigrateAsync();

    // Create roles
    await SeedRolesAsync(roleManager);

    // Create test users
    await SeedUsersAsync(userManager);

    // Seed categories and products
    await SeedDataAsync(context);
}

app.Run();

static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "User", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
{
    // Admin user
    var adminEmail = "admin@swiftpick.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            Provider = "Local",
            IsActive = true
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "User" });
        }
    }

    // Regular user
    var userEmail = "user@swiftpick.com";
    if (await userManager.FindByEmailAsync(userEmail) == null)
    {
        var user = new ApplicationUser
        {
            UserName = userEmail,
            Email = userEmail,
            FirstName = "Test",
            LastName = "User",
            Provider = "Local",
            IsActive = true
        };
        var result = await userManager.CreateAsync(user, "User123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
        }
    }

    // Manager user
    var managerEmail = "manager@swiftpick.com";
    if (await userManager.FindByEmailAsync(managerEmail) == null)
    {
        var manager = new ApplicationUser
        {
            UserName = managerEmail,
            Email = managerEmail,
            FirstName = "Manager",
            LastName = "User",
            Provider = "Local",
            IsActive = true
        };
        var result = await userManager.CreateAsync(manager, "Manager123!");
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(manager, new[] { "Manager", "User" });
        }
    }
}

static async Task SeedDataAsync(ApplicationDbContext context)
{
    if (context.Categories.Any()) return;

    // Categories
    var categories = new List<Category>
    {
        new Category { Name = "Клавиатуры", Description = "Игровые клавиатуры всех типов" },
        new Category { Name = "Мыши", Description = "Игровые мыши с высокоточными сенсорами" },
        new Category { Name = "Наушники", Description = "Игровые наушники с микрофоном" },
        new Category { Name = "Коврики", Description = "Коврики для мыши различных размеров" },
        new Category { Name = "Геймпады", Description = "Контроллеры для ПК и консолей" },
        new Category { Name = "Мониторы", Description = "Игровые мониторы с высокой частотой обновления" },
        new Category { Name = "Аксессуары", Description = "Дополнительные аксессуары для геймеров" }
    };
    await context.Categories.AddRangeAsync(categories);
    await context.SaveChangesAsync();

    // Products
    var products = new List<Product>
    {
        // Keyboards
        new Product { Name = "Razer BlackWidow V3", Description = "Механическая игровая клавиатура с зелёными свитчами", Price = 12990, Stock = 25, Brand = "Razer", CategoryId = 1, ImagePath = "/images/products/razer-blackwidow.jpg" },
        new Product { Name = "Logitech G Pro X", Description = "Механическая клавиатура для киберспорта", Price = 15990, Stock = 18, Brand = "Logitech", CategoryId = 1, ImagePath = "/images/products/logitech-gpro.jpg" },
        new Product { Name = "HyperX Alloy FPS Pro", Description = "Компактная механическая клавиатура", Price = 8990, Stock = 30, Brand = "HyperX", CategoryId = 1, ImagePath = "/images/products/hyperx-alloy.jpg" },
        new Product { Name = "SteelSeries Apex Pro", Description = "Клавиатура с регулируемой чувствительностью свитчей", Price = 19990, Stock = 12, Brand = "SteelSeries", CategoryId = 1, ImagePath = "/images/products/steelseries-apex.jpg" },
        
        // Mice
        new Product { Name = "Logitech G Pro X Superlight", Description = "Беспроводная мышь весом всего 63 грамма", Price = 14990, Stock = 22, Brand = "Logitech", CategoryId = 2, ImagePath = "/images/products/logitech-superlight.jpg" },
        new Product { Name = "Razer DeathAdder V3 Pro", Description = "Беспроводная мышь с сенсором Focus Pro 30K", Price = 13990, Stock = 20, Brand = "Razer", CategoryId = 2, ImagePath = "/images/products/razer-deathadder.jpg" },
        new Product { Name = "SteelSeries Rival 650", Description = "Беспроводная мышь с двойными сенсорами", Price = 11990, Stock = 15, Brand = "SteelSeries", CategoryId = 2, ImagePath = "/images/products/steelseries-rival.jpg" },
        new Product { Name = "HyperX Pulsefire Haste", Description = "Лёгкая проводная мышь с сотовым корпусом", Price = 5990, Stock = 35, Brand = "HyperX", CategoryId = 2, ImagePath = "/images/products/hyperx-pulsefire.jpg" },
        
        // Headphones
        new Product { Name = "HyperX Cloud II", Description = "Легендарные игровые наушники", Price = 7990, Stock = 40, Brand = "HyperX", CategoryId = 3, ImagePath = "/images/products/hyperx-cloud2.jpg" },
        new Product { Name = "SteelSeries Arctis 7", Description = "Беспроводные наушники с объемным звуком", Price = 12990, Stock = 25, Brand = "SteelSeries", CategoryId = 3, ImagePath = "/images/products/steelseries-arctis7.jpg" },
        new Product { Name = "Razer BlackShark V2 Pro", Description = "Беспроводные наушники с THX Spatial Audio", Price = 14990, Stock = 18, Brand = "Razer", CategoryId = 3, ImagePath = "/images/products/razer-blackshark.jpg" },
        new Product { Name = "Logitech G733", Description = "Беспроводные наушники с RGB подсветкой", Price = 11990, Stock = 22, Brand = "Logitech", CategoryId = 3, ImagePath = "/images/products/logitech-g733.jpg" },
        
        // Mousepads
        new Product { Name = "SteelSeries QcK Heavy", Description = "Толстый коврик для мыши", Price = 1990, Stock = 100, Brand = "SteelSeries", CategoryId = 4, ImagePath = "/images/products/steelseries-qck.jpg" },
        new Product { Name = "Razer Goliathus Extended", Description = "Большой коврик для клавиатуры и мыши", Price = 3490, Stock = 60, Brand = "Razer", CategoryId = 4, ImagePath = "/images/products/razer-goliathus.jpg" },
        new Product { Name = "Logitech G640", Description = "Коврик с оптимальным балансом скорости и контроля", Price = 2490, Stock = 80, Brand = "Logitech", CategoryId = 4, ImagePath = "/images/products/logitech-g640.jpg" },
        
        // Gamepads
        new Product { Name = "Xbox Wireless Controller", Description = "Официальный контроллер Xbox для ПК", Price = 5990, Stock = 30, Brand = "Xbox", CategoryId = 5, ImagePath = "/images/products/xbox-controller.jpg" },
        new Product { Name = "PlayStation DualSense", Description = "Контроллер PlayStation 5", Price = 6990, Stock = 25, Brand = "Sony", CategoryId = 5, ImagePath = "/images/products/dualsense.jpg" },
        new Product { Name = "Logitech F710", Description = "Беспроводной геймпад с вибрацией", Price = 4490, Stock = 20, Brand = "Logitech", CategoryId = 5, ImagePath = "/images/products/logitech-f710.jpg" },
        
        // Monitors
        new Product { Name = "ASUS ROG Swift PG279QM", Description = "27\" IPS 240Hz 1440p", Price = 69990, Stock = 5, Brand = "ASUS", CategoryId = 6, ImagePath = "/images/products/asus-rog.jpg" },
        new Product { Name = "LG UltraGear 27GP850", Description = "27\" Nano IPS 165Hz 1440p", Price = 39990, Stock = 8, Brand = "LG", CategoryId = 6, ImagePath = "/images/products/lg-ultragear.jpg" },
        new Product { Name = "Samsung Odyssey G7", Description = "32\" Curved VA 240Hz 1440p", Price = 54990, Stock = 6, Brand = "Samsung", CategoryId = 6, ImagePath = "/images/products/samsung-odyssey.jpg" },
        
        // Accessories
        new Product { Name = "Razer Mouse Bungee V3", Description = "Держатель для провода мыши", Price = 3990, Stock = 40, Brand = "Razer", CategoryId = 7, ImagePath = "/images/products/razer-bungee.jpg" },
        new Product { Name = "HyperX Wrist Rest", Description = "Подставка для запястий", Price = 2490, Stock = 50, Brand = "HyperX", CategoryId = 7, ImagePath = "/images/products/hyperx-wrist.jpg" },
        new Product { Name = "Logitech G PowerPlay", Description = "Беспроводная зарядка для мыши", Price = 11990, Stock = 10, Brand = "Logitech", CategoryId = 7, ImagePath = "/images/products/logitech-powerplay.jpg" }
    };
    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();
}

