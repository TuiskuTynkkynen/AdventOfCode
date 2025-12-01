using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day15
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-15.txt");
            Console.WriteLine("Day fifteen:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            List<string> input = reader.ReadToEnd().Split(',').ToList() ?? throw new Exception("Error reading input file");
            Regex comma = new(@",");
            int sum = 0;
            int multiplier = 17;
            int devider = 256;
            
            while (input.Any())
            {
                string step = comma.Replace(input.Last(), string.Empty);
                input.RemoveAt(input.Count - 1);
                int result = 0;
                foreach(char c in step) { 
                    result += c;
                    result *= multiplier;
                    result %= devider;
                }

                sum += result;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            List<string> input = reader.ReadToEnd().Split(',').ToList() ?? throw new Exception("Error reading input file");
            Regex comma = new(@",");
            Regex operation = new(@"=|-");
            int multiplier = 17;
            int boxCount = 256;
            OrderedDictionary[] boxes = new OrderedDictionary[boxCount];
            for (int i = 0; i < boxCount; i++)
            {
                boxes[i] = new OrderedDictionary();
            }

            while (input.Any())
            {
                string step = comma.Replace(input.First(), string.Empty);
                input.RemoveAt(0);
                int index = operation.Match(step).Index;
                string label = step.Substring(0, index);

                int box = 0;
                foreach (char c in label)
                {
                    box += c;
                    box *= multiplier;
                    box %= boxCount;
                }

                if(step[index] == '=')
                {
                    boxes[box][label] = step[index + 1];
                }
                else
                {
                    boxes[box].Remove(label);
                }
            }

            int sum = 0;
            for (int i = 0; i < boxCount; i++)
            {
                int count = boxes[i].Count;
                char[] values = new char[count];
                boxes[i].Values.CopyTo(values, 0);
                
                for (int j = 0; j < count; j++)
                {
                    sum += (i + 1) * (j + 1) * (values[j] - '0');
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
    }
}
