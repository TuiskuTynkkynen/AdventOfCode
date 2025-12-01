using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day9
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-09.txt");
            int result1 = SolvePuzzle(ref reader, Direction.Forward);
            int result2 = SolvePuzzle(ref reader, Direction.Backward);

            Console.WriteLine("Day nine:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int SolvePuzzle(ref StreamReader reader, Direction dir)
        {
            Regex numbers = new(@"(-?[0-9]+)");
            int sum = 0;
            
            string? input = reader.ReadLine() ?? throw new Exception("Error reading input file");
            while (input != null)
            {
                MatchCollection matches = numbers.Matches(input);
                int matchCount = matches.Count;
                int[] history = new int[matchCount];
                
                for (int i = 0; i < matchCount; i++)
                {
                    history[i] = int.Parse(matches[i].Value);
                }
                sum += Extrapolate(history, dir); 
                input = reader.ReadLine();
            }


            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
        
        private static int Extrapolate(int[] array, Direction dir)
        {

            int value = 0;
            int length = array.Length;
            int nextIndex = (dir == Direction.Forward) ? length - 1 : 0;
            if (array[0] != 0 || array[length - 1] != 0)
            {
                int[] difference = new int[length - 1];

                for (int i = 0; i < length - 1; i++)
                {
                    difference[i] = array[i + 1] - array[i];
                }

                value = array[nextIndex] +  (Extrapolate(difference, dir) * (int)dir);
            }

            return value;
        }

        enum Direction
        {
            Forward = 1,
            Backward = -1,
        }
    }
}
