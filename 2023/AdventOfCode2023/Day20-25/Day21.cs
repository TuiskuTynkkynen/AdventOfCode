using System.Collections.Generic;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2023
{
    internal static class Day21
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-21.txt");
            Console.WriteLine("Day twenty one:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(Environment.NewLine) ?? throw new Exception("Error reading input file");
            int height = input.Length;
            int width = input.Last().Length;
            char startCharacter = 'S';
            char rockCharacter = '#';
            int stepCount = 64;
            List<Vector2> directions = new() { 
                new Vector2(-1, 0),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(0, 1),
            };

            Vector2 start = input.AsParallel()
                                 .Select((item, ind) => new {line = item, index = ind})
                                 .Where(item => item.line.Contains(startCharacter))
                                 .Select(item => new Vector2(item.index, item.line.IndexOf(startCharacter)))
                                 .First();


            HashSet<Vector2> positions = new() { start };
            Dictionary<Vector2, List<Vector2>> memo = new();

            for (int step = 0; step < stepCount; step++)
            {
                List<Vector2> currentPostions = positions.ToList();
                positions.Clear();

                int count = currentPostions.Count;
                for (int i = 0; i < count; i++)
                {
                    Vector2 current = currentPostions[i];

                    if (memo.TryGetValue(current, out List<Vector2>? added))
                    {
                        int addedCount = added.Count;
                        for (int j = 0; j < addedCount; j++)
                        {
                            positions.Add(added[j]);
                        }
                        continue;
                    }

                    added = new();

                    foreach (Vector2 direction in directions)
                    {
                        Vector2 next = current + direction;

                        if (next.Y < 0 || next.Y >= height || next.X < 0 || next.X >= width || input[(int)next.Y][(int)next.X] == rockCharacter || positions.Contains(next))
                        {
                            continue;
                        }

                        added.Add(next);
                        positions.Add(next);
                    }

                    memo.Add(current, added);

                }

            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return positions.Count;
        }

        private static int Part2(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
