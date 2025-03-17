using System.Collections;
using Graph;

namespace Algorithm;

public record TimeSpanObjective<TNode, TEdge>(TimeSpan TimeSpan)
    : IComparable<TimeSpanObjective<TNode, TEdge>>, IObjective<TimeSpanObjective<TNode, TEdge>, TNode, TEdge>
    where TEdge : ITimedEdge
{
    public int CompareTo(TimeSpanObjective<TNode, TEdge>? other) =>
        Comparer.DefaultInvariant.Compare(TimeSpan, other?.TimeSpan);

    public bool WeaklyDominates(TimeSpanObjective<TNode, TEdge> other) => TimeSpan <= other.TimeSpan;

    public TimeSpanObjective<TNode, TEdge> Add(IEdge<TNode, TEdge> edge) =>
        new(TimeSpan + (edge.Value.ArrivalTime - edge.Value.DepartureTime));
}