using AutoMapper;
using BLL.Mapper;
using BLL.Services.Admin;
using BLL.Services.Admin.Interfaces;
using DAL.Data;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Intelligence_Book_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Intelligence_Book_APIContext")
        ?? throw new InvalidOperationException("Connection string 'Intelligence_Book_APIContext' not found.")));

// DI Repo + Service
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();

// AutoMapper (scan profile)
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<CouponProfile>());

builder.Services.AddControllers();
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