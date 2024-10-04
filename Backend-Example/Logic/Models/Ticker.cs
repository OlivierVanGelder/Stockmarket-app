namespace Backend_Example.Logic.Classes
{
    public class Ticker
    {
        public int ConvertWordToNumber(string word)
        {
            word = word + "AAAA";
            word.Substring(0, 4);
            return word.ToUpper().Sum(c => c - 'A');
        }
    }
}
