using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day25
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-25.txt");
            Console.WriteLine("Day twenty five:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(Environment.NewLine) ?? throw new Exception("Error reading input file");
            Regex foo = new(@"[a-z]+");
            Random random = new();
            Dictionary<string, List<string>> graph = new();
            const int edgeCount = 3;
   
            //Forms original graph
            foreach(string line in input)
            {
                List<string> vertices = foo.Matches(line).Select(match => match.Value).ToList();
                int vertexCount = vertices.Count;

                for(int i = 1; i < vertexCount; i++)
                {
                    if (graph.TryGetValue(vertices[0], out List<string>? connections))
                    {
                        connections.Add(vertices[i]);
                    }
                    else
                    {
                        connections = new() { vertices[i] };
                        graph.Add(vertices[0], connections);
                    }

                    if (graph.TryGetValue(vertices[i], out connections)){
                        connections.Add(vertices[0]);
                    } 
                    else
                    {
                        connections = new() { vertices[0] };
                        graph.Add(vertices[i], connections);
                    }
                }
            }

            //Repeats Karger's algorithm until number of cut edges = edgeCount
            while (true)
            {
                Dictionary<string, List<string>> contractedGraph = new();
                foreach (KeyValuePair<string, List<string>> kv in graph) //Deep copies original graph elements
                {
                    contractedGraph.Add(kv.Key.ToString(), kv.Value.ToList());
                }

                while (contractedGraph.Count > 2)
                {
                    //Gets two random vertices
                    KeyValuePair<string, List<string>> vertex1 = contractedGraph.ElementAt(random.Next(contractedGraph.Count));
                    (string Key, List<string>? Value) vertex2 = new()
                    {
                        Key = vertex1.Value.ElementAt(random.Next(vertex1.Value.Count))
                    };
                    if (!contractedGraph.TryGetValue(vertex2.Key, out vertex2.Value))
                    {
                        continue;
                    }

                    //Removes original vertices
                    contractedGraph.Remove(vertex1.Key);
                    contractedGraph.Remove(vertex2.Key);

                    List<string> edges = vertex1.Value.Concat(vertex2.Value).ToList();
                    List<string> remove = new();

                    //Removes original vertices and adds new contracted vertex to connected vertices
                    foreach (string vertexName in edges)
                    {
                        if (contractedGraph.TryGetValue(vertexName, out List<string>? connections))
                        {
                            connections.Remove(vertex1.Key);
                            connections.Remove(vertex2.Key);
                            connections.Add(vertex1.Key + "," + vertex2.Key);
                        }
                        else
                        {
                            remove.Add(vertexName);
                        }
                    }

                    //Removes edges that connect to deleted vertices
                    foreach (string vertexName in remove)
                    {
                        edges.Remove(vertexName);
                    }

                    //Adds new contracted vertex
                    contractedGraph.Add(vertex1.Key + "," + vertex2.Key, edges);
                }

                //if both vertices have desired number of edges,
                //return the number of elements in first subgraph multiplied by the number of elements in second subgraph
                if (contractedGraph.ElementAt(0).Value.Count == edgeCount && contractedGraph.ElementAt(1).Value.Count == edgeCount)
                {
                    int SubgraphVertexCount1 = foo.Matches(contractedGraph.ElementAt(0).Key).Count;
                    int SubgraphVertexCount2 = foo.Matches(contractedGraph.ElementAt(1).Key).Count;

                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    return SubgraphVertexCount1 * SubgraphVertexCount2;
                }
            }
        }

        private static int Part2(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
