using Microsoft.AspNetCore.Components;
using PokerLibrary;
using PokerTimer6;
using PokerTimer6.Data;
using PokerTimer6.Data.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddTransient<IDataAccess, DataAccess>();
builder.Services.AddTransient<IPokerData, PokerData>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IPayoutService, PayoutService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddScoped<IPlayerDataBroker, PlayerDataBroker>();
builder.Services.AddScoped<IPlayerDataProvider, PlayerDataProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
