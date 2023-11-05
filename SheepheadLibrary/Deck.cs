using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepheadLibrary
{
    public class Deck
    {
        public List<Card> FullDeck = new List<Card>();
        public Deck()
        {
            InitiaizeDeck();
        }
        private int nextCard;

        private void InitiaizeDeck()
        {
            AddTrumps();
            AddSuits();
            SetPoints();
            nextCard = 0;
        }

        private void SetPoints()
        {
            foreach (Card card in FullDeck)
            {
                switch (card.Weight)
                {
                    case CardWeight.Ace:
                        card.Points = 11;
                        break;
                    case CardWeight.King:
                        card.Points = 4;
                        break;
                    case CardWeight.Queen:
                        card.Points = 3;
                        break;
                    case CardWeight.Jack:
                        card.Points = 2;
                        break;
                    case CardWeight.Ten:
                        card.Points = 10;
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddSuits()
        {
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Ace, 6, 18));
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Ten, 5, 17));
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.King, 4, 16));
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Nine, 3, 15));
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Eight, 2, 14));
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Seven, 1, 13));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Ace, 6, 12));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Ten, 5, 11));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.King, 4, 10));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Nine, 3, 9));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Eight, 2, 8));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Seven, 1, 7));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Ace, 6, 6));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Ten, 5, 5));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.King, 4, 4));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Nine, 3, 3));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Eight, 2, 2));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Seven, 1, 1));
        }

        private void AddTrumps()
        {
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Queen, 14, 32, true));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Queen, 13, 31, true));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Queen, 12, 30, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Queen, 11, 29, true));
            FullDeck.Add(new Card(CardSuit.Club, CardWeight.Jack, 10, 28, true));
            FullDeck.Add(new Card(CardSuit.Spade, CardWeight.Jack, 9, 27, true));
            FullDeck.Add(new Card(CardSuit.Heart, CardWeight.Jack, 8, 26, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Jack, 7, 25, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Ace, 6, 24, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Ten, 5, 23, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.King, 4, 22, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Nine, 3, 21, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Eight, 2, 20, true));
            FullDeck.Add(new Card(CardSuit.Diamond, CardWeight.Seven, 1, 19, true));
        }

        public void ShuffleDeck()
        {
            Random r = new Random();
            for (int n = FullDeck.Count - 1; n > 0; --n)
            {
                int k = r.Next(n+1);
                Card temp = FullDeck[n];
                FullDeck[n] = FullDeck[k];
                FullDeck[k] = temp;
            }
        }

        public Card DealCard()
        {
            Card output = FullDeck[nextCard];  // get next card
            nextCard = (nextCard + 1) % FullDeck.Count;  // increment next card or return to 0 if last card dealt
            return output;                      // return next card
        }
    }
}
