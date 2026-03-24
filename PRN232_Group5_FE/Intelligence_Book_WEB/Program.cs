var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("BookAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7287/api/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Cấu hình Route chuẩn cho nhiều Controller
app.MapControllerRoute(
    name: "default",

    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();