using Graph;

namespace Algorithm;

public static class ShortestPath
{
    public static IReadOnlyCollection<IEdge<TNode, TEdge>> Dijkstra<TNode, TEdge>(IGraph<TNode, TEdge> graph,
        INode<TNode, TEdge> start, INode<TNode, TEdge> end, Func<TEdge, int> cost)
    {
        var distances = new Dictionary<INode<TNode, TEdge>, int>(graph.Nodes.Count);
        var remainingElements =
            new SortedSet<INode<TNode, TEdge>>(
                Comparer<INode<TNode, TEdge>>.Create((x, y) =>
                    (distances[x], x.GetHashCode()).CompareTo((distances[y], y.GetHashCode()))));

        foreach (var node in graph.Nodes)
        {
            if (node.Equals(start))
            {
                distances[node] = 0;
                remainingElements.Add(node);
            }
            else
            {
                distances[node] = int.MaxValue;
                remainingElements.Add(node);
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
                var distanceToNeighbor = currentDistance + cost(edge.Value);
                
                if (distances[edge.NodeB] > distanceToNeighbor)
                {
                    remainingElements.Remove(edge.NodeB);
                    distances[edge.NodeB] = distanceToNeighbor;
                    remainingElements.Add(edge.NodeB);
                    previousEdges[edge.NodeB] = edge;
                }
            }
        }
        
        var edges = new Stack<IEdge<TNode, TEdge>>();

        var lastEdge = previousEdges[end];

        while (lastEdge is not null)
        {
            edges.Push(lastEdge);
            previousEdges.TryGetValue(lastEdge.NodeA, out lastEdge);
        }

        return edges;
    }

    public static IReadOnlyCollection<IReadOnlyCollection<IEdge<TNode, TEdge>>> FindSingleSourceMultiObjectiveTemporalShortestPaths<TNode, TEdge, TObjectives>(
        IGraph<TNode, TEdge> graph,
        INode<TNode, TEdge> start, INode<TNode, TEdge> end, TObjectives initialValue)
        where TObjectives : IComparable<TObjectives>, IObjective<TObjectives, TNode, TEdge>, IEquatable<TObjectives>
        where TEdge : ITimedEdge, IEquatable<TEdge>
        where TNode : IEquatable<TNode>
    {
        var temporaryLabelsByNode = new Dictionary<INode<TNode, TEdge>, SortedSet<NodeLabel<TObjectives, TNode, TEdge>>>(graph.Nodes.Count);
        var permanentLabelsByNode = new Dictionary<INode<TNode, TEdge>, SortedSet<NodeLabel<TObjectives, TNode, TEdge>>>(graph.Nodes.Count);
        var temporaryLabels = new SortedSet<NodeLabel<TObjectives, TNode, TEdge>> ();

        foreach (var node in graph.Nodes)
        {
            temporaryLabelsByNode[node] = new SortedSet<NodeLabel<TObjectives, TNode, TEdge>>();
            permanentLabelsByNode[node] = new SortedSet<NodeLabel<TObjectives, TNode, TEdge>>();
        }

        var initialLabel = new NodeLabel<TObjectives, TNode, TEdge>(DateTime.MinValue, initialValue, null, start, null);
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
                if (ReferenceEquals(edge.NodeA, currentOptimalLabel.Node) && edge.Value.DepartureTime >= currentOptimalLabel.ArrivalTime)
                {
                    var nextNode = edge.NodeB;
                    var newLabel = currentOptimalLabel.Add(edge);

                    var isDominated = temporaryLabelsByNode[nextNode].Concat(permanentLabelsByNode[nextNode]).Any(otherLabel => otherLabel.WeaklyDominates(newLabel));

                    if (!isDominated)
                    {
                        var dominatedLabels = temporaryLabelsByNode[nextNode].Where(otherLabel => newLabel.WeaklyDominates(otherLabel) && !newLabel.Equals(otherLabel)).ToArray();

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

        var finalLabels = permanentLabelsByNode[end];

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
}