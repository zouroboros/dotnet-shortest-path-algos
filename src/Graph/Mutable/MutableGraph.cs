using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;

namespace Graph.Mutable;

public static class MutableGraph
{
    public static MutableGraph<TNode, TNewEdge> From<TNode, TNewEdge, TEdge>(IGraph<TNode, TEdge> graph,
        Func<TEdge, TNewEdge> edgeConverter) => From<TNode, TNode, TNewEdge, TEdge>(graph, node => node, edgeConverter);
    
    public static MutableGraph<TNewNode, TNewEdge> From<TNewNode, TNode, TNewEdge, TEdge>(IGraph<TNode, TEdge> graph,
        Func<TNode, TNewNode> nodeConverter, Func<TEdge, TNewEdge> edgeConverter)
    {
        var newGraph = new MutableGraph<TNewNode, TNewEdge>(new HashSet<MutableNode<TNewNode, TNewEdge>>(),
            new HashSet<IEdge<TNewNode, TNewEdge>>());

        var newNodesByOldNode = graph.Nodes.ToDictionary(node => node, node => newGraph.Add(nodeConverter(node.Value)));

        foreach (var edge in graph.Edges)
        {
            newGraph.Add(edgeConverter(edge.Value), newNodesByOldNode[edge.NodeA], newNodesByOldNode[edge.NodeB]);
        }

        return newGraph;
    }
}

public record MutableGraph<TNode, TEdge>(HashSet<MutableNode<TNode, TEdge>> Nodes, HashSet<IEdge<TNode, TEdge>> Edges) : IGraph<TNode, TEdge>
{
    IReadOnlyCollection<INode<TNode, TEdge>> IGraph<TNode, TEdge>.Nodes => Nodes;
    IEnumerable<IEdge<TNode, TEdge>> IGraph<TNode, TEdge>.Edges => Edges;

    public MutableNode<TNode, TEdge> Add(TNode value)
    {
        var newNode = new MutableNode<TNode, TEdge> { Value = value };
        Nodes.Add(newNode);

        return newNode;
    }
    
    public void Add(TEdge value, MutableNode<TNode, TEdge> nodeA, MutableNode<TNode, TEdge> nodeB)
    {
        var newEdge = new MutableEdge<TNode, TEdge>(nodeA, nodeB) { Value = value };
        nodeA.Add(newEdge);
        nodeB.Add(newEdge);
        Edges.Add(newEdge);
    }

    public void Remove(MutableNode<TNode, TEdge> node)
    {
        Nodes.Remove(node);
        
        foreach (var edge in node.IncomingEdges())
        {
            edge.NodeA.Remove(edge);
            Edges.Remove(edge);
        }
        
        foreach (var edge in node.OutgoingEdges())
        {
            edge.NodeB.Remove(edge);
            Edges.Remove(edge);
        }
    }
}