namespace Graph;

public record Edge<TNode, TEdge> : IEdge<TNode, TEdge>
{

    internal Edge(TEdge value, INode<TNode, TEdge> nodeA, INode<TNode, TEdge> nodeB)
    {
        Value = value;
        NodeA = nodeA;
        NodeB = nodeB;
    }

    public TEdge Value { get; set; }

    public INode<TNode, TEdge> NodeA { get; internal set; }

    public INode<TNode, TEdge> NodeB { get; internal set; }
}
