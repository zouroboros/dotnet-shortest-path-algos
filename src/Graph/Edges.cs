using System.Collections.Generic;
using System.Linq;

namespace Graph;

internal static class Edges
{
    public static IEnumerable<IEdge<TNode, TEdge>> AllEdges<TNode, TEdge>(this IEnumerable<INode<TNode, TEdge>> nodes) =>
        nodes.SelectMany(node => node.Edges.Where(edge => edge.NodeA == node));
}