using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day3
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-03.txt");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Day two:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split('\n');
            Regex symbols = new Regex(@"([-@*=%/$#+&]+)");
            Regex numbers = new Regex(@"([0-9]+)");
            int sum = 0;

            for (int y = 0; y < input.Length; y++)
            {
                foreach (Match match in numbers.Matches(input[y]))
                {
                    string lines = "";
                    int startIndexX = (match.Index - 1 >= 0) ? match.Index - 1 : 0;
                    int endIndexX = (match.Index + match.Length + 1 < input[y].Length) ? match.Index + match.Length + 1 : input[y].Length - 1;
                    int startIndexY = (y - 1 >= 0) ? y - 1 : 0;
                    int endIndexY = (y + 1 < input[y].Length) ? y + 1 : input[y].Length - 1;

                    lines = String.Concat(
                        input[startIndexY].Substring(startIndexX, endIndexX - startIndexX),
                        input[endIndexY].Substring(startIndexX, endIndexX - startIndexX),
                        input[y].Substring(startIndexX, 1),
                        input[y].Substring(endIndexX - 1, 1)
                    );
                    
                    if (symbols.IsMatch(lines))
                    {
                        sum += Int32.Parse(match.Value);
                    }
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split('\n');
            int[][] directions = new int[][]{
                new int[]{ -1, -1 },
                new int[]{ -1, 0 },
                new int[]{ -1, 1 },
                new int[]{ 0, 1 },
                new int[]{ 0, -1 },
                new int[]{ 1, -1 },
                new int[]{ 1, 0 },
                new int[]{ 1, 1 },
            };
            Regex symbols = new Regex(@"([*]+)");
            Regex numbers = new Regex(@"([0-9]+)");
            int sum = 0;

            for (int y = 0; y < input.Length; y++)
            {
                foreach (Match match in symbols.Matches(input[y]))
                {
                    string line = "";
                    HashSet<int[]> valueIndexes = new HashSet<int[]>();

                    for (int i = -1; i <= 1; i++)
                    {
                        int indexY = y + i;
                        line += ".";
                        
                        for (int j = -1; j <= 1; j++)
                        {
                            int indexX = match.Index + j;
                            if (indexY >= 0 && indexY < input.Length && indexX >= 0 && indexX < input[indexY].Length)
                            {
                                line += input[indexY][indexX];
                                valueIndexes.Add(new int[] { indexY, indexX });
                            }
                        }
                    }

                    HashSet<int> values = new HashSet<int>();
                    if (numbers.Matches(line).Count == 2)
                    {
                        foreach (int[] index in valueIndexes)
                        {
                            int indexY = index[0];
                            int indexX = index[1];
                            foreach (Match numberMatch in numbers.Matches(input[indexY]))
                            {
                                if (numberMatch.Index <= indexX && numberMatch.Index + numberMatch.Length > indexX) {
                                    values.Add(Int32.Parse(numberMatch.Value));
                                }
                            }
                        }
                        sum += values.Aggregate(1, (total, next) => total * next);
                        values.Clear();
                    }
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
    }
}
