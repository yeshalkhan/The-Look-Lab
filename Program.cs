using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using The_Look_Lab.Data;
using The_Look_Lab.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepository<Product>, GenericRepository<Product>>();
builder.Services.AddTransient<IRepository<Cart>, GenericRepository<Cart>>();
builder.Services.AddTransient<IRepository<Order>, GenericRepository<Order>>();
builder.Services.AddTransient<IRepository<OrderItem>, GenericRepository<OrderItem>>();
builder.Services.AddTransient<IRepository<User>, GenericRepository<User>>();
builder.Services.AddTransient<IRepository<Category>, GenericRepository<Category>>();
builder.Services.AddTransient<IProductService, ProductRepository>();
builder.Services.AddTransient<ICartService,CartRepository>();
builder.Services.AddTransient<IOrderService,OrderRepository>();
builder.Services.AddTransient<IOrderItemService,OrderItemRepository>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
   {
       option.IdleTimeout = TimeSpan.FromDays(1);
     //  option.Cookie.HttpOnly = true;                 // Make the session cookie accessible only to the server-side code 
   });

builder.Services.AddAuthorization(
    options=> {
        options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

//#pragma warning disable ASP0014 // Suggest using top level route registrations
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "custom",
//        pattern: "{customUrl}",
//        defaults: new { controller = "Home", action = "DisplayMessage" });

//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});
//#pragma warning restore ASP0014 // Suggest using top level route registrations
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
