using System.Globalization;
using Algorithm.Model;
using Graph;
using GTFS;

namespace GtfsPlanner;

public class GtfsImporter
{
    private readonly GTFSReader<GTFSFeed> _reader = new()
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
    
    public IGraph<(string Id, string Name), TimedEdge<string>> ImportGraphFromGtfs(FileInfo gtfsFile, DateOnly date)
    {
        var feed = _reader.Read(gtfsFile.FullName);
        var tripReader = new TripReader(feed);

        var trips = tripReader.ReadTrips(date).ToList();

        return Graphs.CreateTemporalGraph(trips);
    }
}