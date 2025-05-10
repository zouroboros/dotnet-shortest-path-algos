namespace Graph.Mutable;

public record MutableEdge<TNode, TEdge>(MutableNode<TNode, TEdge> NodeA, MutableNode<TNode, TEdge> NodeB) : IEdge<TNode, TEdge>
{
    public required TEdge Value { get; set; }
    INode<TNode, TEdge> IEdge<TNode, TEdge>.NodeA => NodeA;
    INode<TNode, TEdge> IEdge<TNode, TEdge>.NodeB => NodeB;
}