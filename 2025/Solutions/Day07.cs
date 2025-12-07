namespace AdventOfCode2025.Solutions
{
    class Day07 : ISolution
    {
        public long? Part1(TextReader input)
        {
            string first = input.ReadLine()!;
            bool[] current = new bool[first.Length];
            current[first.IndexOf('S')] = true;

            long result = 0;
            string? line = input.ReadLine();
            while(line != null)
            {
                bool[] next = new bool[first.Length];

                var active = current.Select((b, index) => b ? index : -1).Where(i => i != -1);
                foreach (int i in active)
                {
                    int offset = line[i] == '^' ? 1 : 0;
                    
                    next[i - offset] = true;
                    next[i + offset] = true;
                    result += offset;
                }

                current = next;
                line = input.ReadLine();
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            string first = input.ReadLine()!;

            long[] beamCounts = new long[first.Length];
            beamCounts[first.IndexOf('S')] = 1;

            string? line = input.ReadLine();
            while (line != null)
            {
                for(int i = 0; i < beamCounts.Length; i++)
                {
                    if (line[i] != '^')
                    {
                        continue;
                    }

                    beamCounts[i - 1] += beamCounts[i];
                    beamCounts[i + 1] += beamCounts[i];

                    beamCounts[i] = 0;
                }

                line = input.ReadLine();
            }

            return beamCounts.Sum();
        }

        public TextReader GetExample()
        {
            return new StringReader(".......S.......\n...............\n.......^.......\n...............\n......^.^......\n...............\n.....^.^.^.....\n...............\n....^.^...^....\n...............\n...^.^...^.^...\n...............\n..^...^.....^..\n...............\n.^.^.^.^.^...^.\n...............");
        }
    }
}
