using BetterTracker.Api;
using BetterTracker.Common;
using BetterTracker.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddCommon();
builder.AddDatabase();
builder.Services.AddRepositories();
builder.AddApi();

var app = builder.Build();

app.UseApi();
app.MapApi();

app.Run();
