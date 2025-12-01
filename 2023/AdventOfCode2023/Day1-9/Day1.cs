using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day1
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-01.txt");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Day one:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            int sum = 0;
            char firstnumber, lastnumber;
            string wholenumber;

            string? line = reader.ReadLine();
            while (line != null)
            {
                firstnumber = line[line.IndexOfAny(numbers)];
                lastnumber = line[line.LastIndexOfAny(numbers)];

                wholenumber = String.Concat(firstnumber, lastnumber);
                sum += Int32.Parse(wholenumber);

                line = reader.ReadLine();
            }


            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            Dictionary<string, int> numbers = new Dictionary<string, int> {
                { "1", 1},
                { "2", 2},
                { "3", 3},
                { "4", 4},
                { "5", 5},
                { "6", 6},
                { "7", 7},
                { "8", 8},
                { "9", 9},
                { "one", 1},
                { "two", 2},
                { "three", 3},
                { "four", 4},
                { "five", 5},
                { "six", 6},
                { "seven", 7},
                { "eight", 8},
                { "nine", 9},
            };

            int sum = 0;
            string? line = reader.ReadLine();
            string wholenumber;

            while (line != null)
            {
                int[] firstnumber = { Int32.MaxValue, -1 };
                int[] lastnumber = { -1, -1 };
                
                foreach (KeyValuePair<string, int> entry in numbers)
                {
                    int index = line.IndexOf(entry.Key);
                    
                    if(index != -1 && index < firstnumber[0])
                    {
                        firstnumber[0] = index;
                        firstnumber[1] = entry.Value;
                    }

                    index = line.LastIndexOf(entry.Key);
                    if(index != -1 && index > lastnumber[0])
                    {
                        lastnumber[0] = index;
                        lastnumber[1] = entry.Value;
                    }
                }

                wholenumber = String.Concat(firstnumber[1], lastnumber[1]);
                sum += Int32.Parse(wholenumber);
                line = reader.ReadLine();
            }


            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
    }
}
