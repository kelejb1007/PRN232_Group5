using System.Text;
using BLL.Mapper;
using BLL.Services.Admin;
using BLL.Services.Admin.Interfaces;
using BLL.Services.User;
using BLL.Services.User.Interfaces;
using BLL.Services.Util;
using BLL.Services.Util.Interfaces;
using DAL.Data;
using DAL.Mapper;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;
using DAL.Repositories.User;
using DAL.Repositories.User.Interfaces;
using Intelligence_Book_API.Services;
using Intelligence_Book_API.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
var connectionString = builder.Configuration.GetConnectionString("Intelligence_Book_APIContext")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<Intelligence_Book_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Intelligence_Book_APIContext")
        ?? throw new InvalidOperationException("Connection string 'Intelligence_Book_APIContext' not found.")));

// DI Repo + Service
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<DAL.Repositories.Admin.Interfaces.IOrderRepository, DAL.Repositories.Admin.OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// AutoMapper (scan profile)
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddProfile<CouponProfile>();
    cfg.AddProfile<OrderProfile>();
});

// ================= CONTROLLERS =================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Register Background Services
builder.Services.AddHostedService<OrderCompletionHostedService>();

// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7117")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ================= DEPENDENCY INJECTION =================

// Admin
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookRepository_Anh, BookRepository_Anh>();
builder.Services.AddScoped<IBookService_Anh, BookService_Anh>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<DAL.Repositories.User.Interfaces.IOrderRepository, DAL.Repositories.User.OrderRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICartRepositoryH, CartRepositoryH>();
builder.Services.AddScoped<ICartServiceH, CartServiceH>();
builder.Services.AddScoped<IOrderRepositoryH, OrderRepositoryH>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IBookRepositoryH, BookRepositoryH>();
builder.Services.AddScoped<IBookServiceH, BookServiceH>();

// AutoMapper (MappingProfile for User-side)
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Utils
builder.Services.AddHttpClient();
builder.Services.AddScoped<PayOSService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================= JWT =================
var jwtSettings = builder.Configuration.GetSection("Jwt");

if (string.IsNullOrEmpty(jwtSettings["Key"]))
    throw new Exception("JWT Key is missing in appsettings.json");

var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Auth/Login";
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // dev only
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    // 🔥 lấy token từ cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("access_token", out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// ================= SWAGGER =================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Intelligence Book API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ================= PIPELINE =================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 CORS phải trước Auth
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();