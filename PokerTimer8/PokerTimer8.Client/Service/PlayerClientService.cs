using PokerEntities;
using System.Net.Http.Json;

namespace PokerTimer8.Client.Service
{
    public class PlayerClientService : IPlayerService
    {
        private readonly HttpClient http;

        public PlayerClientService(HttpClient httpClient)
        {
            http = httpClient;
        }

        public Task AddPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public async Task AddPlayer(string playerName)
        {
            await http.PostAsync($"/Players/{playerName}", new StringContent( );
        }

        public async Task<Player?> GetPlayer(int playerId)
        {
            try
            {
                return await http.GetFromJsonAsync<Player>($"/players/{playerId}/");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<Player?> GetPlayer(string playerName)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Player>?> GetPlayers()
        {
            return await http.GetFromJsonAsync<IEnumerable<Player>?>("/players");
        }

        public Task ModifyPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public async Task RemovePlayer(Player player)
        {
            await http.DeleteFromJsonAsync<Player>($"/players/{player}");
        }

        public async Task RemovePlayer(int playerId)
        {
            await http.DeleteAsync($"/players/{playerId}");
        }
    }
}
