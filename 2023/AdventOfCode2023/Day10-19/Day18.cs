using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day18
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-18.txt");
            Console.WriteLine("Day 18:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string? input = reader.ReadLine() ?? throw new Exception("Error reading input file");
            Dictionary<char, Vector2> operations = new Dictionary<char, Vector2>()
            {
                {'L', new Vector2(-1, 0)},
                {'R', new Vector2(1, 0)},
                {'U', new Vector2(0, -1)},
                {'D', new Vector2(0, 1)},
            };
            List<Vector2> directions = new()
            {
                new Vector2(-1, 0),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(0, 1),
            };
            Regex numbers = new(@"[0-9]+");
            Vector2 min = new Vector2(0, 0);
            Vector2 max = new Vector2(0, 0);
            Vector2 current = new Vector2(0, 0);
            List<Vector2> plan = new();

            while(input != null)
            {
                operations.TryGetValue(input[0], out Vector2 operation);
                int count = int.Parse(numbers.Match(input).Value);

                current.Y += operation.Y * count;
                current.X += operation.X * count;

                min.Y = (current.Y < min.Y) ? current.Y : min.Y;
                max.Y = (current.Y > max.Y) ? current.Y : max.Y;
                min.X = (current.X < min.X) ? current.X : min.X;
                max.X = (current.X > max.X) ? current.X : max.X;

                for(int i = 0; i < count; i++)
                {
                    plan.Add(operation);
                }

                input = reader.ReadLine();
            }

            int height = (int)(max.Y - min.Y + 3);
            int width = (int)(max.X - min.X + 3);
            int[][] map = new int[height][];
            for(int i = 0; i < height; i++)
            {
                map[i] = new int[width];
            }

            current = new Vector2(1 - min.X, 1 - min.Y);
            foreach(Vector2 vec in plan)
            {
                current += vec;
                int y = (int)current.Y;
                int x = (int)current.X;
                map[y][x] = 1;
            }

            List<Vector2> outside = new()
            {
                new Vector2(0, 0)
            };
            HashSet<Vector2> seen = new();

            while (outside.Any())
            {
                Vector2 vec = outside.Last();
                outside.RemoveAt(outside.Count - 1);
                
                map[(int)vec.Y][(int)vec.X] = 2;
                seen.Add(vec);
                
                foreach(Vector2 direction in directions)
                {
                    Vector2 next = vec + direction;
                    int y = (int)next.Y;
                    int x = (int)next.X;
                    if (y < 0 || y >= height || x < 0 || x >= width || seen.Contains(next) || map[y][x] == 1)
                    {
                        continue;
                    }
                    outside.Add(next);
                }
            }

            int sum = 0;
            for (int i = 0; i < height; i++)
            {
                sum += map[i].Where(item => item != 2).Count();
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
        
        private static int Part2(ref StreamReader reader)
        {
            //string[] input = reader.ReadToEnd().Split('(') ?? throw new Exception("Error reading input file");
            string? line = reader.ReadLine() ?? throw new Exception("Error reading input file");
            Regex numbers = new(@"[0-9]+");
            Dictionary<char, Vector2> operations = new Dictionary<char, Vector2>()
            {
                {'0', new Vector2(1, 0)},
                {'1', new Vector2(0, 1)},
                {'2', new Vector2(-1, 0)},
                {'3', new Vector2(0, -1)},
            };
            Vector2 current = new Vector2(0, 0);
            Vector2 next;
            List<Vector2> corners = new();

            int foo = 0;
            while (line != null)
            {
                char c = line[0] == 'R' ? '0' : line[0] == 'D' ? '1' : line[0] == 'L' ? '2' : '3';
                operations.TryGetValue(c, out Vector2 operation);
                int count = int.Parse(numbers.Match(line).Value);

                current.Y += operation.Y * count;
                current.X += operation.X * count;

                Console.WriteLine(line[0] + " " + count + ",\t(" + operation.X + ", " + operation.Y + ")" + ", (" + current.X + ", " + current.Y + ")");
                corners.Add(current);

                line = reader.ReadLine();
            }
            //foreach (string line in input)
            //{
            //    if (line[0] != '#')
            //    { 
            //        continue;
            //    }
            //
            //    operations.TryGetValue(line[6], out Vector2 operation);
            //    string hexString = line.Substring(1, 5);
            //    int count = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            //
            //    char c = line[6] == '0' ? 'R' : line[6] == '1' ? 'D' : line[6] == '2' ? 'L' : 'U';
            //
            //    current.Y += operation.Y * count;
            //    current.X += operation.X * count;
            //
            //    Console.WriteLine(c +  " " + count + ",\t(" + current.X + ", " + current.Y + ")");
            //    corners.Add(current);
            //}


            BigInteger area = 0;
            int stepCount = corners.Count - 1;
            for (int i = 0; i < stepCount; i++)
            {
                current = corners[i];
                next = corners[i + 1];
                area += (long)(current.X * next.Y - current.Y * next.X);
                Console.Write(area + ", ");
            }
            area /= 2;

            Console.WriteLine();
            Console.WriteLine(area);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
