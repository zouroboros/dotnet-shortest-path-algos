using System.Collections.Generic;
using System.Linq;

namespace Graph.Mutable;

public record MutableNode<TNode, TEdge> : INode<TNode, TEdge>
{
    private readonly List<MutableEdge<TNode, TEdge>> _allEdges = new();
    private readonly List<MutableEdge<TNode, TEdge>> _outgoingEdges = new();
    private readonly List<MutableEdge<TNode, TEdge>> _incomingEdges = new();
    
    public required TNode Value { get; set; } 
    IReadOnlyCollection<IEdge<TNode, TEdge>> INode<TNode, TEdge>.Edges => _allEdges;
    public IReadOnlyCollection<MutableEdge<TNode, TEdge>> Edges => _allEdges;

    public IEnumerable<MutableEdge<TNode, TEdge>> OutgoingEdges() => _outgoingEdges;
    public IEnumerable<MutableEdge<TNode, TEdge>> IncomingEdges() => _incomingEdges;

    public void Add(MutableEdge<TNode, TEdge> edge)
    {
        _allEdges.Add(edge);

        if (edge.NodeA == this)
        {
            _outgoingEdges.Add(edge);
        }
        else
        {
            _incomingEdges.Add(edge);
        }
    }
    
    public void Remove(MutableEdge<TNode, TEdge> edge)
    {
        _allEdges.Remove(edge);

        if (edge.NodeA == this)
        {
            _outgoingEdges.Remove(edge);
        }
        else
        {
            _incomingEdges.Remove(edge);
        }
    }
}