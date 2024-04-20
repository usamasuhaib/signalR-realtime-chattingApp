using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Chat.SignalR.Data;
using Chat.SignalR.Models;
using Chat.SignalR.Hubs;



var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");





builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    //options.Configuration = builder.Configuration.GetConnectionString("Redis");

    options.Configuration = "localhost:6379";
    options.InstanceName = "Rdis_Demo_ChatApp";
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();
