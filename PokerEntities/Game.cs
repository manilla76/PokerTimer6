using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerEntities
{
    public class Game
    {
        public List<Player>? Players { get; set; }
        public List<Table>? Tables { get; set; }
        public List<Round>? Rounds { get; set; }
        public uint BuyinAmount { get; set; }
        public uint AddonAmount { get; set; }
        public uint RebuyAmount { get; set; }
    }
}
