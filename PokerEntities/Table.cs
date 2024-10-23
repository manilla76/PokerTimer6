using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerEntities
{
    public class Table
    {
        public int TableId { get; set; }
        public List<Player>? Players { get; set; }
        public uint Dealer { get; set; }
    }
}
