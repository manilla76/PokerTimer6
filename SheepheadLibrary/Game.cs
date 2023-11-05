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
        private int firstTrickPlayer = -1;
        private int dealer = 0;
        private Deck deck;
        public int ActivePlayer;
        public int LastWinner = -1;
        public List<int> TeamQueen = new List<int>();
        public List<int> TeamNoQueen = new List<int>();
        private bool teamsAreKnown = false;
        private bool bothQueensPlayed = false;

        public Game(Deck deck) 
        {
            PlayerList.Add(new Player(0));  // player 0
            PlayerList.Add(new Player(1));  // player 1
            PlayerList.Add(new Player(2));  // player 2
            PlayerList.Add(new Player(3));  // player 3
            this.deck = deck;
        }

        public void ShuffleCards()
        {
            deck.ShuffleDeck();
        }
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
            TeamNoQueen.Clear();
            TeamQueen.Clear();
            firstTrickPlayer = -1;
            teamsAreKnown = false;
            bothQueensPlayed = false;
        }

        public bool PlayCard(int cardNumber)
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

        public int ProcessTrick()  // return trick number
        {
            int winner = CurrentTrick.IdentifyWinner();
            PlayerList[winner].AddToWinningTricks(TrickList.Count);
            if (teamsAreKnown == false)
            {
                LookForQueenInCurrentTrick(winner);
            }
            ActivePlayer = winner;
            LastWinner = ActivePlayer;
            TrickList.Add(CurrentTrick);
            return TrickList.Count;
        }

        private void LookForQueenInCurrentTrick(int winner)
        {
            var queenPlayers = CurrentTrick.TrickCards
                .Where(c => c.SortStrength == 32 || c.SortStrength == 31)
                .Select(p => p.PlayedByPlayer).ToList();
            if (queenPlayers.Count == 0)        // No queens check to see if first trick is set
            {
                if (firstTrickPlayer == -1)
                {
                    firstTrickPlayer = winner;
                    if (bothQueensPlayed)
                    {
                        TeamQueen.Add(winner);
                        SetNoQueenTeam();
                    }
                }
            }
            if (queenPlayers.Count() > 1)       // Both queens played in single trick
            {
                TeamQueen.Add(queenPlayers[0]);
                TeamQueen.Add(queenPlayers[1]);
                SetNoQueenTeam();
            }
            if (queenPlayers.Count() > 0)       // At least one queen played, see if this is the first or 2nd queen.  If 2nd, see if both are same person.  If so, see if first trick set
            {
                if (TeamQueen.Count == 0)
                {
                    TeamQueen.Add(queenPlayers[0]);
                    return;
                }
                if (TeamQueen[0] == queenPlayers[0])
                {
                    bothQueensPlayed = true;
                    if (firstTrickPlayer > -1)
                    {
                        TeamQueen.Add(firstTrickPlayer);
                        SetNoQueenTeam();
                    }
                }
                else
                {
                    TeamQueen.Add(winner);
                    SetNoQueenTeam();
                }
            }
            
            if (TeamQueen.Count == 2)           // Team Queen complete, set the other team
            {
                
                SetNoQueenTeam();       
            }    

        }

        private void SetNoQueenTeam()
        {
            teamsAreKnown = true;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (!TeamQueen.Contains(i))
                {
                    TeamNoQueen.Add(i);
                }
            }
        }

        public void FinishHand()
        {
            int queensScore = 0, nonQueensScore = 0;
            foreach (var player in TeamQueen)
            {
                foreach (var trick in PlayerList[player].TricksWon)
                {
                    foreach(var card in TrickList[trick].TrickCards)
                    {
                        queensScore += card.Points;
                    }
                }
            }
            foreach (var player in TeamNoQueen)
            {
                foreach (var trick in PlayerList[player].TricksWon)
                {
                    foreach (var card in TrickList[trick].TrickCards)
                    {
                        nonQueensScore += card.Points;
                    }
                }
            }
            if (queensScore == 120)
            {
                PointsToAdd(TeamQueen, 4);
            }
            if (queensScore > 90)
            {
                PointsToAdd(TeamQueen, 2);
            }
            if (queensScore > 60)
            {
                PointsToAdd(TeamQueen, 1);
            }
            if (nonQueensScore >= 90)
            {
                PointsToAdd(TeamNoQueen, 3);
            }
            if (nonQueensScore >= 60)
            {
                PointsToAdd(TeamNoQueen, 2);
            }
        }

        private void PointsToAdd(List<int> team, int points)
        {
            foreach (var player in team)
            {
                PlayerList[player].Score += points;
            }
        }
    }
}
