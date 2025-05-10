using System;
using System.Collections.Generic;
using System.Linq;
using Graph.Mutable;

namespace Graph;

public static class Graphs
{
    public static IGraph<IEdge<TNode, TEdge>, TNode> ToLineGraph<TNode, TEdge>(this IGraph<TNode, TEdge> graph, 
        Func<IEdge<TNode, TEdge>, INode<TNode, TEdge>, IEdge<TNode, TEdge>, bool>? edgeFilter = null)
    where TEdge : ITimedEdge
    {
        var graphBuilder = new GraphBuilder<IEdge<TNode, TEdge>, TNode>();

        var newNodeByEdge = graph.Edges.ToDictionary(edge => edge, edge => graphBuilder.CreateNode(edge));

        foreach (var firstEdge in newNodeByEdge.Keys)
        {
            var nodeForEdge = newNodeByEdge[firstEdge];

            foreach (var nextEdge in firstEdge.NodeB.Edges.Where(nextEdge => firstEdge.NodeB == nextEdge.NodeA && firstEdge.Value.ArrivalTime <= nextEdge.Value.DepartureTime))
            {
                var nodeForNextEdge = newNodeByEdge[nextEdge];

                if (edgeFilter is null || edgeFilter(firstEdge, firstEdge.NodeB, nextEdge))
                {
                    graphBuilder.CreateEdge(firstEdge.NodeB.Value, nodeForEdge, nodeForNextEdge);
                }
            }
        }

        return new Graph<IEdge<TNode, TEdge>, TNode>(newNodeByEdge.Values);
    }

    public static void Simplify<TNode, TEdge>(this MutableGraph<TNode, ICollection<TEdge>> graph)
    {
        var graphChanged = true;

        while (graphChanged)
        {
            graphChanged = false;
            
            var nodeWithOneOutgoingEdge = graph.Nodes.FirstOrDefault(node => node.OutgoingEdges().Count() == 1);

            if (nodeWithOneOutgoingEdge is not null)
            {
                var outgoingEdge = nodeWithOneOutgoingEdge.OutgoingEdges().Single();

                foreach (var incomingEdge in nodeWithOneOutgoingEdge.IncomingEdges())
                {
                    graph.Add([..outgoingEdge.Value, ..incomingEdge.Value], incomingEdge.NodeA, outgoingEdge.NodeB);
                }
                
                graph.Remove(nodeWithOneOutgoingEdge);
                
                graphChanged = true;
            }
            
            var nodeWithOneIncomingEdge = graph.Nodes.FirstOrDefault(node => node.IncomingEdges().Count() == 1);
            
            if (nodeWithOneIncomingEdge is not null)
            {
                var incomingEdge = nodeWithOneIncomingEdge.IncomingEdges().Single();

                foreach (var outgoingEdge in nodeWithOneIncomingEdge.OutgoingEdges())
                {
                    graph.Add([..incomingEdge.Value, ..outgoingEdge.Value], incomingEdge.NodeA, outgoingEdge.NodeB);
                }
                
                graph.Remove(nodeWithOneIncomingEdge);
                
                graphChanged = true;
            }
        }
    }
}