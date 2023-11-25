using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepheadLibrary
{
    public class Trick
    {
        public Card[] TrickCards = new Card[4];

        public CardSuit FirstSuit = CardSuit.None;
        public int CardsPlayed { get => TrickCards.Count(c => c is not null); }  

        public int IdentifyWinner()
        {
            if (CardsPlayed < 4) return -1;  // Only allowed to win trick if all 4 cards played.
            // if any trump played, pick the highest
            // else pick the highest of first suit
            if (TrickCards.FirstOrDefault(c=>c.IsTrump) != null)
                return TrickCards.OrderByDescending(c => c.SortStrength).First().PlayedByPlayer; // Get winning card, and return player.
            return TrickCards.Where(c => c.Suit == FirstSuit).OrderByDescending(c => c.SortStrength).First().PlayedByPlayer;
        }
        public void ReceiveCard(Card card) 
        {
            if (CardsPlayed == 0)
            {
                FirstSuit = card.IsTrump ? CardSuit.Trump : card.Suit;  // Set suit for others to follow
            }
            TrickCards[CardsPlayed] = card;
        }

        public bool IsLegalPlay(Card card, Hand hand)  // True if card played from this hand is legal
        {
            var validCards = GetLegalCardsToPlay(hand);
            return validCards.Contains(card);
        }

        public List<Card> GetLegalCardsToPlay(Hand hand) // Identifies all legal card plays for the given hand.
        {
            if (FirstSuit == CardSuit.None) return hand.Cards;
            if (FirstSuit == CardSuit.Trump) return hand.Cards.FindAll(c => c.IsTrump).Count == 0 ? hand.Cards : hand.Cards.FindAll((c) => c.IsTrump == true );
            return hand.Cards.FindAll(c => c.Suit == FirstSuit && c.IsTrump == false).Count == 0 ? hand.Cards : hand.Cards.FindAll((c) => c.Suit == FirstSuit && c.IsTrump == false);
        }

        public string ShowTrick()
        {
            
            string output = "";
            string separator = "";
            foreach (Card c in TrickCards)
            {
                if (c == null) return output == "" ? string.Empty : output;
                output += separator + c.Abbreviation;
                separator = ", ";
            }
            output += ": " + FirstSuit.ToString();
            return output;
        }
    }
}
