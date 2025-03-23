using Algorithm.Algorithm.Ssmtspp;
using Algorithm.Model;
using Graph;

namespace Algorithm.Test.Algorithm.Ssmtspp;

public class SsmtsppTest
{
    [Theory]
    [MemberData(nameof(SsmtsppTestData.FindSingleSourceMultiObjectiveTemporalShortestPaths), MemberType = typeof(SsmtsppTestData))]
    public void FindSingleSourceMultiObjectiveTemporalShortestPaths(
        IGraph<string, TimedEdge<string>> graph, INode<string, TimedEdge<string>> start, INode<string, TimedEdge<string>> end, LatestDepartureObjective<string, TimedEdge<string>> initialValue,
        IReadOnlyCollection<IReadOnlyCollection<IEdge<string, TimedEdge<string>>>> expectedPaths)
    {
        var paths = global::Algorithm.Algorithm.Ssmtspp.Ssmtspp.FindSingleSourceMultiObjectiveTemporalShortestPaths(graph, start, end, initialValue);

        Assert.Equal(expectedPaths, paths);
    }
}