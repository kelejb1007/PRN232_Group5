using BLL.Services.Admin;
using BLL.Services.Admin.Interfaces;
using DAL.Data;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DAL.Data;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;
using BLL.Services.Admin.Interfaces;
using BLL.Services.Admin;
using DAL.Mapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DAL.Repositories.User.Interfaces;
using BLL.Services.User.Interfaces;
using BLL.Services.User;
using DAL.Repositories.User;
var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
builder.Services.AddDbContext<Intelligence_Book_APIContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Intelligence_Book_APIContext")
        ?? throw new InvalidOperationException("Connection string not found.")
    ));

// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// ================= SERVICES =================
builder.Services.AddControllers();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBookRepositoryH, BookRepositoryH>();
builder.Services.AddScoped<IBookServiceH, BookServiceH>();
builder.Services.AddScoped<ICartRepositoryH, CartRepositoryH>();
builder.Services.AddScoped<ICartServiceH, CartServiceH>();
builder.Services.AddScoped<ICartRepositoryH, CartRepositoryH>();
builder.Services.AddScoped<ICartServiceH, CartServiceH>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Phải cấu hình CORS để cho phép Client MVC gọi tới.
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token. Example: eyJhbGciOi..."
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

// ⚠ CORS phải nằm trước MapControllers
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();