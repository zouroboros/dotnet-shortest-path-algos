namespace Graph;

public interface IEdge<TNode, TEdge>
{
    TEdge Value { get; }
    INode<TNode, TEdge> NodeA { get; }
    INode<TNode, TEdge> NodeB { get; }
}