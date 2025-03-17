using GTFS;
using GTFS.Entities;

namespace GtfsPlanner;

public class TripReader(GTFSFeed feed)
{
    public IEnumerable<Trip> ReadTrips(DateOnly date)
    {
        var serviceIds = feed.Calendars.Where(calendar => RunsOn(date, calendar))
            .Select(calender => calender.ServiceId);
        return feed.Trips.Where(trip => serviceIds.Contains(trip.ServiceId))
            .Join(feed.StopTimes, trip => trip.Id, time => time.TripId, (trip, stopTime) => (trip, stopTime))
            .Join(feed.Stops, tripAndStopTime => tripAndStopTime.stopTime.StopId, stop => stop.Id,
                (tripAndStopTime, stop) => (tripAndStopTime.trip, tripAndStopTime.stopTime, stop))
            .GroupJoin(feed.Stops, tripAndStop => tripAndStop.stop.ParentStation, stop => stop.Id,
                (tripAndStop, parentStops) => (tripAndStop.trip, tripAndStop.stopTime, tripAndStop.stop,
                    parentStop: parentStops.DefaultIfEmpty().First()))
            .GroupJoin(feed.Stops, tripAndStop => tripAndStop.parentStop?.ParentStation, stop => stop.Id,
                (tripAndStop, parentParentStops) => (tripAndStop.trip, tripAndStop.stopTime, tripAndStop.stop,
                    tripAndStop.parentStop, parentParentStop: parentParentStops.DefaultIfEmpty().First()))
            .GroupBy(tripAndStop => tripAndStop.trip,
                (trip, tripAndStops) => new Trip(trip.ShortName,
                    tripAndStops.Select(tripAndStop =>
                            new Stop(
                                tripAndStop.parentParentStop?.Id ?? tripAndStop.parentStop?.Id ?? tripAndStop.stop.Id,
                                tripAndStop.parentParentStop?.Name ??
                                tripAndStop.parentStop?.Name ?? tripAndStop.stop.Name,
                                ToDateTime(date, tripAndStop.stopTime.ArrivalTime!.Value),
                                ToDateTime(date, tripAndStop.stopTime.DepartureTime!.Value)))
                        .ToList()));
    }

    private DateTime ToDateTime(DateOnly date, TimeOfDay timeOfDay) =>
        date.ToDateTime(new TimeOnly(0, 0)).AddSeconds(timeOfDay.TotalSeconds);
    
    private static bool RunsOn(DateOnly date, Calendar calendar)
    {
        return DateOnly.FromDateTime(calendar.StartDate) <= date && DateOnly.FromDateTime(calendar.EndDate) >= date &&
               RunsOn(date.DayOfWeek, calendar);
    }

    private static bool RunsOn(DayOfWeek dayOfWeek, Calendar calendar)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => calendar.Sunday,
            DayOfWeek.Monday => calendar.Monday,
            DayOfWeek.Tuesday => calendar.Tuesday,
            DayOfWeek.Wednesday => calendar.Wednesday,
            DayOfWeek.Thursday => calendar.Thursday,
            DayOfWeek.Friday => calendar.Friday,
            DayOfWeek.Saturday => calendar.Saturday,
        };
    }
}