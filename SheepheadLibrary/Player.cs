namespace SheepheadLibrary
{
    public class Player
    {
        public string? Name { get; set; }
        public int Number { get; set; }
        public Hand Hand { get; set; }
        public int Score { get; set; }
        public List<int> TricksWon { get; set; } = new List<int>();
        public bool IsOut { get => Score >= Game.ScoreToGoOut; }
        public bool HasBothQueens { get; set; } = false;

        public Player(int number)
        {
            Hand = new Hand();
            Score = 0;
            Number = number;
        }

        public void AddToWinningTricks(int trick)
        {
            if (TricksWon == null)
                TricksWon = new List<int>();
            TricksWon.Add(trick);
        }

    }
}