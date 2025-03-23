// See https://aka.ms/new-console-template for more information

using System.Globalization;
using Algorithm;
using Algorithm.Algorithm.ShortestPath;
using Algorithm.Algorithm.Ssmtspp;
using Algorithm.Model;
using GTFS;
using GtfsPlanner;

Console.WriteLine($"Opening gtfs {args[0]}");

var reader = new GTFSReader<GTFSFeed>();

reader.DateTimeReader = dateTimeString =>
{
    if (dateTimeString.Length == 10)
    {
        return DateTime.ParseExact(dateTimeString[1..^1], "yyyyMMdd", CultureInfo.InvariantCulture);
    }

    return DateTime.ParseExact(dateTimeString, "yyyyMMdd", CultureInfo.InvariantCulture);
};

var feed = reader.Read(args[0]);
var tripReader = new TripReader(feed);

var date = DateOnly.FromDateTime(DateTime.Today);

var trips = tripReader.ReadTrips(date).ToList();

Console.WriteLine($"Found {trips.Count} trips");

var simpleGraph = Graphs.CreateSimpleGraph(trips);
var temporalGraph = Graphs.CreateTemporalGraph(trips);

var edgesInSimpleGraph = simpleGraph.Nodes.SelectMany(node => node.Edges);
var edgesInTemporalGraph = temporalGraph.Nodes.SelectMany(node => node.Edges);

Console.WriteLine($"Number edges {edgesInSimpleGraph.Count()} {edgesInTemporalGraph.Count()}");

var wienHbf = simpleGraph.Nodes.First(node => node.Value.Name == "Wien Hauptbahnhof");
var lienz = simpleGraph.Nodes.First(node => node.Value.Name == "Lienz Bahnhof");

var simplePath = Dijkstra.ShortestPath(simpleGraph, wienHbf, lienz, i => i);

var wienHbfTemporal = temporalGraph.Nodes.First(node => node.Value.Name == "Wien Hauptbahnhof");
var lienzTemporal = temporalGraph.Nodes.First(node => node.Value.Name == "Lienz Bahnhof");

var temporalPaths = Ssmtspp.FindSingleSourceMultiObjectiveTemporalShortestPaths(temporalGraph, wienHbfTemporal,
    lienzTemporal, new LatestDepartureObjective<(string, string), TimedEdge<string>>(null));

Console.WriteLine($"{temporalPaths.Count} paths found.");

foreach (var path in temporalPaths.OrderByDescending(path => path.Last().Value.ArrivalTime - path.First().Value.DepartureTime))
{
    Console.WriteLine(path.Last().Value.ArrivalTime - path.First().Value.DepartureTime);
}