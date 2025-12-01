using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day12
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-12.txt");
            Console.WriteLine("Day twelve:\t\t(this may take a while)\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string input = reader.ReadLine() ??throw new Exception("Error reading input file");
            Regex springGroups = new(@"([?]*#*)+(?:(?<!/)\.)?");
            Regex broken = new(@"#+");
            Regex numbers = new(@"[0-9]+");
            int sum = 0;

            while(input != null)
            {
                List<int> brokenLengths = numbers.Matches(input)
                                                .AsParallel()
                                                .Select(match => int.Parse(match.Value))
                                                .ToList();

                int brokenLengthCount = brokenLengths.Count();
                int totalBroken = brokenLengths.Aggregate((length, next) => length + next);

                char[] springGroup = springGroups.Matches(input)
                                                   .AsParallel()
                                                   .Select(match => match.Value)
                                                   .Aggregate((foo, next) => foo + next)
                                                   .ToArray();

                List<int> unkownIndexes = springGroup.AsParallel()
                                                      .Select((val, index) => new { val, index })
                                                      .Where((item) => item.val == '?')
                                                      .Select(item => item.index)
                                                      .ToList();


                int unknownCount = input.Count(item => item == '?');
                int permutationCount = (int)Math.Pow(2, unknownCount);
                BitArray unknowns = new(unknownCount);

                int brokenCount = springGroup.AsParallel().Where((item) => item == '#').Count();

                for (int i = 0; i < permutationCount; i++)
                {
                    for (int j = 0; j < unknownCount && !(unknowns[j] = !unknowns[j++]);) ; //Increments bit array by 1
                    int currentBroken = brokenCount;
                    
                    for (int j = 0; j < unknownCount; j++)
                    {
                        springGroup[unkownIndexes[j]] = unknowns[j] ? '#' : '.';
                        if (unknowns[j])
                        {
                            currentBroken++;
                        }
                    }

                    if(currentBroken != totalBroken)
                    {
                        continue;
                    }

                    string springs = new string(springGroup);
                    
                    List<int> brokenGroups = new();
                    foreach (Match match in broken.Matches(springs))
                    {
                        brokenGroups.Add(match.Length);
                    }
                    
                    int groupCount = brokenGroups.Count;
                    bool isValid = (groupCount == brokenLengthCount);
                    for (int j = 0; j < groupCount && isValid; j++)
                    {
                        isValid = brokenGroups[j] == brokenLengths[j];
                    }

                    if (isValid)
                    {
                        sum++;
                    }
                }

                input = reader.ReadLine();
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
