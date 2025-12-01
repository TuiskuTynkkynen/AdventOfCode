using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day13
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-13.txt");
            Console.WriteLine("Day thirteen:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] patterns = reader.ReadToEnd().Split("\r\n\r\n") ?? throw new Exception("Error reading input file");
            int sum = 0;

            foreach(string pattern in patterns)
            {
                string[] input = pattern.Split('\n');
                int height = input.Length;
                int width = input[height - 1].Length;

                int reflection = 0;
                for (int i = 0; i < width - 1 && reflection == 0; i++)
                {
                    reflection = i + 1;
                    for (int j = 0; j < height && reflection != 0; j++)
                    {
                        for (int k = 0; i + k < width - 1 && i - k >= 0; k++)
                        {
                            if (input[j][i - k] != input[j][i + k + 1])
                            {
                                reflection = 0;
                            }
                        }
                    }
                }
                
                sum += reflection;
                
                reflection = 0;
                for (int i = 0; i < height - 1 && reflection == 0; i++)
                {
                    reflection = i + 1;
                    for (int j = 0; j < width && reflection != 0; j++)
                    {
                        for (int k = 0; i + k + 1 < height && i - k >= 0; k++)
                        {
                            if (input[i - k][j] != input[i + k + 1][j])
                            {
                                reflection = 0;
                            }
                        }
                    }
                }

                sum += reflection * 100;
            }
            
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            string[] patterns = reader.ReadToEnd().Split("\r\n\r\n") ?? throw new Exception("Error reading input file");
            int sum = 0;

            foreach (string pattern in patterns)
            {
                string[] input = pattern.Split('\n');
                int height = input.Length;
                int width = input[height - 1].Length;

                int reflection = 0;
                for (int i = 0; i < width - 1 && reflection == 0; i++)
                {
                    int mismatchCount = 0;
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; i + k < width - 1 && i - k >= 0; k++)
                        {
                            if (input[j][i - k] != input[j][i + k + 1])
                            {
                                mismatchCount++;
                            }
                        }
                    }

                    if(mismatchCount == 1) {
                        reflection = i + 1;
                    }
                }

                sum += reflection;
                
                reflection = 0;
                for (int i = 0; i < height - 1 && reflection == 0; i++)
                {
                    int mismatchCount = 0;
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; i + k + 1 < height && i - k >= 0; k++)
                        {
                            if (input[i - k][j] != input[i + k + 1][j])
                            {
                                mismatchCount++;
                            }
                        }

                    }

                    if (mismatchCount == 1)
                    {
                        reflection = i + 1;
                    }
                }

                sum += reflection * 100;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
    }
}
