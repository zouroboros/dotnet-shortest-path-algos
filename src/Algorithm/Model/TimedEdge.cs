namespace Algorithm.Model;

public record TimedEdge<TValue>(DateTime DepartureTime, DateTime ArrivalTime, TValue Value) : ITimedEdge;