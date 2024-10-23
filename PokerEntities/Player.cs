using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerEntities
{
    public class Player : IComparable<Player>
    {
        public string Name { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public uint Table { get; set; }
        public uint Seat { get; set; }
        public bool IsActive { get; set; }

        public int CompareTo(Player? other) => string.Compare(Name, other!.Name, StringComparison.OrdinalIgnoreCase);        
    }
}
