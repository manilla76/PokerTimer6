using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepheadLibrary
{
    public class Game
    {
        public List<Player> PlayerList = new List<Player>();
        public List<Trick> TrickList = new List<Trick>();
        public Trick CurrentTrick = new Trick();
        private int dealer = 0;
        private Deck deck;
        private readonly Team teams;
        public int ActivePlayer;
        public int LastWinner = -1;
        public static int ScoreToGoOut = 12;

        public Game(Deck deck, Team teams) 
        {
            PlayerList.Add(new Player(0));  // player 0
            PlayerList.Add(new Player(1));  // player 1
            PlayerList.Add(new Player(2));  // player 2
            PlayerList.Add(new Player(3));  // player 3
            this.deck = deck;
            this.teams = teams;
        }
        /// <summary>
        /// shuffle cards
        /// </summary>
        public void ShuffleCards()
        {
            deck.ShuffleDeck();
        }
        /// <summary>
        /// Deal cards to all players
        /// </summary>
        public void DealCards()
        {
            for (int j = 0; j < 8; j++) //# of cards per player
            {
                for (int i = 0; i < PlayerList.Count; i++)  //1 to each player starting 1 after the dealer
                {
                    int player = (dealer + i + 1) % 4;
                    PlayerList[player].Hand.Cards.Add(deck.DealCard());
                    PlayerList[player].Hand.Cards[j].PlayedByPlayer = player;
                }
            }
            ActivePlayer = (dealer + 1) % PlayerList.Count;
        }
        /// <summary>
        /// Try to play the card given
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>true if success</returns>
        public bool TryPlayCard(int cardNumber)
        {
            if (CurrentTrick == null)
                CurrentTrick = new();
            if (CurrentTrick.IsLegalPlay(PlayerList[ActivePlayer].Hand.Cards[cardNumber], PlayerList[ActivePlayer].Hand))
            {
                CurrentTrick.ReceiveCard(PlayerList[ActivePlayer].Hand.Cards[cardNumber]);
                PlayerList[ActivePlayer].Hand.RemoveCard(cardNumber);
                IncrementPlayer();
                return true;
            }
            return false;
        }

        private void IncrementPlayer()
        {
            ActivePlayer = (ActivePlayer + 1) % PlayerList.Count;
        }
        /// <summary>
        /// Process the trick: identify winner, add trick to winning player, 
        /// set active player to know who leads next trick, 
        /// if first trick flag set and player eligible, set teams
        /// reset first trick flag and player flag.
        /// </summary>
        /// <returns>trick number</returns>
        public int ProcessTrick()  // return trick number
        {
            int winner = CurrentTrick.IdentifyWinner();
            PlayerList[winner].AddToWinningTricks(TrickList.Count);
            ActivePlayer = winner;
            LastWinner = ActivePlayer;
            if (teams.NeedsFirstTrick && PlayerList[winner].HasBothQueens == false) // looking for first trick.  Winner eligible
            {
                // set teams
                // reset player flag, reset first trick flag
                var playerWithQueens = PlayerList.First(p => p.HasBothQueens);
                teams.SetTeams(PlayerList, PlayerList[winner], playerWithQueens);  // resets firstTrick flag
                playerWithQueens.HasBothQueens = false;
            }
            TrickList.Add(CurrentTrick);
            return TrickList.Count;
        }

        private void PointsToAdd(List<int> team, int points)  // use the points service instead
        {
            foreach (var player in team)
            {
                PlayerList[player].Score += points;
            }
        }

        public void FinishHand()
        {
            var queensScore = PointService.AddPoints(teams.TeamQueen, TrickList);
            var nonQueensScore = PointService.AddPoints(teams.TeamQueen, TrickList);
            var handScore = PointService.CalculateWinner(queensScore, nonQueensScore);
            if (nonQueensScore >= queensScore)
            {
                foreach (var player in teams.TeamNonQueen)
                {
                    player.Score += handScore;
                }
            }
            else
            {
                foreach (var player in teams.TeamQueen)
                {
                    player.Score += handScore;
                }
            }
        }

        public void StartHand()
        {
            ShuffleCards();
            DealCards();
            teams.SetTeams(PlayerList, dealer);
        }

        public void PlayRound()
        {
            throw new NotImplementedException();
        }
    }
}
