using Algorithm.Model;
using Graph;

namespace GtfsPlanner;

public class Display
{
    public void DisplayPath(IReadOnlyCollection<IEdge<(string Id, string Name), TimedEdge<string>>> path)
    {
        var longestStationNameLength = LongestStationNameLength(path);
        var longestViaName = LongestViaNameLength(path);

        DisplayPath(path, longestStationNameLength, longestViaName);
    }

    public void DisplayPaths(
        IReadOnlyCollection<IReadOnlyCollection<IEdge<(string Id, string Name), TimedEdge<string>>>> paths)
    {
        var longestStationNameLength = paths.Max(LongestStationNameLength);
        var longestViaName = paths.Max(LongestViaNameLength);

        foreach (var path in paths)
        {
            DisplayPath(path, longestStationNameLength, longestViaName);
        }
    }

    private static void DisplayPath(IReadOnlyCollection<IEdge<(string Id, string Name), TimedEdge<string>>> path,
        int longestStationNameLength, int longestViaName)
    {
        var simplifiedPath = Paths.MergeConsecutiveEdgesOnSameTrain(path);
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

    private static int LongestStationNameLength(
        IReadOnlyCollection<IEdge<(string Id, string Name), TimedEdge<string>>> path) =>
        Paths.SelectStationNames(path).Max(name => name.Length);

    private static int LongestViaNameLength(
        IReadOnlyCollection<IEdge<(string Id, string Name), TimedEdge<string>>> path) =>
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