using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2023
{
    internal static class Day11
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-11.txt");
            long result1 = SolvePuzzle(ref reader, 2);
            long result2 = SolvePuzzle(ref reader, 1000000);

            Console.WriteLine("Day eleven:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static long SolvePuzzle(ref StreamReader reader, int expansionRate)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");
            int lineLength = input.IndexOf('\n') + 1;
            int lineCount = input.Length / lineLength;
            Regex galaxy = new(@"#");
            List<int> galaxyX = new();
            List<int> galaxyY = new();
            long sum = 0;
            expansionRate--;

            foreach (Match match in galaxy.Matches(input))
            {
                int x = match.Index % lineLength;
                int y = match.Index / lineLength;
                galaxyX.Add(x);
                galaxyY.Add(y);
            }

            for (int i = 0; i < lineLength; i++)
            {
                if (galaxyX.Contains(i))
                {
                    continue;
                }

                for (int j = 0; j < galaxyX.Count; j++)
                {
                    if (galaxyX[j] > i)
                    {
                        galaxyX[j] += expansionRate;
                    }
                }
                i += expansionRate;
                lineLength += expansionRate;
            }

            for (int i = 0; i < lineCount; i++)
            {
                if (galaxyY.Contains(i))
                {
                    continue;
                }

                for (int j = 0; j < galaxyX.Count; j++)
                {
                    if (galaxyY[j] > i)
                    {
                        galaxyY[j] += expansionRate;
                    }
                }
                i += expansionRate;
                lineCount += expansionRate;
            }

            for (int i = 0; i < galaxyX.Count; i++)
            {
                for (int j = i + 1; j < galaxyY.Count; j++)
                {
                    int difX = Math.Abs(galaxyX[i] - galaxyX[j]);
                    int difY = Math.Abs(galaxyY[i] - galaxyY[j]);

                    sum += difX + difY;
                }

            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
    }
}
