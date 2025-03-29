using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using Algorithm;
using Graph;
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

var earliestArrival =
    new Command("EarliestArrival", "Calculates the path with the earliest arrival given a departure time");
var departureTimeOption = new Option<TimeOnly>("--departure-time", "The time of the departure");
earliestArrival.Add(departureTimeOption);

rootCommand.Add(earliestArrival);

var latestDepartureAndEarliestArrival = new Command("LatestDepartureEarliestArrival",
    "Calculates paths with latest possible departure for earliest possible arrival");

rootCommand.Add(latestDepartureAndEarliestArrival);

earliestArrival.SetHandler((gtfsFile, date, start, destination, startTime) =>
{
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

    var temporalGraph = Graphs.CreateTemporalGraph(trips);

    var startNode = temporalGraph.Nodes.First(node => node.Value.Name == start);
    var destinationNode = temporalGraph.Nodes.First(node => node.Value.Name == destination);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var path = PathFinder.FindEarliestArrivalPath(temporalGraph, startNode,
        destinationNode, new DateTime(date, startTime));
    stopwatch.Stop();

    Console.WriteLine($"Found path and the calculation took {stopwatch.ElapsedMilliseconds}ms.");

    var simplifiedPath = Paths.MergeConsecutiveEdgesOnSameTrain(path);
    
    foreach (var (departure, departureTime, via, arrival, arrivalTime) in simplifiedPath)
    {
        Console.WriteLine($"{departure,-40} ({departureTime:HH:mm}) {via,-10} ({arrivalTime:HH:mm}) {arrival,-40}");
    }
    
}, gtfsOption, dateOption, startOption, endOption, departureTimeOption);

latestDepartureAndEarliestArrival.SetHandler((gtfsFile, date, start, destination) =>
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
    
    var temporalGraph = Graphs.CreateTemporalGraph(trips);
    
    var startNode = temporalGraph.Nodes.First(node => node.Value.Name == start);
    var destinationNode = temporalGraph.Nodes.First(node => node.Value.Name == destination);

    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var temporalPaths = PathFinder.FindShortestPaths(temporalGraph, startNode,
        destinationNode);
    stopwatch.Stop();

    Console.WriteLine($"Found {temporalPaths.Count} paths and the calculation took {stopwatch.ElapsedMilliseconds}ms.");

    foreach (var temporalPath in temporalPaths)
    {
        var simplifiedPath = Paths.MergeConsecutiveEdgesOnSameTrain(temporalPath);
        Console.WriteLine(
            $"{simplifiedPath.First().Departure}, {simplifiedPath.First().DepartureTime}, {simplifiedPath.Last().Arrival}, {simplifiedPath.Last().ArrivalTime}, {simplifiedPath.Last().ArrivalTime - simplifiedPath.First().DepartureTime}, {string.Join(", ",
                simplifiedPath.Select(transition =>
                    $"{transition.Departure}, {transition.DepartureTime}, {transition.Name}, {transition.Arrival} {transition.ArrivalTime}"))}");
    }
    
}, gtfsOption, dateOption, startOption, endOption);

await rootCommand.InvokeAsync(args);