namespace Graph;

using System.Collections.Generic;

public interface INode<TNode, TEdge>
{
    TNode Value { get; }
    IReadOnlyCollection<IEdge<TNode, TEdge>> Edges { get; }
}