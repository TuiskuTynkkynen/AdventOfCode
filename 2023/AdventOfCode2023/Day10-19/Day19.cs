using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AdventOfCode2023
{
    internal static class Day19
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-19.txt");
            Console.WriteLine("Day nineteen:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");
            Dictionary<char, int> partRatingIndexes = new()
            {
                {'x', 0},
                {'m', 1},
                {'a', 2},
                {'s', 3},
            };
            Regex numbers = new(@"[0-9]+");
            int sum = 0;

            string[] workflowStrings = input.Split(Environment.NewLine + Environment.NewLine)[0].Split(Environment.NewLine);
            string[] partStrings = input.Split(Environment.NewLine + Environment.NewLine)[1].Split(Environment.NewLine);
            Dictionary<string, string[]> workflows = new();
            List<int[]> parts = new();

            foreach (string workflowString in workflowStrings)
            {
                string name = workflowString.Split("{")[0];
                string[] rules = workflowString.Split("{")[1].Split("}")[0].Split(new char[] {',', ':'});
                workflows.Add(name, rules);
            }
            
            foreach (string part in partStrings)
            {
                MatchCollection matches = numbers.Matches(part);
                if(matches.Count == 0)
                {
                    continue;
                }
                int x = int.Parse(matches[0].Value);
                int m = int.Parse(matches[1].Value);
                int a = int.Parse(matches[2].Value);
                int s = int.Parse(matches[3].Value);
                parts.Add(new int[] { x, m, a, s });
            }
            
            foreach(int[] part in parts)
            {
                string name = "in";
                while(name != "A" && name != "R")
                {
                    workflows.TryGetValue(name, out string[]? rule);
                    if(rule == null)
                    {
                        break;
                    }

                    int ruleIndex = 0;
                    while (ruleIndex != -1)
                    {
                        partRatingIndexes.TryGetValue(rule[ruleIndex][0], out int ratingIndex);
                        
                        if(rule[ruleIndex].Length <= 1)
                        {
                            name = rule[ruleIndex];
                            break;
                        }

                        if (rule[ruleIndex][1] == '<')
                        {
                            int ruleValue = int.Parse(numbers.Match(rule[ruleIndex]).Value);
                            if (part[ratingIndex] < ruleValue)
                            {
                                ruleIndex++;
                            } 
                            else
                            {
                                ruleIndex += 2;
                            }
                        } 
                        else if (rule[ruleIndex][1] == '>')
                        {
                            int ruleValue = int.Parse(numbers.Match(rule[ruleIndex]).Value);
                            if (part[ratingIndex] > ruleValue)
                            {
                                ruleIndex++;
                            }
                            else
                            {
                                ruleIndex += 2;
                            }
                        }
                        else
                        {
                            name = rule[ruleIndex];
                            break;
                        }
                    }
                }

                if(name == "A")
                {
                    sum += part.Aggregate((val, next) => val + next);
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            string[] workflowStrings = reader.ReadToEnd()
                                             .Split(Environment.NewLine + Environment.NewLine)[0]
                                             .Split(Environment.NewLine)
                                             ?? throw new Exception("Error reading input file");
            Dictionary<char, int> partRatingIndexes = new()
            {
                {'x', 0},
                {'m', 2},
                {'a', 4},
                {'s', 6},
            };
            Dictionary<string, string[]> workflows = new();
            Regex numbers = new(@"[0-9]+");
            long sum = 0;

            int min = 1;
            int max = 4000;
            List<(string name, int index, int[] ranges)> parts = new() {
                ("in", 0, new int[] {min, max, min, max, min, max, min, max})
            };


            foreach (string workflowString in workflowStrings)
            {
                string name = workflowString.Split("{")[0];
                string[] rules = workflowString.Split("{")[1].Split("}")[0].Split(new char[] { ',', ':' });
                workflows.Add(name, rules);
            }

            while (parts.Any())
            {
                int[] part = parts.Last().ranges;
                string name = parts.Last().name;
                int ruleIndex = parts.Last().index;
                parts.RemoveAt(parts.Count - 1);


                if (name == "A")
                {
                Console.WriteLine();
                Console.WriteLine($"{name}, {ruleIndex}, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]}):");
                    long sumX = part[1] - part[0];
                    long sumM = part[3] - part[2];
                    long sumA = part[5] - part[4];
                    long sumS = part[7] - part[6];

                    Console.WriteLine(sumX * sumM * sumA * sumS);
                    if (sumX < 0 || sumM < 0 || sumA < 0 || sumS < 0)
                    {
                        Console.WriteLine("GGGGGGGGGGGGGGGGGGGGGGGGGG");
                    }
                    sum += sumX * sumM * sumA * sumS;
                    continue;
                }

                if(name == "R")
                {
                    continue;
                }

                workflows.TryGetValue(name, out string[]? rule);
                if (rule == null)
                {
                    break;
                }

                partRatingIndexes.TryGetValue(rule[ruleIndex][0], out int ratingIndex);

                if (rule[ruleIndex].Length <= 1)
                {
                    //Console.WriteLine($"added: {rule[ruleIndex]}, -1, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]} )");
                    parts.Add((rule[ruleIndex], -1, part));
                    continue;
                }

                int rangeStart = part[ratingIndex];
                int rangeEnd = part[ratingIndex + 1];

                if (rule[ruleIndex][1] == '<')
                {
                    int ruleValue = int.Parse(numbers.Match(rule[ruleIndex]).Value);

                    if (rangeEnd < ruleValue) //if whole range matches rule
                    {
                        parts.Add((name, ruleIndex + 1, part));
                        //Console.WriteLine("Whole range matches");
                    }
                    else if (rangeStart < ruleValue && rangeEnd >= ruleValue) //if some values match rule
                    {
                        part[ratingIndex + 1] = ruleValue - 1;
                        //Console.WriteLine($"added: {name}, {ruleIndex + 1}, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]} )");
                        parts.Add((name, ruleIndex + 1, part)); //matching part(start = start, end = rule - 1)

                        part[ratingIndex] = ruleValue;
                        part[ratingIndex + 1] = rangeEnd;
                        //Console.WriteLine($"added: {name}, {ruleIndex + 2}, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]} )");
                        parts.Add((name, ruleIndex + 2, part)); //non matching part(start = rule, end = end)
                    }
                    else //if whole range doesn't match rule
                    {
                        parts.Add((name, ruleIndex + 2, part));
                        //Console.WriteLine("Whole range doesn't match");
                    }
                }
                else if (rule[ruleIndex][1] == '>')
                {
                    int ruleValue = int.Parse(numbers.Match(rule[ruleIndex]).Value);

                    if (rangeStart > ruleValue) //if whole range matches rule
                    {
                        parts.Add((name, ruleIndex + 1, part));
                        //Console.WriteLine("Whole range matches");
                    }
                    else if (rangeStart <= ruleValue && rangeEnd > ruleValue) //if some values match rule
                    {
                        part[ratingIndex + 1] = ruleValue;
                        //Console.WriteLine($"added: {name}, {ruleIndex + 2}, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]} )");
                        parts.Add((name, ruleIndex + 2, part)); //non matching part(start = start, end = rule)

                        part[ratingIndex] = ruleValue + 1;
                        part[ratingIndex + 1] = rangeEnd;
                        //Console.WriteLine($"added: {name}, {ruleIndex + 1}, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]} )");
                        parts.Add((name, ruleIndex + 1, part)); //matching part(start = rule + 1, end = end)
                    }
                    else //if whole range doesn't match rule
                    {
                        parts.Add((name, ruleIndex + 2, part));
                      //  Console.WriteLine("Whole range doesn't match");
                    }
                }
                else
                {
                    //Console.WriteLine($"added: {rule[ruleIndex]}, 0, ({part[0]} - {part[1]}, {part[2]} - {part[3]}, {part[4]} - {part[5]}, {part[6]} - {part[7]} )");
                    parts.Add((rule[ruleIndex], 0, part));
                    continue;
                }
            }

            Console.WriteLine(sum);

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
