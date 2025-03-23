using Graph;

namespace Algorithm.Model;

public static class Edges
{
    /// <summary>
    /// Converts a list of edges into a list of nodes.
    /// </summary>
    /// <param name="edges">List of edges.</param>
    /// <typeparam name="TNode">The type of the node values.</typeparam>
    /// <typeparam name="TEdge">The type of the edge values.</typeparam>
    /// <returns>Enumeration of nodes.</returns>
    public static IEnumerable<INode<TNode, TEdge>> NodesOfPath<TNode, TEdge>(IEnumerator<IEdge<TNode, TEdge>> edges)
    {
        if (edges.MoveNext())
        {
            yield return edges.Current.NodeA;
            yield return edges.Current.NodeB;
            
            while (edges.MoveNext())
            {
                yield return edges.Current.NodeB;
            }
        }
    }
}