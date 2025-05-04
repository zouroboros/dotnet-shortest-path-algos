namespace Graph.Test;

public class EdgesTestData
{
    public static TheoryData<IEnumerable<INode<string, string>>, IEnumerable<IEdge<string, string>>>
        AllEdgesReturnsCorrectResultExample()
    {
        var examples = new TheoryData<IEnumerable<INode<string, string>>, IEnumerable<IEdge<string, string>>>();
        
        var graphBuilder = new GraphBuilder<string, string>();

        var nodeA = graphBuilder.CreateNode("A");
        var nodeB = graphBuilder.CreateNode("B");
        
        examples.Add([nodeA, nodeB], []);
        
        var nodeC = graphBuilder.CreateNode("C");
        var nodeD = graphBuilder.CreateNode("D");
        
        var cToD = graphBuilder.CreateEdge("C to D", nodeC, nodeD);
        
        examples.Add([nodeC, nodeD], [cToD]);
        
        var nodeE = graphBuilder.CreateNode("C");
        var nodeF = graphBuilder.CreateNode("D");
        
        var eToF = graphBuilder.CreateEdge("E to F", nodeE, nodeF);
        var fToE = graphBuilder.CreateEdge("F to E", nodeF, nodeE);
        
        examples.Add([nodeE, nodeF], [eToF, fToE]);
        
        return examples;
    }
}