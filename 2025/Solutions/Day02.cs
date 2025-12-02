namespace AdventOfCode2025.Solutions
{
    class Day02 : ISolution
    {
        public long? Part1(TextReader input)
        {
            long result = 0;
            string[] ranges = input.ReadToEnd().Split(',');
            
            foreach(string range in ranges)
            {
                string[] bounds = range.Split('-');
                long lower = long.Parse(bounds[0]);
                long higher = long.Parse(bounds[1]);

                for (long i = lower; i < higher + 1; i++)
                {
                    string s = Convert.ToString(i);
                    
                    if(s.Length % 2 == 1)
                    {
                        continue;
                    }

                    string sub =  s.Substring(0, s.Length / 2);
                    if (s.EndsWith(sub))
                    {
                        result += i;
                    }
                }
            }
            
            return result;
        }

        public long? Part2(TextReader input)
        {
            long result = 0;
            string[] ranges = input.ReadToEnd().Split(',');

            Parallel.ForEach(ranges, range =>
            {
                string[] bounds = range.Split('-');
                long lower = long.Parse(bounds[0]);
                long higher = long.Parse(bounds[1]);

                for (long i = lower; i < higher + 1; i++)
                {
                    string s = Convert.ToString(i);

                    for (int j = 1; j < s.Length; j++)
                    {
                        if (s.Length % j != 0)
                        {
                            continue;
                        }
                        string sub = s.Substring(0, j);
                        bool found = s.Chunk(j).All(s => s.SequenceEqual(sub));

                        if (found)
                        {
                            result += i;
                            break;
                        }
                    }
                }
            });

            return result;
        }

        public TextReader GetExample()
        {
            return new StringReader("11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124");
        }
    }
}