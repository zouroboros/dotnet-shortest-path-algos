using Graph;

namespace Algorithm.Algorithm.Ssmtspp;

public interface IObjective<TSelf, TNode, TEdge>
{
    bool WeaklyDominates(TSelf other);

    bool Dominates(TSelf other);
    
    TSelf Add(IEdge<TNode, TEdge> edge);
}