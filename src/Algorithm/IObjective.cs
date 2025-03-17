using Graph;

namespace Algorithm;

public interface IObjective<TSelf, TNode, TEdge>
{
    bool WeaklyDominates(TSelf observable);
    
    TSelf Add(IEdge<TNode, TEdge> edge);
}