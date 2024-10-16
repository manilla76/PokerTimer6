using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PokerTimer8.Client.Data;

namespace PokerTimer8.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddSingleton<ITimerService, TimerService>();
            //builder.Services.AddScoped<InputDataList>();
            await builder.Build().RunAsync();
        }
    }
}
