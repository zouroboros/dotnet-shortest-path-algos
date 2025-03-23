using Algorithm.Model;
using Graph;

namespace Algorithm.Algorithm.Ssmtspp;

/// <summary>
/// 
/// </summary>
/// <param name="ArrivalTime">When the oath arrived at the current node</param>
/// <param name="OtherObjectives">The value of the other objectives.</param>
/// <param name="Edge">The last edge in the path.</param>
/// <param name="Node">The current node.</param>
/// <param name="PreviousLabel">The label from which this label was created.</param>
/// <typeparam name="TOtherObjectives">Type of the other objectives.</typeparam>
/// <typeparam name="TNode">The node value type.</typeparam>
/// <typeparam name="TEdge">The edge type.</typeparam>
public record NodeLabel<TOtherObjectives, TNode, TEdge>(
    DateTime ArrivalTime,
    TOtherObjectives OtherObjectives,
    int LabelNumber,
    IEdge<TNode, TEdge>? Edge,
    INode<TNode, TEdge> Node,
    NodeLabel<TOtherObjectives, TNode, TEdge>? PreviousLabel)
    : IComparable<NodeLabel<TOtherObjectives, TNode, TEdge>>
    where TOtherObjectives : IComparable<TOtherObjectives>, IObjective<TOtherObjectives, TNode, TEdge>, IEquatable<TOtherObjectives>
    where TEdge : ITimedEdge, IEquatable<TEdge>
    where TNode : IEquatable<TNode>
{
    public int CompareTo(NodeLabel<TOtherObjectives, TNode, TEdge>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var arrivalTimeComparison = ArrivalTime.CompareTo(other.ArrivalTime);
        if (arrivalTimeComparison != 0) return arrivalTimeComparison;
        var otherObjectivesComparison = OtherObjectives.CompareTo(other.OtherObjectives);
        if (otherObjectivesComparison != 0) return otherObjectivesComparison;
        var labelNumberComparison = LabelNumber.CompareTo(other.LabelNumber);
        if (labelNumberComparison != 0) return labelNumberComparison;
        return 0;
    }

    public bool WeaklyDominates(NodeLabel<TOtherObjectives, TNode, TEdge> other)
    {
       
        if (ArrivalTime <= other.ArrivalTime)
        {
            return OtherObjectives.WeaklyDominates(other.OtherObjectives);
        }

        return false;
    }

    public bool Dominates(NodeLabel<TOtherObjectives, TNode, TEdge> other)
    {
        if (ArrivalTime < other.ArrivalTime)
        {
            return OtherObjectives.Dominates(other.OtherObjectives);
        }

        return false;
    }

    public NodeLabel<TOtherObjectives, TNode, TEdge> Add(IEdge<TNode, TEdge> edge, int labelNumber) =>
        new(edge.Value.ArrivalTime, OtherObjectives.Add(edge), labelNumber, edge,
            edge.NodeB, this);
}