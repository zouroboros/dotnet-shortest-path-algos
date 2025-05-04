namespace Graph;

using System.Collections.Generic;

public record Graph<TNode, TEdge>(IReadOnlyCollection<INode<TNode, TEdge>> Nodes) : IGraph<TNode, TEdge>
{
    public IEnumerable<IEdge<TNode, TEdge>> Edges => Nodes.AllEdges();
}