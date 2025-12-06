namespace AdventOfCode2025.Solutions
{
    internal class Day06 : ISolution
    {
        public long? Part1(TextReader input)
        {
            List<Opreration> oprerations = [];
            var lines = input.ReadToEnd().Split('\n').Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries));

            var symbols = lines.Last();
            foreach (var symbol in symbols)
            {
                oprerations.Add(new Opreration(symbol.First()));
            }

            var numbers = lines.SkipLast(1).SelectMany(s => s.Index());
            foreach (var number in numbers)
            {
                long value = long.Parse(number.Item);

                oprerations[number.Index].Invoke(value);
            }

            return oprerations.Sum(op => op.Value);
        }

        public long? Part2(TextReader input)
        {
            List<string[]> problems = [];
            var lines = input.ReadToEnd().Split('\n');

            int offset= 0;
            for (int i = 0; i < lines.First().Length; i++)
            {
                if (lines.All(s => s[i] == ' '))
                {
                    problems.Add([.. lines.Select(s => s.Substring(offset, i - offset))]);
                    offset = i + 1;
                }
            }
            // Messy
            problems.Add([.. lines.Select(s => s.Substring(offset, lines.First().Length - offset))]);

            long result = 0;
            foreach (var problem in problems)
            {
                Opreration operation = new(problem.Last().First());

                for (int i = 0; i < problem.First().Length; i++)
                {
                    long value = problem.SkipLast(1)
                        .Select(s => s[i])
                        .Where(c => c != ' ')
                        .Reverse()
                        .Index()
                        .Select(tuple => (long)(char.GetNumericValue(tuple.Item) * Math.Pow(10, tuple.Index)))
                        .Sum();

                    operation.Invoke(value);
                }

                result += operation.Value;
            }

            return result;
        }

        public TextReader GetExample()
        {
            return new StringReader("123 328  51 64 \n 45 64  387 23 \n  6 98  215 314\n*   +   *   +   ");
        }

        private class Opreration
        {
            public enum Symbol
            {
                Addition,
                Multiplication,
            }
            private readonly Symbol Type;
            public long Value;

            public Opreration(char symbol)
            {
                switch (symbol)
                {
                    case '+': 
                        Type = Symbol.Addition; 
                        break;
                    case '*':
                        Type = Symbol.Multiplication;
                        break;
                    default: 
                        throw new ArgumentException("Invalid operation symbol");
                }

                Value = (Type == Symbol.Multiplication) ? 1 : 0;
            }

            public void Invoke(long value)
            {
                switch (Type)
                {
                    case Symbol.Addition:
                        Value += value;
                        break;
                    case Symbol.Multiplication:
                        Value *= value;
                        break;
                }
            }
        }
    }
}
