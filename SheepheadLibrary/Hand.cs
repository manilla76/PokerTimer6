using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepheadLibrary
{
    public class Hand
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<string> ShowHandList()
        {
            return Cards.Select((c)=>c.Abbreviation).ToList();
        }
        public string ShowHandString()
        {
            string output = "";
            string separator = "";
            foreach (Card c in Cards)
            {
                output += separator + c.Abbreviation;
                separator = ", ";
            }
            return output;
        }

        public void OrderHand()
        {
            Cards = Cards.OrderByDescending(c => c.SortStrength).ToList();
        }

        internal void RemoveCard(int cardNumber)
        {
            Cards.RemoveAt(cardNumber);
        }
    }
}
