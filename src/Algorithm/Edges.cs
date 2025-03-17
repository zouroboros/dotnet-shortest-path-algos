using Graph;

namespace Algorithm;

public static class Edges
{
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