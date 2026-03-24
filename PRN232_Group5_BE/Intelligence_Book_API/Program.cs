using AutoMapper;
using BLL.Mapper;
using BLL.Services.Admin;
using BLL.Services.Admin.Interfaces;
using DAL.Data;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Intelligence_Book_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Intelligence_Book_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Intelligence_Book_APIContext")
        ?? throw new InvalidOperationException("Connection string 'Intelligence_Book_APIContext' not found.")));

// DI Repo + Service
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// AutoMapper (scan profile)
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddProfile<CouponProfile>();
    cfg.AddProfile<OrderProfile>();
});

// Register Background Services
builder.Services.AddHostedService<OrderCompletionHostedService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();