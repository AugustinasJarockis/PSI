using NOTEA.Services.LogServices;
using NOTEA.Models.OnlineUserListModels;
using System.Security.Policy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogsService, LogsService>();
builder.Services.AddScoped<IUserLogService, UserLogService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IOnlineUserList, OnlineUserList>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
});

var apiBaseAdress = builder.Configuration.GetValue<Uri>("BaseUri");

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Use(async (context, next) =>
{
    var userLogService = context.RequestServices.GetRequiredService<IUserLogService>();
    int user_id = context.Session.GetInt32("Id") ?? default;
    userLogService.SaveLogInfo($"Request: {context.Request.Path}", user_id);
    Console.WriteLine(context.Session.GetString("User"));
    await next.Invoke();

    userLogService.SaveLogInfo($"Response: {context.Response.StatusCode}", user_id);
    userLogService.SeparateLogInfo(user_id);

});

app.Run();


