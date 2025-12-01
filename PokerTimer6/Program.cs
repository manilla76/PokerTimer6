using Microsoft.AspNetCore.Components;
using PokerLibrary;
using PokerTimer6;
using PokerTimer6.Data;
using PokerTimer6.Data.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization globally
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddTransient<IDataAccess, DataAccess>();
builder.Services.AddTransient<IPokerData, PokerData>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IPayoutService, PayoutService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddScoped<ICountryDataBroker, CountryDataBroker>();
builder.Services.AddScoped<ICountryDataProvider, CountryDataProvider>();
builder.Services.AddScoped<IPlayerDataBroker, PlayerDataBroker>();
builder.Services.AddScoped<IPlayerDataProvider, PlayerDataProvider>();
//builder.Services.AddTransient<CountryPresenter>();
builder.Services.AddTransient<IndexPresenter>();
if (!builder.Services.Any(x => x.ServiceType == typeof(HttpClient)))
{
    builder.Services.AddScoped<HttpClient>(s =>
    {
        var uriHelper = s.GetRequiredService<NavigationManager>();
        return new HttpClient { BaseAddress = new Uri(uriHelper.BaseUri) };
    });
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
