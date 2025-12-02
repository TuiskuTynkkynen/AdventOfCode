namespace AdventOfCode2025.Solutions
{
    class Day01 : ISolution
    {
        public long? Part1(TextReader input)
        {
            int zeroCount = 0;
            int dialPosition = 50;

            string? line = input.ReadLine();
            while (line != null) {
                int direction = line[0] == 'L' ? -1 : 1;
                int distance = int.Parse(line.Substring(1));

                dialPosition += direction * distance;
                dialPosition %= 100;

                if (dialPosition == 0)
                {
                    zeroCount++;
                }

                line = input.ReadLine();
            }

            return zeroCount;
        }
        public long? Part2(TextReader input)
        {
            int zeroCount = 0;
            int dialPosition = 50;

            string? line = input.ReadLine();
            while (line != null)
            {
                int previousSign = int.Sign(dialPosition);
                int direction = line[0] == 'L' ? -1 : 1;
                int distance = int.Parse(line.Substring(1));

                dialPosition += direction * distance;
                
                if(previousSign != int.Sign(dialPosition))
                {
                    zeroCount += Math.Abs(previousSign);
                }

                zeroCount += Math.Abs(dialPosition) / 100;
                dialPosition %= 100;

                line = input.ReadLine();
            }

            return zeroCount;
        }

        public TextReader GetExample()
        {
            return new StringReader("L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82");
        }
    }
}