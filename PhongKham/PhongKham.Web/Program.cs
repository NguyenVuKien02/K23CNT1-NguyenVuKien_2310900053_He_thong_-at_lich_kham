using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBusinessServices(builder.Configuration);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

var viCulture = new CultureInfo("vi-VN");
var supportedCultures = new[] { viCulture };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(viCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller=ThongKe}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapAreaControllerRoute(
    name: "LeTanArea",
    areaName: "LeTan",
    pattern: "LeTan/{controller=LichHen}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapAreaControllerRoute(
    name: "BacSiArea",
    areaName: "BacSi",
    pattern: "BacSi/{controller=DanhSachCho}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapAreaControllerRoute(
    name: "BenhNhanArea",
    areaName: "BenhNhan",
    pattern: "BenhNhan/{controller=LichHen}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
