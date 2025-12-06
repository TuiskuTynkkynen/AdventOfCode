using System;
using System.Reflection;

namespace AdventOfCode2025.Solutions
{
    class Day05 : ISolution
    {
        public long? Part1(TextReader input)
        {
            string? line = input.ReadLine();
            List<Range> ranges = [];

            while (line != null && line != string.Empty)
            {
                string[] bounds = line.Split("-");
                ranges.Add(new(long.Parse(bounds[0]), long.Parse(bounds[1])));
                line = input.ReadLine();
            }

            line = input.ReadLine();

            long result = 0;
            while (line != null) {
                long id = long.Parse(line);

                if(ranges.Any(range => range.Contains(id)))
                {
                    result++;
                }

                line = input.ReadLine();
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            string? line = input.ReadLine();
            List<Range> ranges = [];

            while (line != null && line != string.Empty)
            {
                string[] bounds = line.Split("-");
                List<Range> current = [new(long.Parse(bounds[0]), long.Parse(bounds[1]))];
                
                ranges.ForEach(r => {
                    current = r.Split(current); // This is so bad
                });
                ranges.AddRange(current);

                line = input.ReadLine();
            }

            return ranges.Sum(r => r.Length);
        }

        public TextReader GetExample()
        {
            return new StringReader("3-5\n10-14\n16-20\n12-18\n\n1\n5\n8\n11\n17\n32");
        }

        private struct Range(long start, long end)
        {
            public long Start = start;
            public long End = end;
            public readonly long Length => End - Start + 1;

            public readonly bool Contains(long value) => value >= Start && value <= End;

            public readonly List<Range> Split(List<Range> ranges) {
                List<Range> result = [];

                foreach (Range other in ranges)
                {   
                    bool start = Contains(other.Start);
                    bool end = Contains(other.End);

                    if (start && end)
                    {
                        continue;
                    }

                    bool both = other.Contains(Start) && other.Contains(End);

                    if (start || both)
                    {
                        result.Add(new(End + 1, other.End));
                    } 
                    
                    if (end || both)
                    {
                        result.Add(new(other.Start, Start - 1));
                    }
                    
                    if (!start && !end && !both) {
                        result.Add(other);
                    }
                }

                return result;
            }
        }
    }
}
