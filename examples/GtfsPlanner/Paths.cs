using System.Collections.Immutable;
using Algorithm.Model;
using Graph;

namespace GtfsPlanner;

public static class Paths
{
    private static (string Departure, DateTime DepartureTime, string Name, string Arrival, DateTime ArrivalTime) MergeEdges(
        IEdge<(string, string), TimedEdge<string>> firstEdge, IEdge<(string, string), TimedEdge<string>> secondEdge)
    {
        return (firstEdge.NodeA.Value.Item2, firstEdge.Value.DepartureTime, firstEdge.Value.Value,
            secondEdge.NodeB.Value.Item2, secondEdge.Value.ArrivalTime);
    }

    public static
        IReadOnlyCollection<(string Departure, DateTime DepartureTime, string Name, string Arrival, DateTime ArrivalTime
            )> MergeConsecutiveEdgesOnSameTrain(
            IReadOnlyCollection<IEdge<(string Id, string Name), TimedEdge<string>>> path)
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
                        newPathAndLastTransition.LastEdge)), edge, edge);
            });

        return newPath.Add(MergeEdges(lastEdge, currentEdge));
    }
}