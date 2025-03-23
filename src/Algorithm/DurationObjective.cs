using System.Collections;
using Graph;

namespace Algorithm;

public record DurationObjective<TNode, TEdge>(DateTime? InitialDeparture, TimeSpan? CurrentDuration)
    : IComparable<DurationObjective<TNode, TEdge>>, IObjective<DurationObjective<TNode, TEdge>, TNode, TEdge>
    where TEdge : ITimedEdge
{
    public int CompareTo(DurationObjective<TNode, TEdge>? other) =>
        Comparer.DefaultInvariant.Compare((CurrentDuration, InitialDeparture), (other?.CurrentDuration, other?.InitialDeparture));

    public bool WeaklyDominates(DurationObjective<TNode, TEdge> other) => CurrentDuration <= other.CurrentDuration;
    public bool Dominates(DurationObjective<TNode, TEdge> other) => CurrentDuration < other.CurrentDuration;

    public DurationObjective<TNode, TEdge> Add(IEdge<TNode, TEdge> edge) => InitialDeparture == null ?
        new DurationObjective<TNode, TEdge>(edge.Value.DepartureTime, edge.Value.ArrivalTime - edge.Value.DepartureTime) :
        this with { CurrentDuration = edge.Value.ArrivalTime - InitialDeparture };
}