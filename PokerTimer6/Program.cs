using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PokerLibrary;
using PokerTimer6;
using PokerTimer6.Data;
using PokerTimer6.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
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
    //app.UseHsts();
}


//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");
app.UseAntiforgery();
app.Run();
