using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace AdventOfCode2025.Solutions
{
    class Day10 : ISolution
    {
        public long? Part1(TextReader input)
        {
            long result = 0;
            
            string[] lines = input.ReadToEnd().Split("\n");
            Parallel.ForEach(lines, line =>  {
                int lights = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1)
                    .Index()
                    .Aggregate(0, (r, c) => r |= Convert.ToInt32(c.Item == '#') << c.Index);

                int[] buttons = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - line.IndexOf("(") - 1)
                    .Split(") (")
                    .Select(s =>
                        s.Split(",")
                        .Aggregate(0, (r, s) => r |= 1 << int.Parse(s)))
                    .ToArray();

                
                long res = long.MaxValue;
                for(int i = 1; i < 1 << (buttons.Length); i++)
                {   
                    static int IsBitSet(int value, int bitIndex) => value >> bitIndex & 1;

                    int state = buttons.Index()
                        .Aggregate(0, (state, b) => state ^= IsBitSet(i, b.Index) * b.Item);
                   
                    if(state == lights)
                    {
                        res = long.Min(res, int.PopCount(i));
                    }
                }

                result += res;
            });

            return result;
        }

        private class Equation(List<int> variables, long result)
        {
            public List<int> Free = variables;
            public List<int> Bound = [];
            public long Result = result;

            public void Bind(int variable)
            {
                if(Free.Remove(variable))
                    Bound.Add(variable);
            }

            public long CalcuteTarget(List<int> solved)
            {
                return Result - Bound.Select(b => solved[b]).Sum();
            }

            public long MaxSolutionCount(long target) => (Free.Count > 1) ? (long)Math.Pow(target + 1, Free.Count - 1) : 1;

            public bool TrySolve(List<int> solved, long target, long solutionIndex)
            {
                long sum = 0;
                solutionIndex = (solutionIndex + 1) * target;

                for (int i = 0; i < Free.Count; i++) {
                    var res = long.DivRem(solutionIndex, target + 1);

                    sum += solved[Free[i]] = (int)res.Remainder;
                    solutionIndex = res.Quotient;
                }

                return sum == target;
            }
        }

        public long? Part2(TextReader input)
        {
            long result = 0;
            int c = 0;

            string? line = input.ReadLine();
            while(line != null) 
            {
                Console.WriteLine(line);
                int[] joltages = line.Substring(line.IndexOf('{') + 1, line.IndexOf('}') - line.IndexOf('{') - 1)
                    .Split(",")
                    .Select(s => int.Parse(s))
                    .ToArray();

                int[][] buttons = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - line.IndexOf("(") - 1)
                    .Split(") (")
                    .Select(s =>
                        s.Split(",")
                        .Select(i => int.Parse(i))
                        .ToArray())
                    .ToArray();

                line = input.ReadLine();

                List<Equation> inverse = [.. Enumerable.Range(0, joltages.Length).Select(i => new Equation(buttons.Index().Where(b => b.Item.Contains(i)).Select(b => b.Index).ToList(), joltages[i]))];

                foreach(Equation eq in inverse)
                {
                    Console.WriteLine(string.Join(", ", eq.Free));
                }
                Console.WriteLine();

                long totalEntropy = 1;

                List<Equation> eliminated = [];
                while(inverse.Count != 0)
                {
                    var smallest = inverse.MinBy(eq => eq.Free.Count)!;
                    inverse.Remove(smallest);

                    foreach (Equation eq in inverse)
                    {
                        smallest.Free.ForEach(eq.Bind);
                    }

                    if(smallest.Free.Count != 0)
                        totalEntropy *= smallest.Free.Count;

                    eliminated.Add(smallest);
                }


                foreach(var eq in eliminated)
                {
                    Console.WriteLine($"Jolts = {eq.Result}, Free = {string.Join(", ", eq.Free)}, Bound = {string.Join(", ", eq.Bound)}");
                }

                for(int i = 1; i < eliminated.Count; i++)
                {
                    var eq = eliminated[i];
                    for(int j = 0; j < i; j++)
                    {
                        var all = eliminated[j].Free.Concat(eliminated[j].Bound);
                        
                        if (all.All(eq.Bound.Contains)) {
                            eq.Bound.RemoveAll(all.Contains);
                            eq.Result -= eliminated[j].Result;
                            
                            if(eq.Result == 0)
                            {
                                eliminated.RemoveAt(i--);
                            }
                        }
                    }
                }

                Console.WriteLine("\n");
                foreach (var eq in eliminated)
                {
                    Console.WriteLine($"Jolts = {eq.Result}, Free = {string.Join(", ", eq.Free)}, Bound = {string.Join(", ", eq.Bound)}");
                }

                for (int i = 1; i < eliminated.Count; i++)
                {
                    bool inserted = false;
                    var eq = eliminated[i];
                    
                    if(eq.Free.Count != 0)
                    {
                        continue;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        if (!eq.Bound.All(eliminated[j].Free.Contains))
                        {
                            continue;
                        }
                        
                        eliminated[j].Free.RemoveAll(eq.Bound.Contains);
                        eliminated[j].Result -= eq.Result;

                        if (!inserted)
                        {
                            eliminated.RemoveAt(i);
                            eliminated.Insert(j++, new Equation(eq.Bound, eq.Result));
                            inserted = true;
                        }
                    }
                }

                Console.WriteLine("\n");
                foreach (var eq in eliminated)
                {
                    Console.WriteLine($"Jolts = {eq.Result}, Free = {string.Join(", ", eq.Free)}, Bound = {string.Join(", ", eq.Bound)}");
                }

                int min = int.MaxValue;
                long totalSolutionCount = eliminated.Select(eq => eq.MaxSolutionCount(eq.Result)).Aggregate((long)1, (res, c) => res *= c);

                Console.WriteLine(string.Join(", ", eliminated.Select(eq => eq.MaxSolutionCount(eq.Result))));
                Console.WriteLine(totalSolutionCount);

                Parallel.For((long)0, totalSolutionCount, s =>
                {
                    List<int> presses = [.. Enumerable.Repeat(-1, buttons.Length)];
                    long solutionIndex = s;

                    foreach (var eq in eliminated)
                    {
                        long target = eq.CalcuteTarget(presses);

                        if (target < 0)
                        {
                            return;
                        }

                        long max = eq.MaxSolutionCount(target);
                        var bar = long.DivRem(solutionIndex, max);

                        solutionIndex = bar.Quotient;
                        bool solution = eq.TrySolve(presses, target, bar.Remainder);

                        if (!solution)
                        {
                            return;
                        }
                    }

                    min = int.Min(min, presses.Sum());
                });
                Console.WriteLine(min);

                if (min >= joltages.Sum())
                {
                    return null;
                }
                result += min;
                c++;
                Console.WriteLine((c * 100.0f) / 180);
            }
            return result;
        }

        public TextReader GetExample()
        {
            return new StringReader("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}");    
        }
    }
}
