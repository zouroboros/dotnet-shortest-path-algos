using Graph;

namespace Algorithm.Algorithm.ShortestPath;

public static class Dijkstra
{
    public static IReadOnlyCollection<IEdge<TNode, TEdge>> ShortestPath<TNode, TEdge, TCost>(IGraph<TNode, TEdge> graph,
        INode<TNode, TEdge> start, INode<TNode, TEdge> end, Func<TCost, IEdge<TNode, TEdge>, TCost> cost, TCost initialCostForStartNode,
        TCost initialCostForNonStartNodes)
        where TCost : IComparable<TCost>

    {
        var previousEdges = LabelNodes(graph, start, cost, initialCostForStartNode, initialCostForNonStartNodes);

        return ConstructPath<TNode, TEdge, TCost>(end, previousEdges);
    }

    public static IReadOnlyCollection<IEdge<TNode, TEdge>> ConstructPath<TNode, TEdge, TCost>(INode<TNode, TEdge> end,
        Dictionary<INode<TNode, TEdge>, IEdge<TNode, TEdge>?> previousEdges)
        where TCost : IComparable<TCost>
    {
        var edges = new Stack<IEdge<TNode, TEdge>>();

        var lastEdge = previousEdges[end];

        while (lastEdge is not null)
        {
            edges.Push(lastEdge);
            previousEdges.TryGetValue(lastEdge.NodeA, out lastEdge);
        }

        return edges;
    }

    public static Dictionary<INode<TNode, TEdge>, IEdge<TNode, TEdge>?> LabelNodes<TNode, TEdge, TCost>(
        IGraph<TNode, TEdge> graph, INode<TNode, TEdge> start, Func<TCost, IEdge<TNode, TEdge>, TCost> cost,
        TCost initialCostForStartNode, TCost initialCostForNonStartNodes,
        Func<INode<TNode, TEdge>, bool>? nodeFilter = null) where TCost : IComparable<TCost>
    {
        var distances = new Dictionary<INode<TNode, TEdge>, TCost>(graph.Nodes.Count);
        var remainingElements =
            new SortedSet<INode<TNode, TEdge>>(
                Comparer<INode<TNode, TEdge>>.Create((x, y) =>
                    (distances[x], x.GetHashCode()).CompareTo((distances[y], y.GetHashCode()))));

        foreach (var node in graph.Nodes)
        {
            if (nodeFilter is null || nodeFilter(node))
            {
                if (node.Equals(start))
                {
                    distances[node] = initialCostForStartNode;
                    remainingElements.Add(node);
                }
                else
                {
                    distances[node] = initialCostForNonStartNodes;
                    remainingElements.Add(node);
                }
            }
        }

        var previousEdges = new Dictionary<INode<TNode, TEdge>, IEdge<TNode, TEdge>?>(graph.Nodes.Count);

        while (remainingElements.Count > 0)
        {
            var nearestNode = remainingElements.Min!;
            remainingElements.Remove(nearestNode);
            var currentDistance = distances[nearestNode];

            var nextEdges = nearestNode.Edges.Where(edge => remainingElements.Contains(edge.NodeB));

            foreach (var edge in nextEdges)
            {
                var distanceToNeighbor = cost(currentDistance, edge);

                if (distances[edge.NodeB].CompareTo(distanceToNeighbor) > 0)
                {
                    remainingElements.Remove(edge.NodeB);
                    distances[edge.NodeB] = distanceToNeighbor;
                    remainingElements.Add(edge.NodeB);
                    previousEdges[edge.NodeB] = edge;
                }
            }
        }

        return previousEdges;
    }
}