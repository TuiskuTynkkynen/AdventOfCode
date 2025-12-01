using System.Collections.Generic;
using System.Numerics;

namespace AdventOfCode2023
{
    internal static class Day23
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-23.txt");
            Console.WriteLine("Day twenty three:\t\t(part 2 DNF)\n");

            int result1 = Part1(ref reader);
            Console.WriteLine("Puzzle 1 = " + result1 + "\n");

            int result2 = Part2(ref reader);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(Environment.NewLine) ?? throw new Exception("Error reading input file");
            char forestCharacter = '#';
            char pathCharacter = '.';
            Dictionary<char, Vector2> slopes = new()
            {
                {'v', new Vector2(0, 1)},
                {'>', new Vector2(1, 0)},
                {'<', new Vector2(-1, 0)},
            };
            List<Vector2> directions = new() { 
                new Vector2(-1, 0),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(0, 1),
            };
            Vector2 start = new(input[0].IndexOf(pathCharacter), 0);
            int heigth = input.Length;
            int width = input[0].Length;

            int maxSteps = 0;

            PriorityQueue<HashSet<Vector2>, int> positions = new();
            positions.Enqueue(new() { start }, 0);
            while(positions.Count > 0)
            {
                positions.TryDequeue(out HashSet<Vector2>? currentPath, out int steps);
                if (currentPath == null)
                {
                    continue;
                }
                Vector2 current = currentPath.Last();

                maxSteps = (maxSteps > steps) ? steps : maxSteps;

                foreach(Vector2 direction in directions)
                {
                    Vector2 temp = current + direction;
                    int x = (int)temp.X;
                    int y = (int)temp.Y;
                    if (x < 0 || x >= width || y < 0 || y >= heigth || input[y][x] == forestCharacter || currentPath.Contains(temp))
                    {
                        continue;
                    }
                    else if(input[y][x] != pathCharacter)
                    {
                        slopes.TryGetValue(input[y][x], out Vector2 slope);
                        if (direction != slope)
                        {
                            continue;
                        }
                    }

                    HashSet<Vector2> tempPath = currentPath.ToHashSet();
                    tempPath.Add(temp);
                    positions.Enqueue(tempPath, tempPath.Count * -1);
                }
            }
            
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return maxSteps * -1 - 1;
        }

        private static int Part2(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(Environment.NewLine) ?? throw new Exception("Error reading input file");
            List<Vector2> directions = new() {
                new Vector2(-1, 0),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(0, 1),
            };
            char forestCharacter = '#';
            char pathCharacter = '.';
            int heigth = input.Length;
            int width = input[0].Length;
            Vector2 start = new(input[0].IndexOf(pathCharacter), 0);
            Vector2 end = new(input[heigth - 1].IndexOf(pathCharacter), heigth - 1);
            int maxSteps = 0;

            PriorityQueue<HashSet<Vector2>, int> positions = new();
            positions.Enqueue(new() { start }, 0);
            while (positions.Count > 0)
            {
                positions.TryDequeue(out HashSet<Vector2>? currentPath, out int steps);
                if(currentPath == null)
                {
                    continue;
                }
                Vector2 current = currentPath.Last();

                if(current == end && maxSteps > steps) {
                    Console.WriteLine($"step {steps * -1} @ end");
                }

                maxSteps = (current == end && maxSteps > steps) ? steps : maxSteps;


                foreach (Vector2 direction in directions)
                {
                    Vector2 temp = current + direction;
                    int x = (int)temp.X;
                    int y = (int)temp.Y;
                    if (x < 0 || x >= width || y < 0 || y >= heigth || input[y][x] == forestCharacter || currentPath.Contains(temp))
                    {
                        continue;
                    }

                    HashSet<Vector2> tempPath = currentPath.ToHashSet();
                    tempPath.Add(temp);
                    positions.Enqueue(tempPath, tempPath.Count * -1);
                }
            }
            //ans > 6207
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return maxSteps * -1 - 1;
        }
    }
}
