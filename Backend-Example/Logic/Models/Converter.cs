namespace Backend_Example.Logic.Classes
{
    public static class Converter
    {
        public static int ConvertWordToNumber(string word)
        {
            word = word + "AAAA";
            word.Substring(0, 4);
            return word.ToUpper().Sum(c => c - 'A');
        }

        public static DateTime ConvertDigitToDate(double x)
        {
            DateTime baseDate = new DateTime(2024, 11, 1, 12, 0, 0); // Base date set to 1980-01-01T12:00:00
            double totalIncrement = x;
            DateTime currentDate = baseDate;
            currentDate = currentDate.AddDays(Math.Floor(totalIncrement));

            double fractionalPart = totalIncrement - Math.Floor(totalIncrement);
            int hoursIncrement = (int)(fractionalPart / 0.04166666667);
            currentDate = currentDate.AddHours(hoursIncrement);

            double remainingFraction = fractionalPart % 0.04166666667;
            int minutesIncrement = (int)(remainingFraction / 0.0006944444444);
            currentDate = currentDate.AddMinutes(minutesIncrement);

            return currentDate;
        }

        public static double ConvertDateToDigit(DateTime dateTime)
        {
            DateTime baseDate = new DateTime(2024, 11, 1, 12, 0, 0);

            TimeSpan timeDifference = dateTime - baseDate;
            double totalDays = timeDifference.TotalDays;
            return Math.Round(totalDays, 15);
        }
    }
}
