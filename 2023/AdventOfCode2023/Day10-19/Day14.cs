using System.Collections;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day14
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-14.txt");
            Console.WriteLine("Day fourteen:\t\t(Puzzle 2 cycle count is 1 insted of 1 000 000)\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            Regex roundStones = new(@"O");
            Regex cubeStones = new(@"#");
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");
            int width = input.IndexOf('\n') + 1;
            int height = input.Length / width;

            List<Vector2> roundeds = roundStones.Matches(input)
                                               .AsParallel()
                                               .Select(match => new Vector2(match.Index % width, match.Index / width))
                                               .ToList();
            
            List<Vector2> cubes = cubeStones.Matches(input)
                                               .AsParallel()
                                               .Select(match => new Vector2(match.Index % width, match.Index / width))
                                               .ToList();
            int roundedCount = roundeds.Count;
            float sum = 0;
            for (int i = 0; i < roundedCount; i++)
            {
                List<float> blockingCubes = cubes.AsParallel()
                                     .Where(item => item.X == roundeds[i].X && item.Y < roundeds[i].Y)
                                     .Select(item => item.Y)
                                     .ToList();

                blockingCubes.AddRange(roundeds.AsParallel()
                                     .Where(item => item.X == roundeds[i].X && item.Y < roundeds[i].Y)
                                     .Select(item => item.Y)
                             );

                if (blockingCubes.Any())
                {
                    roundeds[i] = new Vector2(roundeds[i].X, blockingCubes.Max() + 1);
                }
                else
                {
                    roundeds[i] = new Vector2(roundeds[i].X, 0);
                }

                sum += height - roundeds[i].Y + 1;
            }


            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return (int)sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            Regex roundStones = new(@"O");
            Regex cubeStones = new(@"#");
            int cycleCount = 1; 
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");
            int width = input.IndexOf('\n') + 1;
            int height = input.Length / width;

            List<Vector2> roundeds = roundStones.Matches(input)
                                               .AsParallel()
                                               .Select(match => new Vector2(match.Index % width, match.Index / width))
                                               .ToList();

            List<Vector2> cubes = cubeStones.Matches(input)
                                               .AsParallel()
                                               .Select(match => new Vector2(match.Index % width, match.Index / width))
                                               .ToList();
            int roundedCount = roundeds.Count;

            for (int i = 0; i < cycleCount * 2; i++)
            {
                int direction = (i % 2 == 0) ? 1 : -1;
                int defaultValue = (i % 2 == 0) ? 0 : height;
                for (int j = 0; j < roundedCount; j++)
                {
                    List<float> blockingCubes = cubes.AsParallel()
                                         .Where(item => item.X == roundeds[j].X && item.Y * direction < roundeds[j].Y * direction)
                                         .Select(item => item.Y * direction)
                                         .ToList();

                    List<float> blockingRoundeds = roundeds.AsParallel()
                                         .Where(item => item.X == roundeds[j].X && item.Y * direction < roundeds[j].Y * direction)
                                         .Select(item => item.Y * direction)
                                         .ToList();

                    if (blockingCubes.Any() || blockingRoundeds.Any())
                    {
                        float index = blockingCubes.Any() ? blockingCubes.Max() + 1 : defaultValue * direction;
                        for (; blockingRoundeds.Contains(index); index += 1) ; //Gets the first empty index
                        roundeds[j] = new Vector2(roundeds[j].X, index * direction);
                    }
                    else
                    {
                        roundeds[j] = new Vector2(roundeds[j].X, defaultValue);
                    }
                }

                defaultValue = (i % 2 == 0) ? 0 : width - 3;
                for (int j = 0; j < roundedCount; j++)
                {
                    List<float> blockingCubes = cubes.AsParallel()
                                         .Where(item => item.Y == roundeds[j].Y && item.X * direction < roundeds[j].X * direction)
                                         .Select(item => item.X * direction)
                                         .ToList();

                    List<float> blockingRoundeds = roundeds.AsParallel()
                                         .Where(item => item.Y == roundeds[j].Y && item.X * direction < roundeds[j].X * direction)
                                         .Select(item => item.X * direction)
                                         .ToList();

                    if (blockingCubes.Any() || blockingRoundeds.Any())
                    {
                        float index = blockingCubes.Any() ? blockingCubes.Max() + 1 : defaultValue * direction;
                        for (;  blockingRoundeds.Contains(index); index++); //Gets the first empty index
                        roundeds[j] = new Vector2(index * direction, roundeds[j].Y);
                    }
                    else
                    {
                        roundeds[j] = new Vector2(defaultValue, roundeds[j].Y);
                    }
                }
            }

            float sum = 0;
            foreach(Vector2 vec in roundeds)
            {
                sum += height - vec.Y + 1;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return (int)sum;
        }
    }
}
