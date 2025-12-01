using System.Numerics;

namespace AdventOfCode2023
{
    internal static class Day16
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-16.txt");
            Console.WriteLine("Day sixteen:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split("\n") ?? throw new Exception("Error reading input file");
            int height = input.Length;
            int width = input.Last().Length;
            Vector4 initial = new Vector4(0, 0, 0, 1);

            int sum = solveBeam(ref initial, ref input, height, width);
            
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split("\n") ?? throw new Exception("Error reading input file");
            int height = input.Length;
            int width = input.Last().Length;
            Vector4 initial;
            List<int> energized = new();

            for(int i = 0; i < height; i++)
            {
                initial = new Vector4(0, i, 0, 1);
                energized.Add(solveBeam(ref initial, ref input, height, width));
                initial = new Vector4(width, i, 0, -1);
                energized.Add(solveBeam(ref initial, ref input, height, width));
            }

            for(int i = 0; i < width; i++)
            {
                initial = new Vector4(i, 0, 1, 0);
                energized.Add(solveBeam(ref initial, ref input, height, width));
                initial = new Vector4(i, height, -1, 0);
                energized.Add(solveBeam(ref initial, ref input, height, width));
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return energized.Max();
        }

        private static int solveBeam(ref Vector4 initial, ref string[] input, int height, int width)
        {
            bool[][] visited = new bool[height][];
            List<Vector4> beams = new()
            {
                initial
            }; //x and y are the postion, z is the vertical direction, w is the horizotal direction
            HashSet<Vector4> path = new();
            for (int i = 0; i < height; i++)
            {
                visited[i] = new bool[width];
            }

            while (beams.Any())
            {
                Vector4 beam = beams.Last();
                beams.Remove(beam);

                if (beam.X < 0 || beam.X >= width || beam.Y < 0 || beam.Y >= height || path.Contains(beam))
                {
                    continue;
                }

                visited[(int)beam.Y][(int)beam.X] = true;
                path.Add(beam);

                char tile = input[(int)beam.Y][(int)beam.X];

                if (tile == '|')
                {
                    if (beam.W != 0)
                    {
                        beams.Add(new Vector4(beam.X, beam.Y + 1, 1, 0));
                        beams.Add(new Vector4(beam.X, beam.Y - 1, -1, 0));
                    }
                    else
                    {
                        beams.Add(new Vector4(beam.X, beam.Y + beam.Z, beam.Z, beam.W));
                    }
                }
                else if (tile == '-')
                {
                    if (beam.Z != 0)
                    {
                        beams.Add(new Vector4(beam.X + 1, beam.Y, 0, 1));
                        beams.Add(new Vector4(beam.X - 1, beam.Y, 0, -1));
                    }
                    else
                    {
                        beams.Add(new Vector4(beam.X + beam.W, beam.Y, beam.Z, beam.W));
                    }
                }
                else if (tile == '/')
                {
                    beams.Add(new Vector4(beam.X + beam.Z * -1, beam.Y + beam.W * -1, beam.W * -1, beam.Z * -1));
                }
                else if (tile == '\\')
                {
                    beams.Add(new Vector4(beam.X + beam.Z, beam.Y + beam.W, beam.W, beam.Z));
                }
                else
                {
                    beams.Add(new Vector4(beam.X + beam.W, beam.Y + beam.Z, beam.Z, beam.W));
                }
            }

            int sum = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += visited[i][j] ? 1 : 0;
                }
            }
            return sum;
        }
    }
}
