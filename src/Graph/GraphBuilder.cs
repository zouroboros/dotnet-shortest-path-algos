using System.Collections.Generic;
using System.Linq;

namespace Graph;

public class GraphBuilder<TNode, TEdge>
{
    public GraphBuilder() { }

    public Node<TNode, TEdge> CreateNode(TNode value) => new(value, new List<IEdge<TNode, TEdge>>());

    public Edge<TNode, TEdge> CreateEdge(TEdge value, Node<TNode, TEdge> nodeA, Node<TNode, TEdge> nodeB)
    {
        var edge = new Edge<TNode, TEdge>(value, nodeA, nodeB);
        nodeA.Edges.Add(edge);
        nodeB.Edges.Add(edge);

        return edge;
    }
}

