namespace GtfsPlanner;

public static class DateTimes
{
    public static IReadOnlySet<DayOfWeek> DaysOfWeekBetween(DateTime from, DateTime to)
    {
        var day = from.Date;

        var dayOfWeeks = new HashSet<DayOfWeek>();
        
        while (day <= to)
        {
            dayOfWeeks.Add(day.DayOfWeek);
            day = day.AddDays(1);
        }

        return dayOfWeeks;
    } 
}