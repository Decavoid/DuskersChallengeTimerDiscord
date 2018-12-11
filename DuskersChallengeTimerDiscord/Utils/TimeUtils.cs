using System;
using System.Globalization;

public static class TimeUtils
{
    private static readonly CultureInfo CultureInfoUS = CultureInfo.GetCultureInfo("en-US");

    public static int GetHoursRemainingUntilUTCMidnight()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime today = utcNow.Date;
        DateTime tomorrow = today.AddDays(1);
        TimeSpan hoursRemaining = tomorrow - utcNow;
        int hours = (int)Math.Round(hoursRemaining.TotalHours, MidpointRounding.AwayFromZero);
        return hours;
    }

    public static string GetUtcDateString()
    {
        DateTime utcDate = DateTime.UtcNow.AddMinutes(30).Date;
        string s = utcDate.ToString("MMMM d, yyyy", CultureInfoUS);
        return s;
    }
}
