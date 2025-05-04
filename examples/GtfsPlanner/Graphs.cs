using Algorithm;
using Algorithm.Model;
using Graph;

namespace GtfsPlanner;

public static class Graphs
{
    public static IGraph<(string Id, string Name), int> CreateSimpleGraph(IReadOnlyCollection<Trip> trips)
    {
        var stops = trips.SelectMany(trip => trip.Stops.Select(stop => (stop.Id, stop.Name))).Distinct();
        
        var graphBuilder = new GraphBuilder<(string Id, string Name), int>();
        
        var nodesByStop = stops.ToDictionary(stop => stop, stop => graphBuilder.CreateNode(stop));

        foreach (var (from, to) in trips.SelectMany(trip =>
                     trip.Stops.PredecessorPairs().Select(tuple =>
                         ((tuple.Item1.Id, tuple.Item1.Name), (tuple.Item2.Id, tuple.Item2.Name)))).Distinct())
        {
            graphBuilder.CreateEdge(0, nodesByStop[(from.Id, from.Name)], nodesByStop[(to.Id, to.Name)]);
        }

        return new Graph<(string Id, string Name), int>(nodesByStop.Values);
    }

    public static IGraph<(string Id, string Name), TimedEdge<string>> CreateTemporalGraph(
        IReadOnlyCollection<Trip> trips)
    {
        var graphBuilder =
            new GraphBuilder<(string Id, string Name), TimedEdge<string>>();

        var stops = trips.SelectMany(trip => trip.Stops.Select(stop => (stop.Id, stop.Name))).Distinct();

        var nodesByStop = stops.ToDictionary(stop => stop, stop => graphBuilder.CreateNode(stop));
        
        var edges = new HashSet<(string, TimedEdge<string>, string)>();

        foreach (var trip in trips)
        {
            foreach (var (from, to) in trip.Stops.PredecessorPairs())
            {
                var newEdge = new TimedEdge<string>(from.DepartureTime, to.ArrivalTime, trip.Name);

                if (edges.Add((from.Id, newEdge, to.Id)))
                {
                    graphBuilder.CreateEdge(newEdge,
                        nodesByStop[(from.Id, from.Name)], nodesByStop[(to.Id, to.Name)]);
                }
            }
        }

        return new Graph<(string Id, string Name), TimedEdge<string>>(nodesByStop.Values);
    }
}