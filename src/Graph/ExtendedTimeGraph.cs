using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph;

public static class ExtendedTimeGraph
{
    public static ExtendedTimeGraph<TNode, TEdge> From<TEdge, TNode>(IGraph<TNode, TEdge> graph,
        Func<INode<TNode, TEdge>, IEdge<TNode, TEdge>, IEnumerable<IEdge<TNode, TEdge>>> edgeSelector)
        where TEdge : ITimedEdge => new(CreateExtendedNodes(graph, edgeSelector));
    
    private static IReadOnlyCollection<INode<(TNode Label, DateTime Time), TEdge>> CreateExtendedNodes<TNode, TEdge>(
        IGraph<TNode, TEdge> graph,
        Func<INode<TNode, TEdge>, IEdge<TNode, TEdge>, IEnumerable<IEdge<TNode, TEdge>>> edgeSelector)
    where TEdge : ITimedEdge
    {
        var graphBuilder = new GraphBuilder<(TNode Label, DateTime Time), TEdge>();

        var newNodesByLabel = graph.Nodes.SelectMany(node =>
                node.Edges.Where(edge => edge.NodeB == node)
                    .Select(edge => graphBuilder.CreateNode((node.Value, edge.Value.ArrivalTime))))
            .GroupBy(node => node.Value)
            .ToDictionary(nodes => nodes.Key, nodes => nodes.First());

        foreach (var node in graph.Nodes)
        {
            foreach (var incomingEdge in node.Edges.Where(edge => edge.NodeB == node))
            {
                var newA = newNodesByLabel[(node.Value, incomingEdge.Value.ArrivalTime)];

                foreach (var outgoingEdge in edgeSelector(node, incomingEdge))
                {
                    var newB = newNodesByLabel[(outgoingEdge.NodeB.Value, outgoingEdge.Value.ArrivalTime)];

                    graphBuilder.CreateEdge(outgoingEdge.Value, newA, newB);
                }
            }
        }

        return newNodesByLabel.Values;
    }
}

/// <summary>
/// Extended time graph where each node is duplicated each (distinct) time an edge arrives at a node.
/// </summary>
/// <param name="Nodes">The nodes of the graph.</param>
/// <typeparam name="TNode">Type of node labels.</typeparam>
/// <typeparam name="TEdge">Type of edge labels.</typeparam>
public record ExtendedTimeGraph<TNode, TEdge>(IReadOnlyCollection<INode<(TNode Label, DateTime Time), TEdge>> Nodes)
    : IGraph<(TNode Label, DateTime Time), TEdge>
{
    private readonly ILookup<TNode, INode<(TNode Label, DateTime Time), TEdge>> _extendedNodesByBaseLabel =
        Nodes.ToLookup(node => node.Value.Label);

    public IEnumerable<INode<(TNode Label, DateTime Time), TEdge>> GetNodesForBaseLabel(TNode baseLabel) =>
        _extendedNodesByBaseLabel[baseLabel];
}