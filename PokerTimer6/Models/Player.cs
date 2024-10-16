using System.ComponentModel.DataAnnotations;

namespace PokerTimer6.Models
{
    public class Player : IComparable<Player>, IEquatable<Player>
    {
        public uint id { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public Seat Player_Seat { get; private set; } = new Seat();
        public int Table { get; set; } = 0;
        public int Seat { get; set; } = 0;

        public Player()
        {

        }
        public Player(uint ID, string name)
        {
            id = ID;
            Name = name;
            Player_Seat.TableNumber = (uint)Table;
            Player_Seat.SeatNumber = (uint)Seat;
        }

        public void AssignSeat(Seat seat)
        {
            Player_Seat = seat;
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(Player? other)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return -1;
            }
            return Name.CompareTo(other.Name);
        }

        public bool Equals(Player? other)
        {
            return Name == other.Name;
        }
    }
}
