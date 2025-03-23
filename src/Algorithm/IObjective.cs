using Graph;

namespace Algorithm;

public interface IObjective<TSelf, TNode, TEdge>
{
    bool WeaklyDominates(TSelf other);

    bool Dominates(TSelf other);
    
    TSelf Add(IEdge<TNode, TEdge> edge);
}