namespace AdventOfCode2025.Solutions
{
    class Day03 : ISolution
    {
        public long? Part1(TextReader input)
        {
            long result = 0;
            string? line = input.ReadLine();

            while (line != null)
            {
                var bank = line.Select(c => (int)char.GetNumericValue(c)).Index();
                
                var first = bank.SkipLast(1).MaxBy(x => x.Item);
                var second = bank.Skip(first.Index + 1).MaxBy(x => x.Item);

                result += first.Item * 10 + second.Item;
                line = input.ReadLine();
            }
            return result;
        }

        public long? Part2(TextReader input)
        {
            long result = 0;
            string? line = input.ReadLine();

            while (line != null)
            {
                var bank = line.Select(c => (int)char.GetNumericValue(c)).Index();

                long total = 0;
                int previousIndex = -1;

                for(int i = 11; i >= 0; i--)
                {
                    var nextLargest = bank.Skip(previousIndex + 1).SkipLast(i).MaxBy(x => x.Item);

                    total += nextLargest.Item * (long)Math.Pow(10, i);
                    previousIndex = nextLargest.Index;
                }

                result += total;
                line = input.ReadLine();
            }

            return result;
        }

        public TextReader GetExample()
        {
            return new StringReader("987654321111111\n811111111111119\n234234234234278\n818181911112111");
        }
    }
}
