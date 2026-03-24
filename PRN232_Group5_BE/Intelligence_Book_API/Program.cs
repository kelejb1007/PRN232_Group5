using BLL.Services.User;
using BLL.Services.User.Interfaces;
using DAL.Data;
using DAL.Repositories.User;
using DAL.Repositories.User.Interfaces;
using Intelligence_Book_API.Services.User;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookRepositoryH, BookRepositoryH>();
builder.Services.AddScoped<IBookServiceH, BookServiceH>();

builder.Services.AddScoped<ICartRepositoryH, CartRepositoryH>();
builder.Services.AddScoped<ICartServiceH, CartServiceH>();

builder.Services.AddScoped<ICartRepositoryH, CartRepositoryH>();
builder.Services.AddScoped<ICartServiceH, CartServiceH>();

builder.Services.AddScoped<IOrderRepositoryH, OrderRepositoryH>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddHttpClient<PayOSService>();
//builder.Services.AddScoped<PayOSService>();
var app = builder.Build();

// ================= PIPELINE =================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// phải nằm trước MapControllers
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();