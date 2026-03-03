using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DAL.Data;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;
using BLL.Services.Admin.Interfaces;
using BLL.Services.Admin;
using DAL.Mapper;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Intelligence_Book_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Intelligence_Book_APIContext") ?? throw new InvalidOperationException("Connection string 'Intelligence_Book_APIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
