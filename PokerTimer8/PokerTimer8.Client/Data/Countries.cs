namespace PokerTimer8.Client.Data
{
    public static class Countries
    {
        public static List<KeyValuePair<int, string>> CountryList
        {
            get
            {
                List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
                var x = 1;
                foreach (var country in CountryArray)
                {
                    list.Add(new KeyValuePair<int, string>(x, country));
                    x++;
                }
                return list;
            }
        }

        public static SortedDictionary<int, string> CountryDictionary
        {
            get
            {
                SortedDictionary<int, string> list = new SortedDictionary<int, string>();
                var x = 1;
                foreach(var country in CountryArray)
                {
                    list.Add(x, country);
                    x++;
                }
                return list;
            }
        }

        public static string[] CountryArray = new string[]
        {
            "Afghanistan",
            "Albania",
            "Algeria"
        };

    }
}
