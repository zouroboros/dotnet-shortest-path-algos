using Algorithm.Model;
using Graph;

namespace GtfsPlanner;

public class Display
{
    public void DisplayPath<TNode>(IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path, Func<TNode, string> nodeFormatter)
    {
        var longestStationNameLength = LongestStationNameLength(path, nodeFormatter);
        var longestViaName = LongestViaNameLength(path);

        DisplayPath(path, longestStationNameLength, longestViaName, nodeFormatter);
    }

    public void DisplayPaths<TNode>(
        IReadOnlyCollection<IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>>> paths, Func<TNode, string> nodeFormatter)
    {
        var longestStationNameLength = paths.Max(path => LongestStationNameLength(path, nodeFormatter));
        var longestViaName = paths.Max(LongestViaNameLength);

        foreach (var path in paths)
        {
            DisplayPath(path, longestStationNameLength, longestViaName, nodeFormatter);
        }
    }

    private static void DisplayPath<TNode>(IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path,
        int longestStationNameLength, int longestViaName,
        Func<TNode, string> nodeFormatter)
    {
        var simplifiedPath = Paths.MergeConsecutiveEdgesOnSameTrain(path, nodeFormatter);
        var firstTransition = simplifiedPath.First();
        var lastTransition = simplifiedPath.Last();

        var duration = (lastTransition.ArrivalTime - firstTransition.DepartureTime);
        var actualVialLength = Math.Max(duration.ToString("g").Length, longestViaName);

        var originalColor = Console.ForegroundColor;
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            $"# ({firstTransition.DepartureTime:HH:mm}) {firstTransition.Departure.PadRight(longestStationNameLength)} - {(lastTransition.ArrivalTime - firstTransition.DepartureTime).ToString("g").PadRight(actualVialLength)} - ({lastTransition.ArrivalTime:HH:mm}) {lastTransition.Arrival} ");
        Console.ForegroundColor = originalColor;
        
        foreach (var (departure, departureTime, via, arrival, arrivalTime) in simplifiedPath)
        {
            Console.WriteLine(
                $"* ({departureTime:HH:mm}) {departure.PadRight(longestStationNameLength)} - {via.PadRight(actualVialLength)} - ({arrivalTime:HH:mm}) {arrival.PadRight(longestStationNameLength)}");
        }
    }

    private static int LongestStationNameLength<TNode>(
        IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path,
        Func<TNode, string> nodeFormatter) =>
        Paths.SelectStationNames(path, nodeFormatter).Max(name => name.Length);

    private static int LongestViaNameLength<TNode>(
        IReadOnlyCollection<IEdge<TNode, TimedEdge<string>>> path) =>
        Paths.SelectViaNames(path).Max(name => name.Length);

    public void DisplayCalculationTime(TimeSpan calculationTime)
    {
        Console.WriteLine($"Found path and the calculation took {calculationTime.TotalMilliseconds:F}ms.");
    }

    public void DisplayCalculationTimeAndNumberOfResults(TimeSpan calculationTime, int numberOfResults)
    {
        Console.WriteLine(
            $"Found {numberOfResults} paths and the calculation took {calculationTime.TotalMilliseconds:F}ms.");
    }
}