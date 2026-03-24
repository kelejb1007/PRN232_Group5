using Intelligence_Book_WEB.Services;
using Intelligence_Book_WEB.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddControllersWithViews();
//<<<<<<< HEAD
//builder.Services.AddHttpClient("api", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
//});
//=======
//>>>>>>> 25a8230ed3082f6f4f27000bda08913d808fb211

// ✅ lấy base URL an toàn
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new Exception("ApiSettings:BaseUrl chưa cấu hình trong appsettings.json");
}

// ✅ AuthService dùng HttpClient riêng
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpContextAccessor();

// ✅ HttpClient chính cho toàn app
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// ⚠ giữ lại nhưng note rõ (tránh dùng nhầm)
builder.Services.AddHttpClient("MyAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7287/");
});

// ================= PIPELINE =================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();