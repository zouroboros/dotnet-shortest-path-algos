using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using Algorithm;
using GTFS;
using GtfsPlanner;

var rootCommand = new RootCommand("GtsPlanner");
var gtfsOption = new Option<FileInfo>("--gtfs", "Path to the GTFS file.");
var dateOption = new Option<DateOnly>("--date", "Date on which to find a route");
var startOption = new Option<string>("--start", "Name of the start of the route.");
var endOption = new Option<string>("--dest", "Name of the destination of the route.");

rootCommand.Add(gtfsOption);
rootCommand.Add(dateOption);
rootCommand.Add(startOption);
rootCommand.Add(endOption);

rootCommand.SetHandler((gtfsFile, date, start, destination) =>
{
    Console.WriteLine($"Opening gtfs {gtfsFile}");

    var reader = new GTFSReader<GTFSFeed>
    {
        DateTimeReader = dateTimeString =>
        {
            if (dateTimeString.Length == 10)
            {
                return DateTime.ParseExact(dateTimeString[1..^1], "yyyyMMdd", CultureInfo.InvariantCulture);
            }

            return DateTime.ParseExact(dateTimeString, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    };

    var feed = reader.Read(gtfsFile.FullName);
    var tripReader = new TripReader(feed);
    
    var trips = tripReader.ReadTrips(date).ToList();

    Console.WriteLine($"Found {trips.Count} trips");

    var temporalGraph = Graphs.CreateTemporalGraph(trips);

    var edgesInTemporalGraph = temporalGraph.Nodes.SelectMany(node => node.Edges);

    Console.WriteLine($"Number edges {edgesInTemporalGraph.Count()}");

    var startNode = temporalGraph.Nodes.First(node => node.Value.Name == start);
    var destinationNode = temporalGraph.Nodes.First(node => node.Value.Name == destination);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var temporalPaths = PathFinder.FindShortestPaths(temporalGraph, startNode,
        destinationNode);
    stopwatch.Stop();

    Console.WriteLine($"Found {temporalPaths.Count} paths and the calculation took {stopwatch.ElapsedMilliseconds}ms.");
}, gtfsOption, dateOption, startOption, endOption);

await rootCommand.InvokeAsync(args);