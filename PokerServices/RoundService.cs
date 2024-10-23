using PokerEntities;
using PokerServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices
{
    public class RoundService(IRoundRepository roundRepository) : IRoundService
    {
        private readonly IRoundRepository roundRepository = roundRepository;

        public Task AddRoundAsync(Round round)
        {
            throw new NotImplementedException();
        }

        public Task<Round> GetRoundByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Round>> GetRoundsAsync()
        {
            throw new NotImplementedException();
        }

        public Task ModifyRoundAsync(Round round)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoundAsync(Round round)
        {
            throw new NotImplementedException();
        }
    }
}
