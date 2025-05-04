using System.Linq;

namespace Graph;

public static class Graphs
{
    public static IGraph<IEdge<TNode, TEdge>, TNode> ToLineGraph<TNode, TEdge>(this IGraph<TNode, TEdge> graph)
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

                graphBuilder.CreateEdge(firstEdge.NodeB.Value, nodeForEdge, nodeForNextEdge);
            }
        }

        return new Graph<IEdge<TNode, TEdge>, TNode>(newNodeByEdge.Values);
    }
}