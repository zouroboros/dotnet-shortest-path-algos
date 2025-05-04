namespace Graph;

using System.Collections.Generic;

public interface IGraph<TNode, TEdge>
{
    IReadOnlyCollection<INode<TNode, TEdge>> Nodes { get; }
    
    IEnumerable<IEdge<TNode, TEdge>> Edges { get; }
}