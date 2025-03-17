using Algorithm;
using Graph;

namespace GtfsPlanner;

public record DurationObjective<TNode, TEdge>(TimeSpan Duration)
    : IObjective<DurationObjective<TNode, TEdge>, TNode, TEdge>, IComparable<DurationObjective<TNode, TEdge>>
    where TEdge : ITimedEdge
{
    public bool WeaklyDominates(DurationObjective<TNode, TEdge> observable) => Duration <= observable.Duration;

    public DurationObjective<TNode, TEdge> Add(IEdge<TNode, TEdge> edge) =>
        new(Duration + (edge.Value.ArrivalTime - edge.Value.DepartureTime));

    public int CompareTo(DurationObjective<TNode, TEdge>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Duration.CompareTo(other.Duration);
    }
}