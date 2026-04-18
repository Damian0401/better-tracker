using BetterTracker.Api;
using BetterTracker.Common;
using BetterTracker.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddCommon();
builder.AddDatabase();
builder.Services.AddRepositories();
builder.AddApi();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseApi();
app.MapApi();

app.Run();
