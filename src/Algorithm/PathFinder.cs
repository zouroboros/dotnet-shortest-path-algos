using Algorithm.Algorithm.ShortestPath;
using Algorithm.Algorithm.Ssmtspp;
using Algorithm.Model;
using Graph;

namespace Algorithm;

/// <summary>
/// Class the that convenience methods for finding paths in graphs.
/// </summary>
public static class PathFinder
{
    /// <summary>
    /// Finds the shortest path in directed graph.
    /// </summary>
    /// <param name="graph">The graph in which to search.</param>
    /// <param name="start">The start node.</param>
    /// <param name="end">The end node.</param>
    /// <param name="cost">Function for extracting the cost/distance of an edge.</param>
    /// <typeparam name="TNode">The type of node values.</typeparam>
    /// <typeparam name="TEdge">The type of edge values.</typeparam>
    /// <returns>A list of edges that represent the path.</returns>
    public static IReadOnlyCollection<IEdge<TNode, TEdge>> FindShortestPath<TNode, TEdge>(IGraph<TNode, TEdge> graph,
        INode<TNode, TEdge> start, INode<TNode, TEdge> end, Func<TEdge, int> cost) =>
        Dijkstra.ShortestPath(graph, start, end, (currentCost, edge) => currentCost + cost(edge.Value), 0,
            int.MaxValue);
    
    /// <summary>
    /// Finds all possible paths of shortest duration from start to end.
    /// </summary>
    /// <param name="graph">The graph in which to search.</param>
    /// <param name="start">The start node.</param>
    /// <param name="end">The end node.</param>
    /// <typeparam name="TNode">The type of node values.</typeparam>
    /// <typeparam name="TEdge">The type of edge values.</typeparam>
    /// <returns>A list of edge paths that represent the possible shortest connections.</returns>
    public static IReadOnlyCollection<IReadOnlyCollection<IEdge<TNode, TEdge>>>
        FindShortestPaths<TNode, TEdge>(
            IGraph<TNode, TEdge> graph,
            INode<TNode, TEdge> start, INode<TNode, TEdge> end)
        where TEdge : ITimedEdge, IEquatable<TEdge>
        where TNode : IEquatable<TNode> =>
        Ssmtspp.FindSingleSourceMultiObjectiveTemporalShortestPaths(graph, start, end,
            new LatestDepartureObjective<TNode, TEdge>());

    /// <summary>
    /// Finds the path to <paramref name="end"/> with the earliest arrival time starting in <paramref name="start"/> at
    /// <paramref name="departureTime"/>..
    /// </summary>
    /// <param name="graph">The graph in which to search.</param>
    /// <param name="start">The starting point.</param>
    /// <param name="end">The end point.</param>
    /// <param name="departureTime">The earliest departure time.</param>
    /// <typeparam name="TNode">Type of node labels.</typeparam>
    /// <typeparam name="TEdge">Type of edge labels.</typeparam>
    /// <returns></returns>
    public static IReadOnlyCollection<IEdge<TNode, TEdge>> FindEarliestArrivalPath<TNode, TEdge>(
        IGraph<TNode, TEdge> graph,
        INode<TNode, TEdge> start, INode<TNode, TEdge> end, DateTime departureTime) where TEdge : ITimedEdge
        => Dijkstra.ShortestPath(graph, start, end, (currentCost, edge) =>
        {
            if (currentCost == TimeSpan.MaxValue || edge.Value.DepartureTime < departureTime + currentCost)
            {
                return TimeSpan.MaxValue;
            }

            return edge.Value.ArrivalTime - departureTime;
        }, TimeSpan.Zero, TimeSpan.MaxValue);

    public static IEnumerable<IEnumerable<IEdge<TNode, TEdge>>> FindEarliestArrivalPaths<TNode, TEdge>(IGraph<IEdge<TNode, TEdge>, TNode> lineGraph, TNode start,
        TNode end, DateTime departureTime) where TEdge : ITimedEdge, IEquatable<TEdge>
    {
        var startNodes = lineGraph.Nodes.Where(node =>
            Equals(node.Value.NodeA.Value, start) && node.Value.Value.DepartureTime >= departureTime);
        var endNodes = lineGraph.Nodes.Where(node => Equals(node.Value.NodeB.Value, end)).ToArray();
        
        foreach (var startNode in startNodes.OrderBy(startNode => startNode.Value.Value.DepartureTime))
        {
            var labels = Dijkstra.LabelNodes(lineGraph, startNode, (currentCost, edge) =>
            {
                if (currentCost == TimeSpan.MaxValue ||
                    edge.NodeB.Value.Value.DepartureTime < departureTime + currentCost)
                {
                    return TimeSpan.MaxValue;
                }
                
                return edge.NodeB.Value.Value.ArrivalTime - departureTime;
            }, TimeSpan.Zero, TimeSpan.MaxValue,
                node => node.Value.Value.DepartureTime >= startNode.Value.Value.DepartureTime);
            
            foreach (var endNode in endNodes)
            {
                if (labels.ContainsKey(endNode))
                {
                    yield return Paths
                        .EdgePathToNodePath(
                            Dijkstra.ConstructPath<IEdge<TNode, TEdge>, TNode, TimeSpan>(endNode, labels))
                        .Select(node => node.Value);
                }
            }
        }
    }
}