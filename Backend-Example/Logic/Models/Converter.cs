namespace Logic.Models;

public static class Converter
{
    public static int ConvertWordToNumber(string word)
    {
        word += "AAAA";
        return word.Substring(0, 4).ToUpper().Sum(c => c - 'A');
    }

    public static DateTime ConvertDigitToDate(double x)
    {
        var baseDate = new DateTime(2020, 11, 1, 12, 0, 0);
        var totalIncrement = x;
        var currentDate = baseDate;
        currentDate = currentDate.AddDays(Math.Floor(totalIncrement));

        var fractionalPart = totalIncrement - Math.Floor(totalIncrement);
        var hoursIncrement = (int)(fractionalPart / 0.04166666667);
        currentDate = currentDate.AddHours(hoursIncrement);

        var remainingFraction = fractionalPart % 0.04166666667;
        var minutesIncrement = (int)(remainingFraction / 0.0006944444444);
        currentDate = currentDate.AddMinutes(minutesIncrement);

        return currentDate;
    }

    public static double ConvertDateToDigit(DateTime dateTime)
    {
        var baseDate = new DateTime(2020, 11, 1, 12, 0, 0);

        var timeDifference = dateTime - baseDate;
        var totalDays = timeDifference.TotalDays;
        return Math.Round(totalDays, 15);
    }
}
