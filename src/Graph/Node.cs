using System.Collections.Generic;

namespace Graph;

public record Node<TNode, TEdge> : INode<TNode, TEdge>
{
    internal Node(TNode value, IList<IEdge<TNode, TEdge>> edges) 
    {
        Value = value;
        Edges = edges;
    }

    public TNode Value { get; internal set; }

    public IList<IEdge<TNode, TEdge>> Edges { get; internal set; }

    TNode INode<TNode, TEdge>.Value => Value;

    IReadOnlyCollection<IEdge<TNode, TEdge>> INode<TNode, TEdge>.Edges => (IReadOnlyCollection<IEdge<TNode, TEdge>>)Edges;
}
