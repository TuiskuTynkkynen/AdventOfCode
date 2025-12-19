using System.Numerics;

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

                int[] buttons = line.Substring(line.IndexOf('(') + 1, line.LastIndexOf(')') - line.IndexOf('(') - 1)
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

        public long? Part2(TextReader input)
        {
            long result = 0;
            string? line = input.ReadLine();
            while (line != null)
            {
                int[] joltages = [.. line.Substring(line.IndexOf('{') + 1, line.IndexOf('}') - line.IndexOf('{') - 1)
                    .Split(",")
                    .Select(int.Parse)];

                int[][] buttons = [.. line.Substring(line.IndexOf('(') + 1, line.LastIndexOf(')') - line.IndexOf('(') - 1)
                    .Split(") (")
                    .Select(s =>
                        s.Split(",")
                        .Select(int.Parse)
                        .ToArray())];

                List<LinearEquation> matrix = [.. 
                    Enumerable.Range(0, joltages.Length)
                    .Select(i 
                        => new LinearEquation([.. Enumerable.Range(0, buttons.Length).Select(j => Convert.ToInt32(buttons[j].Contains(i)))]
                        , joltages[i]))];

                LinearEquation.GaussianElimination(matrix);

                List<Equation> eliminated = [];
                Equation negatives = new();
                
                List<Equation> inverse = [.. matrix.Select(eq => new Equation(eq))];
                while (inverse.Count != 0)
                {
                    var smallest = inverse.MinBy(eq => eq.Free.Count)!;
                    inverse.Remove(smallest);

                    if(smallest.Free.Count == 0)
                    {
                        continue;
                    }

                    smallest.Reduce();
                    eliminated.Add(smallest);

                    foreach (Equation eq in inverse)
                    {
                        foreach(var (variable, _) in smallest.Free)
                        {
                            eq.Bind(variable);
                        }
                    }

                    if (smallest.Free.Count == 1) continue;

                    foreach (var kv in smallest.Free.ToArray())
                    {
                        if (kv.Coefficient >= 0)
                        {
                            continue;
                        }

                        smallest.Free.RemoveAll(b => b.Index == kv.Index);
                        smallest.Bound.Add(kv);
                        negatives.Free.Add(new(kv.Index, 1));
                    }
                }

                for (int i = 1; i < eliminated.Count; i++)
                {
                    var eq = eliminated[i];
                    for (int j = 0; j < i; j++)
                    {
                        if (eq.Bound.Count == 0) 
                        {
                            break; 
                        }

                        var all = eliminated[j].Free.Concat(eliminated[j].Bound);
                        if (!all.All(a => eq.Bound.Where(kv => kv.Index == a.Index).Any(kv => kv.Coefficient == a.Coefficient)))
                        {
                            continue;
                        }

                        eq.Bound.RemoveAll(kv => all.Any(a => a.Index == kv.Index));
                        eq.Result -= eliminated[j].Result;
                            
                        if (eq.Result == 0)
                        {
                            eliminated.RemoveAt(i--);
                        }
                    }
                }

                var lockObject = new Object();
                int min = joltages.Sum();
                long negativeIndex = 0;
                long negativeCount = 0;
                bool done = false;

                int[] initial = [.. Enumerable.Repeat(0, buttons.Length)];
                while(!done)
                { 
                    if(!negatives.TrySolve(initial, negativeCount, negativeIndex++))
                    {
                        continue;
                    }

                    long totalSolutionCount = eliminated.Select(eq => eq.MaxSolutionCount(initial)).Aggregate((long)1, (res, c) => res *= c);
                    Parallel.For(0, totalSolutionCount, (s, p) =>
                    {
                        int[] presses = [.. initial ];
                        long solutionIndex = s;

                        foreach (var eq in eliminated)
                        {
                            long target = eq.CalcuteTarget(presses);

                            long max = eq.SolutionCount(target);
                            var (quotient, remainder) = long.DivRem(solutionIndex, max);

                            solutionIndex = quotient;
                            bool solution = eq.TrySolve(presses, target, remainder);

                            if (!solution)
                            {
                                return;
                            }
                        }

                        int pressCount = presses.Sum();
                        if (pressCount >= min)
                        {
                            return;
                        }

                        int[] result = new int[joltages.Length];
                        for(int i = 0; i < presses.Length; i++)
                        {
                            foreach(int b in buttons[i])
                                result[b] += presses[i];
                        }

                        for (int i = 0; i < joltages.Length; i++)
                        {
                            if (result[i] != joltages[i])
                            {
                                return;
                            }
                        }

                        lock (lockObject)
                        {
                            min = int.Min(min, pressCount);
                        }
                    });

                    long solutionCount = negatives.SolutionCount(negativeCount);
                    if (negativeCount != 0 && solutionCount >= negativeIndex)
                    {
                        continue;
                    }

                    if(solutionCount != 1)
                    {
                        negativeIndex = 0;
                    }

                    done = negatives.Free.Count == 0 || negativeCount >= min / 4;
                    negativeCount++;
                }

                result += min;
                line = input.ReadLine();
            }

            return result;
        }

        public TextReader GetExample()
        {
            return new StringReader("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}");    
        }
    }
}
internal class Equation
{
    public List<(int Index, int Coefficient)> Free = [];
    public List<(int Index, int Coefficient)> Bound = [];
    public int Result;

