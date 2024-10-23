using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PokerServices;
using PokerServices.Interfaces;

namespace PokerTimer8.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            await builder.Build().RunAsync();
        }
    }
}
