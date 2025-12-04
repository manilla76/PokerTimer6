using Microsoft.Extensions.Options;
using PokerTimer6.Data;
using PokerTimer6.Models;

namespace PokerTimer6.Tests
{
    public class PayoutServiceTests
    {
        private readonly AppSettings _settings = new()
        {
            RoundPayoutsToNearest = 10,
            BuyIn = 100m,
            Payouts = new []
            {
                new PayoutBracket
                {
                    MinNumberOfPlayers = 1,
                    MaxNumberOfPlayers = 4,
                    PayoutPercents = new List<decimal> { 1.0m }
                },
                new PayoutBracket
                {
                    MinNumberOfPlayers = 5,
                    MaxNumberOfPlayers = 7,
                    PayoutPercents = new List<decimal> { 0.6m, 0.4m }
                },
                new PayoutBracket
                {
                    MinNumberOfPlayers = 8,
                    MaxNumberOfPlayers = 12,
                    PayoutPercents = new List<decimal> { 0.5m, 0.3m, 0.2m }
                },
                new PayoutBracket{
                    MinNumberOfPlayers = 13,
                    MaxNumberOfPlayers = 18,
                    PayoutPercents = new List<decimal> { 0.4m, 0.3m, 0.2m, 0.1m }
                },
                new PayoutBracket
                {
                    MinNumberOfPlayers = 19,
                    MaxNumberOfPlayers = 1000,
                    PayoutPercents = new List<decimal> { 0.4m, 0.23m, 0.16m, 0.12m, 0.09m }
                }
            }
        };

        private PayoutService CreateService(uint playerCount = 10, decimal prizePoolOverride = 0m)
        {
            var options = Options.Create(_settings);
            var service = new PayoutService(options);
            service.SetActivePayout(playerCount);

            if (prizePoolOverride > 0)
            {
                
            }
            
            return service;
        }

        [Theory]
        [InlineData(3, 300, new[] { 300 })]
        [InlineData(6, 600, new[] { 360, 240 })]
        [InlineData(10, 1000, new[] { 500, 300, 200 })]
        [InlineData(15, 1500, new[] { 600, 450, 300, 150 })]
        [InlineData(25, 2500, new[] { 1000, 580, 400, 300, 220 })]
        public void CalculatePayout_Returns_Correct_Amounts(int playerCount, uint prizePool, int[] expected)
        {
            // Arrange
            var service = CreateService((uint)playerCount);
            typeof(PayoutService).GetProperty("PrizeMoney")?.SetValue(service, prizePool);
            // Act
            service.SetActivePayout((uint)playerCount);
            service.CalculatePayout();
            var actualPayouts = service.ActivePayout.Payouts.ToArray();
            // Assert
            Assert.Equal(expected.Length, actualPayouts.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actualPayouts[i]);
            }
        }

        [Fact]
        public void RoundPayoutsToNearest_Is_Applied_Correctly()
        {
            // Arrange
            var service = CreateService(6, 637m);
            typeof(PayoutService).GetProperty("PrizeMoney")?.SetValue(service, 637u);
            // Act
            service.CalculatePayout();
            var actualPayouts = service.ActivePayout.Payouts.ToArray();
            // Assert
            Assert.Equal(380m, actualPayouts[0]);
            Assert.Equal(257m, actualPayouts[1]);
        }
    }
}
