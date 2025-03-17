namespace Algorithm;

public interface ITimedEdge
{
    DateTime DepartureTime { get; }
    DateTime ArrivalTime { get; }
}