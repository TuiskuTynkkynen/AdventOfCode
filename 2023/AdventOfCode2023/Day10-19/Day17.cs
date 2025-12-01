using System.IO;
using System.Numerics;

namespace AdventOfCode2023
{
    internal static class Day17
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-17.txt");
            Console.WriteLine("Day seventeen");
            int result1 = Part1_1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split("\n") ?? throw new Exception("Error reading input file");
            int height = input.Length;
            int width = input.Last().Length;
            int directionCount = 4;
            Vector2[] directions =
            {
                new Vector2(-1 , 0),
                new Vector2(1 , 0),
                new Vector2(0 , 1),
                new Vector2(0 , -1),
            };

            int[][] heatLoss = new int[height][];
            for (int i = 0; i < height; i++)
            {
                heatLoss[i] = new int[width];
                for (int j = 0; j < width; j++)
                {
                    heatLoss[i][j] = input[i][j] - '0';
                }
            }

            PriorityQueue<Vector4, int> positions = new();
            positions.Enqueue(new Vector4(0, 0, 0, 0), 0);

            HashSet<Vector4> old = new();

            List<int> finalHeats = new();

            while (positions.Count > 0)
            {
                positions.TryDequeue(out Vector4 current, out int heat);

                if (current.Y == height - 1 && current.X == width - 1)
                {
                    finalHeats.Add(heat);
                    Console.WriteLine(heat);
                    continue;
                }

                for (int i = 0; i < directionCount; i++)
                {
                    int y = (int)(current.Y + directions[i].Y);
                    int x = (int)(current.X + directions[i].X);
                    int straight = (i == (int)current.Z) ? (int)current.W + 1 : 1;

                    if (y < 0 || y >= height || x < 0 || x >= width || straight > 3)
                    {
                        continue;
                    }

                    Vector4 newVec = new Vector4(x, y, i, straight);
                    if (!old.Contains(newVec))
                    {
                        old.Add(newVec);
                        positions.Enqueue(new Vector4(x, y, i, straight), heat + heatLoss[y][x]);
                    }
                }
            }

            return -1;
        }

        private static int Part1_1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split("\n") ?? throw new Exception("Error reading input file");
            int height = input.Length;
            int width = input.Last().Length;
            int directionCount = 4;
            Vector2[] directions =
            {
                new Vector2(-1 , 0),
                new Vector2(1 , 0),
                new Vector2(0 , 1),
                new Vector2(0 , -1),
            };

            int[][] heatLoss = new int[height][];
            for (int i = 0; i < height; i++)
            {
                heatLoss[i] = new int[width];
                for (int j = 0; j < width; j++)
                {
                    heatLoss[i][j] = input[i][j] - '0';

                    Console.CursorLeft = j * 3;
                    Console.Write(heatLoss[i][j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            int[][] minHeat = new int[height][];
            for(int i = 0; i < height; i++)
            {
                minHeat[i] = new int[width];
                Array.Fill(minHeat[i], Int32.MaxValue);
            }
            minHeat[0][0] = 0;

            Vector4[][] cameFrom = new Vector4[height][];
            for (int i = 0; i < height; i++)
            {
                cameFrom[i] = new Vector4[width];
            }

            PriorityQueue<Vector4, int> positions = new();
            positions.Enqueue(new Vector4(0, 0, 0, 0), 0);
            while (positions.Count > 0)
            {
                positions.TryDequeue(out Vector4 current, out int heat);

                if(current.X == width - 1 && current.Y ==  height - 1)
                {
                    positions.Clear();
                    continue;
                }

                for (int i = 0; i < directionCount; i++)
                {
                    int y = (int)(current.Y + directions[i].Y);
                    int x = (int)(current.X + directions[i].X);
                    int straight = (i == (int)current.Z) ? (int)current.W + 1 : 0;

                    if (y < 0 || y >= height || x < 0 || x >= width || straight >= 3)
                    {
                        continue;
                    }

                    int score = heat + heatLoss[y][x];
                    if(score <= minHeat[y][x])
                    {
                        positions.Enqueue(new Vector4(x, y, i, straight), heat + heatLoss[y][x]);
                        minHeat[y][x] = score;
                        cameFrom[y][x] = current;
                    }
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.CursorLeft = j * 3;
                    Console.Write(minHeat[i][j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            
            int sum = 0;
            List<Vector4> path = new();
            if (true)
            {
                int y = height - 1;
                int x = width - 1;
                while (y >= 0 && x >= 0 && x + y > 0)
                {
                    sum += heatLoss[y][x];
                    Vector4 vec = cameFrom[y][x];
                    y = (int)vec.Y;
                    x = (int)vec.X;
                    path.Add(vec);
                }
            }

            path.Reverse();
            path.Add(new Vector4(height - 1, width - 1, 0, -1));
            foreach(Vector4 vec in path)
            {
                Console.Write($"({vec.X}, {vec.Y}), ");
            }
            Console.WriteLine();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Vector2 vec = new Vector2(j, i);
                    Console.CursorLeft = j * 3;
                    Console.Write(heatLoss[i][j]);
                }
                Console.WriteLine();
            }

            int cursorY = Console.CursorTop - height;
            Console.ForegroundColor = ConsoleColor.Red;
            foreach(Vector4 vec in path)
            {
                int y = (int)vec.Y;
                int x = (int)vec.X;
                Console.CursorTop = cursorY + y;
                Console.CursorLeft = x * 3;
                char c = (vec.Z < 2) ? (vec.Z == 0) ? '<' : '>' : (vec.Z == 3) ? '^' : 'v';
                Console.Write(vec.W);
            }
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine();
            Console.WriteLine(sum);
            return -1;
        }

        private static int Part2(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
