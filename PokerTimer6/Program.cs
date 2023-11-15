using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PokerLibrary;
using PokerLibrary.Data;
using PokerLibrary.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddTransient<IDataAccess, DataAccess>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IPayoutService, PayoutService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddScoped<IPlayerDataBroker, PlayerDataBroker>();
builder.Services.AddScoped<IPlayerDataProvider, PlayerDataProvider>();
builder.Services.AddTransient<PlayerPresenter>();
//if (!builder.Services.Any(x => x.ServiceType == typeof(HttpClient)))
//{
//    builder.Services.AddScoped<HttpClient>(s =>
//    {
//        var uriHelper = s.GetRequiredService<NavigationManager>();
//        return new HttpClient { BaseAddress = new Uri(uriHelper.BaseUri) };
//    });
//}
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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
