namespace Algorithm.Model;

/// <summary>
/// Interface for edge values that represent temporal edges.
/// </summary>
public interface ITimedEdge
{
    DateTime DepartureTime { get; }
    DateTime ArrivalTime { get; }
}