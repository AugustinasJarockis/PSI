using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Services.LogServices;
using NoteaAPI.Models.OnlineUserListModels;
using NoteaAPI.Database;
using Microsoft.EntityFrameworkCore;
using NoteaAPI.Services.FolderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ILogsService, LogsService>();
builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddSingleton<IOnlineUserList, OnlineUserList>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
