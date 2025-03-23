// See https://aka.ms/new-console-template for more information

using Algorithm;
using Algorithm.Algorithm.ShortestPath;
using Graph;

var builder = new GraphBuilder<string, int>();

var nodeA = builder.CreateNode("Node A");
var nodeB = builder.CreateNode("Node B");
var nodeC = builder.CreateNode("Node C");
var nodeD = builder.CreateNode("Node D");

var fromAToB = builder.CreateEdge(10, nodeA, nodeB);
var fromBToC = builder.CreateEdge(10, nodeB, nodeC);
var fromAToD = builder.CreateEdge(1, nodeA, nodeD);
var fromDtoC = builder.CreateEdge(5, nodeD, nodeC);

var graph = new Graph<string, int>([nodeA, nodeB, nodeC, nodeD]);

var shortestSimplePath = Dijkstra.ShortestPath(graph, nodeA, nodeC, value => value);

Console.WriteLine(shortestSimplePath);