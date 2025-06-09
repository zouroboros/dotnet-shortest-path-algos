using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using Algorithm;
using Graph;
using Graph.Mutable;
using GtfsPlanner;
using Graphs = GtfsPlanner.Graphs;

var rootCommand = new RootCommand("GtsPlanner");
var gtfsOption = new Option<FileInfo>("--gtfs", "Path to the GTFS file.");
var dateOption = new Option<DateOnly>("--date", "Date on which to find a route");
var startOption = new Option<string>("--start", "Name of the start of the route.");
var endOption = new Option<string>("--dest", "Name of the destination of the route.");

rootCommand.Add(gtfsOption);
rootCommand.Add(dateOption);
rootCommand.Add(startOption);
rootCommand.Add(endOption);

var earliestArrival =
    new Command("EarliestArrival", "Calculates the path with the earliest arrival given a departure time");
var departureTimeOption = new Option<TimeOnly>("--departure-time", "The time of the departure");
earliestArrival.Add(departureTimeOption);

rootCommand.Add(earliestArrival);

var latestDepartureAndEarliestArrival = new Command("LatestDepartureEarliestArrival",
    "Calculates paths with latest possible departure for earliest possible arrival");

rootCommand.Add(latestDepartureAndEarliestArrival);

var earliestArrivalInExtendedGraph = new Command("EarliestArrivalInExtendedGraph");
earliestArrivalInExtendedGraph.AddOption(departureTimeOption);

rootCommand.Add(earliestArrivalInExtendedGraph);

var gtfsImporter = new GtfsImporter();
var display = new Display();

earliestArrival.SetHandler((gtfsFile, date, start, destination, startTime) =>
{
    var temporalGraph = gtfsImporter.ImportGraphFromGtfs(gtfsFile, date);
    
    var startNode = temporalGraph.Nodes.First(node => node.Value.Name == start);
    var destinationNode = temporalGraph.Nodes.First(node => node.Value.Name == destination);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var path = PathFinder.FindEarliestArrivalPath(temporalGraph, startNode,
        destinationNode, new DateTime(date, startTime));
    stopwatch.Stop();

    display.DisplayCalculationTime(stopwatch.Elapsed);
    display.DisplayPath(path, label => label.Name);
    
    
}, gtfsOption, dateOption, startOption, endOption, departureTimeOption);

latestDepartureAndEarliestArrival.SetHandler((gtfsFile, date, start, destination) =>
{
    var temporalGraph = gtfsImporter.ImportGraphFromGtfs(gtfsFile, date);
    
    var startNode = temporalGraph.Nodes.First(node => node.Value.Name == start);
    var destinationNode = temporalGraph.Nodes.First(node => node.Value.Name == destination);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var temporalPaths = PathFinder.FindShortestPaths(temporalGraph, startNode,
        destinationNode);
    stopwatch.Stop();

    display.DisplayCalculationTimeAndNumberOfResults(stopwatch.Elapsed, temporalPaths.Count);
    display.DisplayPaths(temporalPaths, label => label.Name);
    
}, gtfsOption, dateOption, startOption, endOption);

earliestArrivalInExtendedGraph.SetHandler((gtfsFile, date, start, destination, startTime) =>
{
    var temporalGraph = gtfsImporter.ImportGraphFromGtfs(gtfsFile, date);
    var lineGraph = temporalGraph.ToLineGraph((incomingEdge, node, outgoingEdge) => node.Value.Name.Contains("Haupt") || outgoingEdge.Value.DepartureTime - incomingEdge.Value.ArrivalTime < TimeSpan.FromMinutes(30));

    Console.WriteLine($"Total number of nodes in line graph {lineGraph.Nodes.Count} and edges {lineGraph.Edges.Count()}");
    
    var nodesWithOnlyOneEdge = lineGraph.Nodes.Where(node => node.Edges.Count(edge => edge.NodeA == node) <= 1);
    
    Console.WriteLine($"Number with at most one outgoing edge in line graph {nodesWithOnlyOneEdge.Count()}" );

    var mutableGraph = MutableGraph.From(lineGraph,
        new Func<(string Id, string Name), ICollection<(string Id, string Name)>>(value => [value]));
    
    Console.WriteLine($"Original graph: #Nodes {mutableGraph.Nodes.Count} #Edges {mutableGraph.Edges.Count()}");
    
    mutableGraph.Simplify();
    
    Console.WriteLine($"Simplified graph: #Nodes {mutableGraph.Nodes.Count} #Edges {mutableGraph.Edges.Count()}");
    
    var startNode = temporalGraph.Nodes.First(node => node.Value.Name == start).Value;
    var destinationNode = temporalGraph.Nodes.First(node => node.Value.Name == destination).Value;

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var paths = PathFinder
        .FindEarliestArrivalPaths(lineGraph, startNode, destinationNode, new DateTime(date, startTime))
        .Select(edges => edges.ToArray()).ToArray();
    stopwatch.Stop();

    display.DisplayCalculationTimeAndNumberOfResults(stopwatch.Elapsed, paths.Length);
    display.DisplayPaths(paths, label => label.Name);
    
    
}, gtfsOption, dateOption, startOption, endOption, departureTimeOption);

await rootCommand.InvokeAsync(args);