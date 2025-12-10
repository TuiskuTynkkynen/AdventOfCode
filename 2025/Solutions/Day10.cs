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

        public long? Part2(TextReader input)
        {
            return null;
        }

        public TextReader GetExample()
        {
            return new StringReader("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}");    
        }
    }
}
