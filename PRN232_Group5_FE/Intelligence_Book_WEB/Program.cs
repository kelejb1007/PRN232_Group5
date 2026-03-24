using Intelligence_Book_WEB.Services;
using Intelligence_Book_WEB.Services.Interfaces;
using Intelligence_Book_WEB.Services.User;
using Intelligence_Book_WEB.Services.User.Interfaces;
using Intelligence_Book_WEB.Services.Admin;
using Intelligence_Book_WEB.Services.Admin.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddControllersWithViews();
//builder.Services.AddHttpClient("api", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
//});

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("ApiSettings:BaseUrl is missing");

// HttpClient cho AuthService
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});
builder.Services.AddHttpClient<IProfileService, ProfileService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IAddressService, AddressService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IDashboardService, DashboardService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpContextAccessor();

// Named HttpClient nếu chỗ khác trong project có dùng
builder.Services.AddHttpClient("MyAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// ⚠ giữ lại nhưng note rõ (tránh dùng nhầm)
builder.Services.AddHttpClient("MyAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7287/");
});
// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
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

// Cấu hình Route chuẩn cho nhiều Controller
app.MapControllerRoute(
    name: "default",

    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();