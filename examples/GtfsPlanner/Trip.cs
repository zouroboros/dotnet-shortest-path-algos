namespace GtfsPlanner;

public record Trip(string Name, IReadOnlyCollection<Stop> Stops);