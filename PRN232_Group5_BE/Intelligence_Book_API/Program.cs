using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DAL.Data;
using BLL.Services.User.Interfaces;
using DAL.Repositories.User.Interfaces;
using DAL.Repositories.User;
using BLL.Services.User;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Intelligence_Book_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Intelligence_Book_APIContext") ?? throw new InvalidOperationException("Connection string 'Intelligence_Book_APIContext' not found.")));

// Add services to the container.
// Repository
builder.Services.AddScoped<IBookRepository_Anh, BookRepository_Anh>();

// Service
builder.Services.AddScoped<IBookService_Anh, BookService_Anh>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
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
