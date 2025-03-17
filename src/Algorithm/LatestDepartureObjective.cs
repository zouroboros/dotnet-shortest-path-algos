using Graph;

namespace Algorithm;

public readonly record struct LatestDepartureObjective<TNode, TEdge>(DateTime? Departure)
    : IObjective<LatestDepartureObjective<TNode, TEdge>, TNode, TEdge>,
        IComparable<LatestDepartureObjective<TNode, TEdge>>
    where TEdge : ITimedEdge
{
    public bool WeaklyDominates(LatestDepartureObjective<TNode, TEdge> other) => Departure >= other.Departure;

    public LatestDepartureObjective<TNode, TEdge> Add(IEdge<TNode, TEdge> edge)
    {
        if (Departure is null)
        {
            return new LatestDepartureObjective<TNode, TEdge>(edge.Value.DepartureTime);
        }

        return this;
    }

    public int CompareTo(LatestDepartureObjective<TNode, TEdge> other) => Nullable.Compare(Departure, other.Departure);
}