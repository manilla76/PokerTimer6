using PokerEntities;
using PokerServices.Interfaces;
using System.Net.NetworkInformation;

namespace PokerApi
{
    public static class Api
    {
        public static void ConfigureApi(this WebApplication app) {

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.MapGet("/Players", GetPlayers);
            app.MapGet("/Players/{id}", GetPlayer);
            app.MapPost("/Players", AddPlayer);
            app.MapPut("/Players", UpdatePlayer);
            app.MapDelete("/Players", RemovePlayer);
        }

        private static async Task<IResult> GetPlayers(IPlayerService playerService)
        {
            try
            {
                return Results.Ok(await playerService.GetPlayersAsync());
            }
            catch (Exception ex)
            {

                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetPlayer(int id, IPlayerService playerService)
        {
            try
            {
                var results = await playerService.GetPlayerByIdAsync(id);
                if (results is null) return Results.NotFound(id);
                return Results.Ok(results);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
                
            }
        }

        private static async Task<IResult> AddPlayer(Player player, IPlayerService playerService)
        {
            try
            {
                await playerService.AddPlayerAsync(player);
                return Results.Ok(player);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> UpdatePlayer(Player player, IPlayerService playerService)
        {
            try
            {
                await playerService.UpdatePlayerAsync(player);
                return Results.Ok(player);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> RemovePlayer(int id, IPlayerService playerService)
        {
            try
            {
                await playerService.RemovePlayerAsync(id);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}
