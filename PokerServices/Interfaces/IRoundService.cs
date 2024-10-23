using PokerEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices.Interfaces
{
    public interface IRoundService
    {
        Task AddRoundAsync(Round round);
        Task RemoveRoundAsync(Round round);
        Task ModifyRoundAsync(Round round);
        Task<IEnumerable<Round>> GetRoundsAsync();
        Task<Round> GetRoundByIdAsync(int id);
    }
}
