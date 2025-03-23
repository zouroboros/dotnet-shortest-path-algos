using Algorithm.Model;
using Graph;

namespace Algorithm.Algorithm.Ssmtspp;

/// <summary>
/// Label setting algorithm for solving the Single Source Multiobjective Temporal Shortest Path Problem as described in:
/// "C. Bazgan, J. Kager, C. Thielen, and D. Vanderpooten,
/// A general label setting algorithm and tractability analysis for the multiobjective temporal shortest path problem
/// Networks. 85 (2025), 76–90. <seealso href="https://doi.org/10.1002/net.22253">doi.org/10.1002/net.22253</seealso>".
/// </summary>
public static class Ssmtspp
{
    /// <summary>
    /// Finds all paths that correspond to nondominated images of temporal s-v paths.
    /// </summary>
    /// <param name="graph">The graph in which to search.</param>
    /// <param name="start">The starting node.</param>
    /// <param name="end">The destination node.</param>
    /// <param name="initialValue">The initial value of the for the objectives.</param>
    /// <typeparam name="TNode">The type of the node's values.</typeparam>
    /// <typeparam name="TEdge">The type of the edge's values.</typeparam>
    /// <typeparam name="TObjectives">The type of the objective.</typeparam>
    /// <returns>Collection of paths from <paramref name="start"/> to <paramref name="end."/></returns>
    public static IReadOnlyCollection<IReadOnlyCollection<IEdge<TNode, TEdge>>>
        FindSingleSourceMultiObjectiveTemporalShortestPaths<TNode, TEdge, TObjectives>(
            IGraph<TNode, TEdge> graph,
            INode<TNode, TEdge> start, INode<TNode, TEdge> end, TObjectives initialValue)
        where TObjectives : IComparable<TObjectives>, IObjective<TObjectives, TNode, TEdge>, IEquatable<TObjectives>
        where TEdge : ITimedEdge, IEquatable<TEdge>
        where TNode : IEquatable<TNode>
    {
        var permanentLabelsByNode = SetLabels(graph, start, initialValue);

        var finalLabels = permanentLabelsByNode[end];

        var paths = TracePaths(finalLabels);

        return paths;
    }

    /// <summary>
    /// Trace all paths leading to the final labels.
    /// </summary>
    /// <param name="finalLabels">The final labels of the destination node.</param>
    /// <typeparam name="TNode">The type of the node's values.</typeparam>
    /// <typeparam name="TEdge">The type of the edge's values.</typeparam>
    /// <typeparam name="TObjectives">The type of the objective.</typeparam>
    /// <returns></returns>
    public static List<IReadOnlyList<IEdge<TNode, TEdge>>> TracePaths<TNode, TEdge, TObjectives>(
        IEnumerable<NodeLabel<TObjectives, TNode, TEdge>> finalLabels)
        where TObjectives : IComparable<TObjectives>, IObjective<TObjectives, TNode, TEdge>, IEquatable<TObjectives>
        where TEdge : ITimedEdge, IEquatable<TEdge>
        where TNode : IEquatable<TNode>
    {
        var paths = new List<IReadOnlyList<IEdge<TNode, TEdge>>>();

        foreach (var finalLabel in finalLabels)
        {
            var path = new Stack<NodeLabel<TObjectives, TNode, TEdge>>();
            path.Push(finalLabel);

            while (path.TryPeek(out var lastLabel) && lastLabel.PreviousLabel is not null)
            {
                path.Push(lastLabel.PreviousLabel);
            }

            paths.Add(path.Select(label => label.Edge).OfType<IEdge<TNode, TEdge>>().ToList());
        }

        return paths;
    }

    /// <summary>
    /// Computes labels for each node in the graph.
    /// </summary>
    /// <param name="graph">The graph in which to search.</param>
    /// <param name="start">The starting node.</param>
    /// <param name="initialValue">The initial value of the for the objectives.</param>
    /// <typeparam name="TNode">The type of the node's values.</typeparam>
    /// <typeparam name="TEdge">The type of the edge's values.</typeparam>
    /// <typeparam name="TObjectives">The type of the objective.</typeparam>
    /// <returns>Dictionary that maps a node to a set labels.</returns>
    public static Dictionary<INode<TNode, TEdge>, SortedSet<NodeLabel<TObjectives, TNode, TEdge>>>
        SetLabels<TNode, TEdge, TObjectives>(IGraph<TNode, TEdge> graph, INode<TNode, TEdge> start,
            TObjectives initialValue)
        where TObjectives : IComparable<TObjectives>, IObjective<TObjectives, TNode, TEdge>, IEquatable<TObjectives>
        where TEdge : ITimedEdge, IEquatable<TEdge>
        where TNode : IEquatable<TNode>
    {
        var temporaryLabelsByNode =
            new Dictionary<INode<TNode, TEdge>, SortedSet<NodeLabel<TObjectives, TNode, TEdge>>>(graph.Nodes.Count);
        var permanentLabelsByNode =
            new Dictionary<INode<TNode, TEdge>, SortedSet<NodeLabel<TObjectives, TNode, TEdge>>>(graph.Nodes.Count);
        var temporaryLabels = new SortedSet<NodeLabel<TObjectives, TNode, TEdge>>();

        foreach (var node in graph.Nodes)
        {
            temporaryLabelsByNode[node] = new SortedSet<NodeLabel<TObjectives, TNode, TEdge>>();
            permanentLabelsByNode[node] = new SortedSet<NodeLabel<TObjectives, TNode, TEdge>>();
        }

        var labelCount = 0;

        var initialLabel =
            new NodeLabel<TObjectives, TNode, TEdge>(DateTime.MinValue, initialValue, labelCount, null, start, null);
        temporaryLabelsByNode[start].Add(initialLabel);
        temporaryLabels.Add(initialLabel);

        while (temporaryLabels.Count > 0)
        {
            var currentOptimalLabel = temporaryLabels.Min!;

            temporaryLabels.Remove(currentOptimalLabel);
            temporaryLabelsByNode[currentOptimalLabel.Node].Remove(currentOptimalLabel);
            permanentLabelsByNode[currentOptimalLabel.Node].Add(currentOptimalLabel);

            foreach (var edge in currentOptimalLabel.Node.Edges)
            {
                if (ReferenceEquals(edge.NodeA, currentOptimalLabel.Node) &&
                    edge.Value.DepartureTime >= currentOptimalLabel.ArrivalTime)
                {
                    labelCount++;
                    var newLabel = currentOptimalLabel.Add(edge, labelCount);
                    var nextNode = newLabel.Node;

                    var isWeaklyDominated = temporaryLabelsByNode[nextNode].Concat(permanentLabelsByNode[nextNode])
                        .Any(otherLabel => otherLabel.WeaklyDominates(newLabel));

                    if (!isWeaklyDominated)
                    {
                        var dominatedLabels = temporaryLabelsByNode[nextNode]
                            .Where(otherLabel => newLabel.Dominates(otherLabel)).ToArray();

                        foreach (var dominatedLabel in dominatedLabels)
                        {
                            temporaryLabelsByNode[nextNode].Remove(dominatedLabel);
                            temporaryLabels.Remove(dominatedLabel);
                        }

                        temporaryLabelsByNode[nextNode].Add(newLabel);
                        temporaryLabels.Add(newLabel);
                    }
                }
            }
        }

        return permanentLabelsByNode;
    }
}