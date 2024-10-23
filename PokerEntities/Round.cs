using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerEntities
{
    public class Round
    {
        public int RoundId { get; set; }
        public int SB { get; set; }
        public int BB { get; set; }
        public int Time { get; set; }
        public bool CanRebuy { get; set; } = false;
        public bool CanAddon { get; set; } = false;
    }
}
