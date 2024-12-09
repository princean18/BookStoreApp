using BookStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookStoreApp.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookStoreAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreAppContext") ?? throw new InvalidOperationException("Connection string 'PrintOrderContext' not found.")));
builder.Services.BuildServiceProvider().GetService<BookStoreAppContext>().Database.Migrate();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();


builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".BookStoreApp.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(300);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    //  The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(name: "Dashboard",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(name: "Books",
    pattern: "{controller=Books}/{action=index}/{id?}");
app.MapControllerRoute(name: "addbooks",
    pattern: "{controller=Books}/{action=index}/{id?}");
app.MapControllerRoute(name: "editbooks",
    pattern: "{controller=Books}/{action=editbooks}/{id?}");
app.MapControllerRoute(name: "Users",
    pattern: "{controller=Users}/{action=index}/{id?}");
app.MapControllerRoute(name: "GenerateReport",
    pattern: "{controller=Reports}/{action=GenerateReport}/{type?}");
app.MapControllerRoute(name: "Login",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();