    public Equation() { }

    public Equation(LinearEquation vectorForm)
    {
        Result = vectorForm.Result;

        for (int i = 0; i < vectorForm.Variables.Length; i++)
        {
            if (vectorForm.Variables[i] != 0)
            {
                Free.Add(new(i, vectorForm.Variables[i]));
            }
        }

    }

    public void Bind(int variable)
    {
        int index = Free.FindIndex(0, kv => kv.Index == variable);
        if (index == -1)
        {
            return;
        }

        Bound.Add(Free[index]);
        Free.RemoveAt(index);
    }

    public void Reduce()
    {
        int gdc = Result;
        int negativeCount = Result < 0 ? 1 : 0;
        int positiveCount = 1 - negativeCount;
        foreach ((_, int coefficient) in Free)
        {
            gdc = (int)BigInteger.GreatestCommonDivisor(gdc, coefficient);
            negativeCount += coefficient < 0 ? 1 : 0;
            positiveCount += coefficient < 0 ? 0 : 1;
        }

        gdc *= negativeCount > positiveCount ? -1 : 1;
        if (gdc == 1)
        {
            return;
        }

        foreach ((_, int coefficient) in Bound)
        {
            gdc = (int)BigInteger.GreatestCommonDivisor(gdc, coefficient);
        }

        Result /= gdc;
        for (int i = 0; i < Free.Count; i++)
        {
            Free[i] = new(Free[i].Index, Free[i].Coefficient / gdc);
        }
        for (int i = 0; i < Bound.Count; i++)
        {
            Bound[i] = new(Bound[i].Index, Bound[i].Coefficient / gdc);
        }
    }

    public int CalcuteTarget(int[] solved)
    {
        int target = Result;
        foreach (var (index, coefficient) in Bound)
        {
            target -= solved[index] * coefficient;
        }
        return target;
    }

    public long SolutionCount(long target) => (Free.Count > 1) ? (long)Math.Pow(long.Abs(target) + 1, Free.Count - 1) : 1;

    public bool TrySolve(int[] solved, long target, long solutionIndex)
    {
        long sum = 0;
        solutionIndex = (solutionIndex + 1) * long.Abs(target);

        foreach (var (index, coefficient) in Free)
        {
            var (quotient, remainder) = long.DivRem(solutionIndex, long.Abs(target) + 1);

            solved[index] = (int)remainder / int.Abs(coefficient);
            sum += (coefficient < 0 ? -1 : 1) * remainder;
            solutionIndex = quotient;
        }

        return sum == target;
    }

    public long MaxSolutionCount(int[] maximums)
    {
        int target = Result - Bound.Where(b => b.Coefficient < 0).Select(b => maximums[b.Index] * b.Coefficient).Sum();
        foreach (var (index, coefficient) in Free)
        {
            maximums[index] = target / int.Abs(coefficient);
        }
        return SolutionCount(target);
    }
}

internal class LinearEquation(int[] coefficients, int results)
{
    public int Result = results;
    public int[] Variables = coefficients;

    public static void GaussianElimination(List<LinearEquation> equations)
    {
        int rows = equations.Count;
        int columns = equations.First().Variables.Length;

        for (int h = 0, k = 0; h < rows && k < columns; k++)
        {
            int i_max = equations.Index()
                .Skip(h)
                .MaxBy(x => Math.Abs(x.Item.Variables[k]))
                .Index;

            if (equations[i_max].Variables[k] == 0)
            {
                continue;
            }

            (equations[h], equations[i_max]) = (equations[i_max], equations[h]);

            for (int i = h + 1; i < rows; i++)
            {
                int a = equations[i].Variables[k];

                if (a == 0)
                {
                    continue;
                }

                int b = equations[h].Variables[k];
                int lcm = a / (int)BigInteger.GreatestCommonDivisor(a, b) * b;
                int scale_i = lcm / a;
                int scale_h = lcm / b;

                equations[i].Variables[k] = 0;
                for (int j = k + 1; j < columns; j++)
                {
                    equations[i].Variables[j] = scale_i * equations[i].Variables[j] - equations[h].Variables[j] * scale_h;
                }
                equations[i].Result = equations[i].Result * scale_i - equations[h].Result * scale_h;
            }

            h++;
        }
    }
}