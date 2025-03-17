namespace Algorithm.Test;

public static class DateTimes
{
    public static DateTime TodayAt(int hour, int minutes = 0) => DateTime.Today + TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minutes);
}