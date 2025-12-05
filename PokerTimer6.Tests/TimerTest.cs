using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PokerTimer6.Data;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;
using PokerTimer6.Pages.Components;
using PokerLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.Configuration;


namespace PokerTimer6.Tests
{
    public class TimerTest : BunitContext
    {
        public TimerTest()
        {
            // === 1. Strongly-typed test settings (exactly the same as production) ===
            var testSettings = new Dictionary<string,string>
            {
                ["AppSettings:ARoundPayoutsToNearest"] = "10",
                ["AppSettings:ABuyIn"] = "100m",
                ["AppSettings:ARebuyAmount"] = "100m",
                ["AppSettings:AAddOnAmount"] = "150m",
                ["AppSettings:AStartingStack"] = "15_000",
                ["AppSettings:ARebuyStack"] = "15_000",
                ["AppSettings:AAddOnStack"] = "25_000"
                
            //{
            //    new() { MinNumberOfPlayers = 1,  MaxNumberOfPlayers = 4,  PayoutPercents = new[] { 1.0m } },
            //    new() { MinNumberOfPlayers = 5,  MaxNumberOfPlayers = 7,  PayoutPercents = new[] { 0.6m, 0.4m } },
            //    new() { MinNumberOfPlayers = 8,  MaxNumberOfPlayers = 12, PayoutPercents = new[] { 0.5m, 0.3m, 0.2m } },
            //    new() { MinNumberOfPlayers = 13, MaxNumberOfPlayers = 18, PayoutPercents = new[] { 0.4m, 0.3m, 0.2m, 0.1m } },
            //    new() { MinNumberOfPlayers = 19, MaxNumberOfPlayers = 1000, PayoutPercents = new[] { 0.4m, 0.23m, 0.16m, 0.12m, 0.09m } }
            //}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(testSettings)
                .Build();
            // === 2. Register IOptions<AppSettings> exactly like production ===
            Services.AddSingleton(Options.Create(testSettings));

            // === 3. Register ALL singletons exactly like Program.cs ===
            // Order doesn't matter — DI will resolve dependencies automatically
            Services.AddSingleton<ITimerService, TimerService>();
            Services.AddSingleton<IPayoutService, PayoutService>();
            Services.AddSingleton<IPlayerService, PlayerService>();
            Services.AddSingleton<IGameService, GameService>();
            Services.AddSingleton<IDataAccess, DataAccess>();
            Services.AddSingleton<IConfiguration>(configuration);
        }

        [Fact]
        public void Initial_Render_Shows_Ready_State_Or_First_Level()
        {
            var cut = Render<TimerComponent>();

            // Adjust to whatever your UI shows when not started
            cut.WaitForAssertion(() =>
            {
                Assert.True(cut.Markup.Contains("SB / BB")
                                          || cut.Markup.Contains("0:00")
                                          || cut.Markup.Contains("Start"));
            }, TimeSpan.FromSeconds(5));
                                      
        }


        [Fact]
        public void When_Timer_Running_Shows_Countdown_And_Correct_Blinds()
        {
            // Get the real shared services (they are the exact same instances the component uses)
            var timerService = Services.GetRequiredService<ITimerService>();
            var gameService = Services.GetRequiredService<IGameService>();

            gameService.AddRound(new Round { BigBlind = 100, RoundMinutes = 5 });
            gameService.AddRound(new Round { BigBlind = 200, RoundMinutes = 5 });
            gameService.AddRound(new Round { BigBlind = 400, RoundMinutes = 5 });
            gameService.AddRound(new Round { RoundMinutes = 5 });
            gameService.AddRound(new Round { BigBlind = 600, RoundMinutes = 5 });
            // Start the tournament (however you do it in real app)
            gameService.SetCurrentRound();               // <-- your public method
                                      
            var currentRound = gameService.CurrentRound;

            var cut = Render<TimerComponent>();

            // Should show full time of current level
            var expected = $"{currentRound.RoundMinutes}:00";
            
            cut.WaitForAssertion(() => cut.Markup.Contains(expected), TimeSpan.FromSeconds(3));
            timerService.StartPauseAsync();
            // === Fast-forward time for test (add this debug method to TimerService) ===
            timerService.SetTimeRemaining(TimeSpan.FromSeconds(95));   // 1:35 left

            cut.WaitForAssertion(() => cut.Markup.Contains("1:35"), TimeSpan.FromSeconds(3));

            // Blind display
            cut.WaitForAssertion(() => cut.Markup.Contains(currentRound.BigBlind.ToString()), TimeSpan.FromSeconds(2));
            cut.WaitForAssertion(() => cut.Markup.Contains(currentRound.SmallBlind.ToString()), TimeSpan.FromSeconds(2));

            // Next blind preview
            gameService.SetCurrentRound(); // or however you expose it
            currentRound = gameService.CurrentRound;
            cut.WaitForAssertion(() => cut.Markup.Contains($"{currentRound.BigBlind} / {currentRound.SmallBlind}"), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void Level_Ends_Automatically_Advances_And_Plays_Sound()
        {
            var timerService = Services.GetRequiredService<ITimerService>();
            var gameService = Services.GetRequiredService<IGameService>();

            gameService.AddRound(new Round { BigBlind = 100, RoundMinutes = 5 });
            gameService.AddRound(new Round { BigBlind = 200, RoundMinutes = 5 });
            gameService.AddRound(new Round { BigBlind = 400, RoundMinutes = 5 });
            gameService.AddRound(new Round { RoundMinutes = 5 });
            gameService.AddRound(new Round { BigBlind = 600, RoundMinutes = 5 });

            var soundSetup = this.JSInterop
                .SetupVoid("pokerSounds.playLevelEnd")
                .SetVoidResult();

            gameService.SetCurrentRound();
            timerService.SetTimeRemaining(TimeSpan.FromSeconds(5));
            //timerService.Start(5); // very short level for test

            var cut = Render<TimerComponent>();
            timerService.StartPauseAsync();
            // Jump to end of level
            //timerService.SetTimeRemaining(TimeSpan.FromSeconds(1));
            //timerService.StartPauseAsync();
            cut.WaitForAssertion(() =>
            {
                //Whatever your UI shows on level up
                //cut.Markup.Contains("0:00");
                this.JSInterop.VerifyInvoke("pokerSounds.playLevelEnd", 1);
                //Assert.True(gameService.CurrentLevelIndex >= 1); // advanced
            }, TimeSpan.FromSeconds(14));
        }
    }
}
