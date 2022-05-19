using System.ComponentModel.DataAnnotations;

namespace PokerTimer6.Models
{
    public class Player
    {
        public uint Player_id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public Seat Player_Seat { get; private set; }
        public int Table { get; set; }
        public int Seat { get; set; }

        public Player()
        {

        }
        public Player(uint id, string name)
        {
            Player_id = id;
            Name = name;
            Player_Seat = new Seat();
            Player_Seat.TableNumber = (uint)Table;
            Player_Seat.SeatNumber = (uint)Seat;
        }

        public void AssignSeat(Seat seat)
        {
            Player_Seat = seat;
        }

    }
}
