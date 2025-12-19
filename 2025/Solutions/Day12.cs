namespace AdventOfCode2025.Solutions
{
    class Day12 : ISolution
    {
        public long? Part1(TextReader input)
        {
            long result = 0;
            string? line = input.ReadLine();
            while(line != null)
            {
                if (!line.Contains('x'))
                {
                    line = input.ReadLine();
                    continue;
                }

                int area = line.Substring(0, line.IndexOf(':'))
                    .Split('x', StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(1, (res, s) => res * int.Parse(s));
                int presentCount = line.Substring(line.IndexOf(':') + 1)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).Sum();

                if (area >= presentCount * 9)
                {
                    result++;
                }

                line = input.ReadLine();
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            return null;
        }

        public TextReader GetExample()
        {
            return new StringReader("4x4: 0 0 0 0 2 0\n12x5: 1 0 1 0 2 2\n12x5: 1 0 1 0 3 2");
        }
    }
}
