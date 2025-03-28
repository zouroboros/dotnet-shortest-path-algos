using Graph;

namespace Algorithm.Algorithm.ShortestPath;

public static class Dijkstra
{
    public static IReadOnlyCollection<IEdge<TNode, TEdge>> ShortestPath<TNode, TEdge>(IGraph<TNode, TEdge> graph,
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
}