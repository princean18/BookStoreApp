using BookStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookStoreApp.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookStoreAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreAppContext") ?? throw new InvalidOperationException("Connection string 'PrintOrderContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

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

app.Run();


