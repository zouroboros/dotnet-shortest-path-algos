namespace Graph.Test;

public class EdgesTest
{
    [Theory]
    [MemberData(nameof(EdgesTestData.AllEdgesReturnsCorrectResultExample), MemberType = typeof(EdgesTestData))]
    public void AllEdgesReturnsCorrectResult(IEnumerable<INode<string, string>> nodes, IEnumerable<IEdge<string, string>> expectedEdges)
    {
        var actualEdges = nodes.AllEdges();
        
        Assert.Equal(expectedEdges, actualEdges);
    }
}