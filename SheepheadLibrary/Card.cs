namespace SheepheadLibrary
{
    public class Card
    {

        private CardSuit suit;
        public CardSuit Suit
        {
            get { return suit; }
            set
            {
                if (value == CardSuit.Heart) SuitAbbreviation = "H";
                if (value == CardSuit.Diamond) SuitAbbreviation = "D";

                if (value == CardSuit.Club) SuitAbbreviation = "C";

                if (value == CardSuit.Spade) SuitAbbreviation = "S"; 
                suit = value; }
        }
        private CardWeight weight;

        public CardWeight Weight
        {
            get { return weight; }
            set {
                if (value == CardWeight.Ace) WeightAbbreviation = "A";
                if (value == CardWeight.King) WeightAbbreviation = "K";
                if (value == CardWeight.Queen) WeightAbbreviation = "Q";
                if (value == CardWeight.Jack) WeightAbbreviation = "J";
                if (value == CardWeight.Ten) WeightAbbreviation = "T";
                if (value == CardWeight.Nine) WeightAbbreviation = "9";
                if (value == CardWeight.Eight) WeightAbbreviation = "8";
                if (value == CardWeight.Seven) WeightAbbreviation = "7"; 
                weight = value; }
        }
        public bool IsTrump { get; set; }
        public int Strength { get; set; }  // order of weight, highest strength wins
        public int Points { get; set; }
        private string SuitAbbreviation;
        private string WeightAbbreviation;
        public int SortStrength;
        public int PlayedByPlayer;
        public string Abbreviation { get { return WeightAbbreviation + SuitAbbreviation; } }
        public Card(CardSuit suit, CardWeight weight, int strength, int sortStrength, bool isTrump = false)
        {
            Suit = suit;
            Weight = weight;
            Strength = strength;
            SortStrength = sortStrength;
            IsTrump = isTrump;
        }
    }

    public enum CardSuit
    {
        Heart, Spade, Club, Diamond, Trump, None
    }
    public enum CardWeight
    {
        Ace, King, Queen, Jack, Ten, Nine, Eight, Seven
    }
}