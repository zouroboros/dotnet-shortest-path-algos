namespace Algorithm;

public record TimedEdge<TValue>(DateTime DepartureTime, DateTime ArrivalTime, TValue Value) : ITimedEdge;