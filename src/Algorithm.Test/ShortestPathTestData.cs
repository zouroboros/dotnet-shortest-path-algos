using Graph;

namespace Algorithm.Test;

public static class ShortestPathTestData
{
    public static TheoryData<IGraph<string, TimedEdge<string>>, INode<string, TimedEdge<string>>, INode<string, TimedEdge<string>>,
            LatestDepartureObjective<string, TimedEdge<string>>,
            IReadOnlyCollection<IReadOnlyCollection<IEdge<string, TimedEdge<string>>>>>
        FindSingleSourceMultiObjectiveTemporalShortestPaths()
    {
        var examples =
            new TheoryData<IGraph<string, TimedEdge<string>>, INode<string, TimedEdge<string>>,
                INode<string, TimedEdge<string>>,
                LatestDepartureObjective<string, TimedEdge<string>>,
                IReadOnlyCollection<IReadOnlyCollection<IEdge<string, TimedEdge<string>>>>>();
        
        var graphBuilder = new GraphBuilder<string, TimedEdge<string>>();

        // two nodes, no edge -> no result
        
        var node1 = graphBuilder.CreateNode("Node 1");
        var node2 = graphBuilder.CreateNode("Node 2");

        examples.Add(new Graph<string, TimedEdge<string>>([node1, node2]), node1, node2,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), []);
        
        // two nodes, one edge -> one result
        
        var node3 = graphBuilder.CreateNode("Node 3");
        var node4 = graphBuilder.CreateNode("Node 4");

        var edgeFrom3To4 =
                graphBuilder.CreateEdge(new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "3 -> 4"),
            node3,
            node4);
        
        examples.Add(new Graph<string, TimedEdge<string>>([node3, node4]), node3, node4,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), [[edgeFrom3To4]]);
        
        // two nodes, one shorter edge and one longer, longer is later -> one result
        
        var node5 = graphBuilder.CreateNode("Node 5");
        var node6 = graphBuilder.CreateNode("Node 6");

        var firstEdgeFrom5To6 =
            graphBuilder.CreateEdge(new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "5 (10:00) -> 6 (11:00)"),
                node5,
                node6);

        _ = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(09), DateTimes.TodayAt(11, 30), "5 (09:00) -> 6 (11:30)"),
            node5,
            node6);

        examples.Add(new Graph<string, TimedEdge<string>>([node5, node6]), node5, node6,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), [[firstEdgeFrom5To6]]);
        
        // two nodes, one longer edge and shorter longer, longer is earlier -> two results

        var node7 = graphBuilder.CreateNode("Node 7");
        var node8 = graphBuilder.CreateNode("Node 8");
        
        var firstEdgeFrom7To8 =
            graphBuilder.CreateEdge(new TimedEdge<string>(DateTimes.TodayAt(8), DateTimes.TodayAt(10, 30), "7 (08:00) -> 8 (10:30)"),
                node7,
                node8);

        var secondEdgeFrom7To8 = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "7 (10:00) -> 68 (11:00)"),
            node7,
            node8);

        examples.Add(new Graph<string, TimedEdge<string>>([node7, node8]), node7, node8,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), [[firstEdgeFrom7To8], [secondEdgeFrom7To8]]);
        
        // three nodes no path -> no result
        
        var node9 = graphBuilder.CreateNode("Node 9");
        var node10 = graphBuilder.CreateNode("Node 10");
        var node11 = graphBuilder.CreateNode("Node 11");

        _ = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "9 (10:00) -> 10 (11:00)"), node9,
            node10);
        
        _ = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "10 (10:00) -> 11 (11:00)"), node10,
            node11);

        examples.Add(new Graph<string, TimedEdge<string>>([node9, node10, node11]), node9, node11,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), []);
        
        // three nodes one path -> one result
        
        var node12 = graphBuilder.CreateNode("Node 12");
        var node13 = graphBuilder.CreateNode("Node 13");
        var node14 = graphBuilder.CreateNode("Node 14");

        var edgeFrom12To13 = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "12 (10:00) -> 13 (11:00)"), node12,
            node13);
        
        var edgeFrom13To14 = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(11, 30), DateTimes.TodayAt(12), "13 (11:30) -> 14 (12:00)"), node13,
            node14);

        examples.Add(new Graph<string, TimedEdge<string>>([node12, node13, node14]), node12, node14,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), [[edgeFrom12To13, edgeFrom13To14]]);
        
        // three nodes one path, additional edges -> one result
        
        var node15 = graphBuilder.CreateNode("Node 15");
        var node16 = graphBuilder.CreateNode("Node 16");
        var node17 = graphBuilder.CreateNode("Node 17");

        var edgeFrom15To16 = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(10), DateTimes.TodayAt(11), "15 (10:00) -> 16 (11:00)"), node15,
            node16);
        
        var edgeFrom16To17 = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(11, 30), DateTimes.TodayAt(12), "16 (11:30) -> 17 (12:00)"), node16,
            node17);

        _ = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(9), DateTimes.TodayAt(12), "15 (09:00) -> 16 (12:00)"), node15,
            node16);

        _ = graphBuilder.CreateEdge(
            new TimedEdge<string>(DateTimes.TodayAt(9), DateTimes.TodayAt(9, 30), "16 (09:00) -> 17 (09:30)"), node16,
            node17);

        examples.Add(new Graph<string, TimedEdge<string>>([node15, node16, node17]), node15, node17,
            new LatestDepartureObjective<string, TimedEdge<string>>(null), [[edgeFrom15To16, edgeFrom16To17]]);
        
        return examples;
    }
}