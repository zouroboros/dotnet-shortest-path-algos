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
        Dijkstra.ShortestPath(graph, start, end, cost);

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
}