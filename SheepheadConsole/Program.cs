using SheepheadLibrary;

namespace SheepheadConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<ConsoleKey> keys = new List<ConsoleKey>();
            keys.Add(ConsoleKey.D1);
            keys.Add(ConsoleKey.D2);
            keys.Add(ConsoleKey.D3);    
            keys.Add(ConsoleKey.D4);
            keys.Add(ConsoleKey.D5);
            keys.Add(ConsoleKey.D6);
            keys.Add(ConsoleKey.D7);
            keys.Add(ConsoleKey.D8);

            Game game = new Game(new Deck());
            game.ShuffleCards();
            game.DealCards();
            bool ContinueGame = true;
            do
            {
                for (int i = 0; i < game.PlayerList.Count; i++)
                {
                    Console.WriteLine($"Trick# {game.TrickList.Count + 1}, Card# {game.CurrentTrick.cardsPlayed}, Last Winner: {game.LastWinner}");
                    Console.WriteLine("Press key to play card");
                    game.PlayerList[game.ActivePlayer].Hand.OrderHand();
                    Console.WriteLine($"Player {game.ActivePlayer}: {game.PlayerList[game.ActivePlayer].Hand.ShowHandString()}");
                    game.CurrentTrick.GetLegalCardsToPlay(game.PlayerList[game.ActivePlayer].Hand).ForEach(x => Console.WriteLine(x.Abbreviation));
                    ConsoleKeyInfo cki;
                    bool CardPlayed = false;
                    cki = CheckForCardPlay(keys, game, ref CardPlayed);
                    CardPlayed = false;
                    Console.Clear();
                }
                // Trick Complete, process...
                int numberOfTricks = game.ProcessTrick();
                
                if (numberOfTricks == 8)
                    ContinueGame = false;
                else
                    game.CurrentTrick = new Trick();
            } while (ContinueGame);
            game.FinishHand();
            // Process score, return to the top
        }

        private static ConsoleKeyInfo CheckForCardPlay(List<ConsoleKey> keys, Game game, ref bool CardPlayed)
        {
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                switch (cki.Key)
                {
                    case ConsoleKey.D1:
                        if (game.PlayCard(0))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D2:
                        if (game.TrickList.Count > 6) break;
                        if (game.PlayCard(1))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D3:
                        if (game.TrickList.Count > 5) break;
                        if (game.PlayCard(2))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D4:
                        if (game.TrickList.Count > 4) break;
                        if (game.PlayCard(3))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D5:
                        if (game.TrickList.Count > 3) break;
                        if (game.PlayCard(4))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D6:
                        if (game.TrickList.Count > 2) break;
                        if (game.PlayCard(5))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D7:
                        if (game.TrickList.Count > 1) break;
                        if (game.PlayCard(6))
                            CardPlayed = true;
                        break;
                    case ConsoleKey.D8:
                        if (game.TrickList.Count > 0) break;
                        if (game.PlayCard(7))
                            CardPlayed = true;
                        break;
                }

            } while (!keys.Contains(cki.Key) || CardPlayed == false); // Card Play
            return cki;
        }
    }
}