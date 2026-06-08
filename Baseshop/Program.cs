using Baseshop.Interface;
using Baseshop.Models;
using Baseshop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Products"; // 未登入會導向此頁
//        options.AccessDeniedPath = "/Products"; // 沒有權限會導向此頁
//        //options.ExpireTimeSpan = TimeSpan.FromSeconds(2); //登入的期限
//    });

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<WebContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("WebDatabase")));

builder.Services.AddScoped<IUsersService, UsersService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Products}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
