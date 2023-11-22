using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using NOTEA.Database;
using NOTEA.Repositories.GenericRepositories;
using NOTEA.Repositories.UserRepositories;
using NOTEA.Services.LogServices;
using NOTEA.Models.OnlineUserListModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ILogsService, LogsService>();
builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IOnlineUserList, OnlineUserList>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=LogIn}/{id?}");

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }