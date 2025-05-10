using System.Collections.Generic;
using System.Linq;

namespace Graph;

public static class Nodes
{
    public static IEnumerable<IEdge<TNode, TEdge>> OutgoingEdges<TNode, TEdge>(this INode<TNode, TEdge> node) 
        => node.Edges.Where(edge => edge.NodeA == node);
    
    public static IEnumerable<IEdge<TNode, TEdge>> IncomingEdges<TNode, TEdge>(this INode<TNode, TEdge> node) 
        => node.Edges.Where(edge => edge.NodeA == node);
}