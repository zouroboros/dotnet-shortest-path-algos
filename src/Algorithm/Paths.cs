using Graph;

namespace Algorithm;

public static class Paths
{
    public static IEnumerable<INode<TNode, TEdge>> EdgePathToNodePath<TNode, TEdge>(
        IReadOnlyCollection<IEdge<TNode, TEdge>> edges)
        => edges.Select(edge => edge.NodeA).Append(edges.Last().NodeB);
    
}