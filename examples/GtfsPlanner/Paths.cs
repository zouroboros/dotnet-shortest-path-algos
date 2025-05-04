using System.Collections.Immutable;
using Algorithm.Model;
using Graph;

namespace GtfsPlanner;

public static class Paths
{
    private static (string Departure, DateTime DepartureTime, string Name, string Arrival, DateTime ArrivalTime)
        MergeEdges<TNode>(
            IEdge<TNode, TimedEdge<string>> firstEdge, IEdge<TNode, TimedEdge<string>> secondEdge,
            Func<TNode, string> nodeFormatter)
    {
        return (nodeFormatter(firstEdge.NodeA.Value), firstEdge.Value.DepartureTime, firstEdge.Value.Value,
            nodeFormatter(secondEdge.NodeB.Value), secondEdge.Value.ArrivalTime);
    }

    public static
        IReadOnlyCollection<(string Departure, DateTime DepartureTime, string Name, string Arrival, DateTime ArrivalTime
            )> MergeConsecutiveEdgesOnSameTrain<TNode>(
            IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path,
            Func<TNode, string> nodeFormatter)
    {
        var firstEdge = path.First();

        var (newPath, lastEdge, currentEdge) = path.Aggregate(
            (NewPath: ImmutableList<(string Departure, DateTime DepartureTime, string Name, string Arrival, DateTime ArrivalTime)>.Empty,
                LastEdge: firstEdge, CurrentEdge: firstEdge),
            (newPathAndLastTransition, edge) =>
            {
                if (newPathAndLastTransition.CurrentEdge.Value.Value == edge.Value.Value)
                {
                    return (newPathAndLastTransition.NewPath, newPathAndLastTransition.LastEdge, edge);
                }

                return (
                    newPathAndLastTransition.NewPath.Add(MergeEdges(newPathAndLastTransition.LastEdge,
                        newPathAndLastTransition.CurrentEdge, nodeFormatter)), edge, edge);
            });

        return newPath.Add(MergeEdges(lastEdge, currentEdge, nodeFormatter));
    }

    public static IEnumerable<string>
        SelectStationNames<TNode>(IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path,
            Func<TNode, string> nodeFormatter) => path
        .Select(edge => nodeFormatter(edge.NodeA.Value)).Concat(path.Select(edge => nodeFormatter(edge.NodeB.Value)))
        .Distinct();

    public static IEnumerable<string>
        SelectViaNames<TNode>(IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path) => path
        .Select(edge => edge.Value.Value).Distinct();
}