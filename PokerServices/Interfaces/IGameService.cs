using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices.Interfaces
{
    public interface IGameService
    {
        uint Buyin { get; }
        uint Addon { get; }
        uint Rebuy { get; }

        void SetBuyin(uint value);
        void SetRebuy(uint value);
        void SetAddon(uint value);
        void SetAllAmounts(uint value);
    }
}
