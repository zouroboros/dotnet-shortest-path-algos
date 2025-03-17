using Graph;

namespace Algorithm.Test;

public class ShortestPathTest
{
    [Theory]
    [MemberData(nameof(ShortestPathTestData.FindSingleSourceMultiObjectiveTemporalShortestPaths), MemberType = typeof(ShortestPathTestData))]
    public void FindSingleSourceMultiObjectiveTemporalShortestPaths(
        IGraph<string, TimedEdge<string>> graph, INode<string, TimedEdge<string>> start, INode<string, TimedEdge<string>> end, TimeSpanObjective<string, TimedEdge<string>> initialValue,
        IReadOnlyCollection<IReadOnlyCollection<IEdge<string, TimedEdge<string>>>> expectedPaths)
    {
        var paths = ShortestPath.FindSingleSourceMultiObjectiveTemporalShortestPaths(graph, start, end, initialValue);

        Assert.Equal(expectedPaths, paths);
    }
}