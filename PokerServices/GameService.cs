using PokerServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices
{
    public class GameService : IGameService
    {
        public uint Buyin { get; private set; } = 50;
        public uint Addon { get; private set; } = 50;
        public uint Rebuy { get; private set; } = 50;
        public void SetAddon(uint value) => Addon = value;

        public void SetBuyin(uint value) => Buyin = value;

        public void SetRebuy(uint value) => Rebuy = value;
        
        public void SetAllAmounts(uint value) => Rebuy = Addon = Buyin = value;
    }
}
