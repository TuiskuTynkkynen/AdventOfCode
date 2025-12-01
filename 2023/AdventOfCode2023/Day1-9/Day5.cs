using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day5
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-05.txt");
            uint result1 = Part1(ref reader);
            long result2 = Part2(ref reader);

            Console.WriteLine("Day five:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static uint Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(':');
            Regex numbers = new Regex(@"([0-9]+)");
            
            MatchCollection seedMatches = numbers.Matches(input[1]);
            int seedCount = seedMatches.Count;
            uint[][] seeds = new uint[2][];
            seeds[0] = new uint[seedCount];
            seeds[1] = new uint[seedCount];

            for (int i = 0; i < seedCount; i++)
            {
                seeds[0][i] = UInt32.Parse(seedMatches[i].Value);
                seeds[1][i] = 0;
            }
            
            for (int i = 2; i < input.Length; i++)
            {
                string[] maps = input[i].Split("\n");
                for(int j = 1; j < maps.Length; j++)
                {
                    MatchCollection mapMatches = numbers.Matches(maps[j]);
                    if (mapMatches.Count == 0)
                    {
                        continue;
                    }
                    uint destinationRangeStart = UInt32.Parse(mapMatches[0].Value);
                    uint sourceRangeStart = UInt32.Parse(mapMatches[1].Value);
                    uint rangeLength = UInt32.Parse(mapMatches[2].Value);
                    uint offset = destinationRangeStart - sourceRangeStart;

                    for (int k = 0; k < seedCount; k++)
                    {
                        if (seeds[1][k] != i && sourceRangeStart <= seeds[0][k] && sourceRangeStart + rangeLength > seeds[0][k])
                        {
                            seeds[0][k] += offset;
                            seeds[1][k] = (uint)i;
                        }
                    }
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return seeds[0].Min();
        }
        
        private static long Part2(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(':');
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            Regex numbers = new Regex(@"([0-9]+)");
            int mapCount = input.Length - 2;
            MatchCollection seedRangeMatches = numbers.Matches(input[1]);
            int seedRangeCount = seedRangeMatches.Count;

            List<SeedRange> seedRanges = new List<SeedRange>();
            for (int i = 0; i < seedRangeCount; i += 2)
            {
                seedRanges.Add(new SeedRange
                {
                    rangeStart = UInt32.Parse(seedRangeMatches[i].Value),
                    rangeLength = UInt32.Parse(seedRangeMatches[i + 1].Value)
                });
            }
            seedRanges.Sort();

            List<MapRange>[] maps = new List<MapRange>[mapCount];
            for (int i = 0; i < mapCount; i++)
            {
                maps[i] = new List<MapRange>();
                foreach (string map in input[i + 2].Split("\n"))
                {
                    MatchCollection mapMatches = numbers.Matches(map);
                    if (mapMatches.Count == 0)
                    {
                        continue;
                    }
                    maps[i].Add(new MapRange{
                        destinationRangeStart = UInt32.Parse(mapMatches[0].Value),
                        sourceRangeStart = UInt32.Parse(mapMatches[1].Value),
                        rangeLength = UInt32.Parse(mapMatches[2].Value),
                    });
                }
                maps[i].Sort();
            }

            List<long> results = new List<long>();
            foreach (SeedRange seedRange in seedRanges)
            {
                PriorityQueue<Range, long> ranges = new PriorityQueue<Range, long>();
                ranges.Enqueue(
                    new Range(
                              seedRange.rangeStart,
                              seedRange.rangeStart + seedRange.rangeLength,
                              -1
                             ),
                    seedRange.rangeStart
                );

                while (ranges.Count > 0)
                {
                    ranges.TryDequeue(out Range range, out long origin);
                    short rangeMap = (short)(range.lastMap + 1);

                    if (rangeMap >= mapCount)
                    {
                        int count = seedRanges.AsParallel()
                                              .Where(item =>
                                                    item.rangeStart <= origin
                                                    && item.rangeStart + item.rangeLength > origin
                                              )
                                              .Count();
                        if(count > 0)
                        {
                            results.Add(range.start);
                        }
                        continue;
                    }

                    List<MapRange> mapRanges = maps[rangeMap].AsParallel()
                                           .Where(item =>
                                                    item.sourceRangeStart <= range.end
                                                    && item.sourceRangeStart + item.rangeLength >= range.start
                                                 )
                                           .ToList();

                    int mapRangeCount = mapRanges.Count;

                    if (mapRangeCount <= 0)
                    {
                        ranges.Enqueue(new Range(range.start, range.end, rangeMap), origin);
                        continue;
                    }

                    long lastRangeEnd = range.start;
                    long lastOrigin = origin;
                    for (int i = 0; i < mapRangeCount; i++)
                    {

                        if (lastRangeEnd < mapRanges[i].sourceRangeStart)
                        {
                            ranges.Enqueue(new Range(lastRangeEnd, mapRanges[i].sourceRangeStart, rangeMap), lastOrigin);
                            lastOrigin += mapRanges[i].sourceRangeStart - lastRangeEnd;
                        }

                        long start = mapRanges[i].destinationRangeStart;
                        long end = mapRanges[i].destinationRangeStart + mapRanges[i].rangeLength;

                        if (range.start > mapRanges[i].sourceRangeStart)
                        {
                            long offset = range.start - mapRanges[i].sourceRangeStart;
                            start = start + offset;
                        }

                        if (range.end < mapRanges[i].sourceRangeStart + mapRanges[i].rangeLength)
                        {
                            long offset = range.end - mapRanges[i].sourceRangeStart;
                            end = mapRanges[i].destinationRangeStart + offset;
                        }

                        ranges.Enqueue(new Range(start, end, rangeMap), lastOrigin);
                        lastRangeEnd = mapRanges[i].sourceRangeStart + end - start;
                        lastOrigin += end - start;
                    }
                    
                    if (range.end > mapRanges[mapRangeCount - 1].sourceRangeStart + mapRanges[mapRangeCount - 1].rangeLength)
                    {
                        long start = mapRanges[mapRangeCount - 1].sourceRangeStart + mapRanges[mapRangeCount - 1].rangeLength;
                        ranges.Enqueue(new Range(start, range.end, rangeMap), lastOrigin);
                    }

                }
            }
            
            return results.Min();
        }

        private struct Range
        {
            public long start;
            public long end;
            public short lastMap;

            public Range(long s, long e, short lm)
            {
                start = s;
                end = e;
                lastMap = lm;
            }
        }
        
        private struct SeedRange : IComparable
        {
            public long rangeStart;
            public long rangeLength;

            public int CompareTo(object? obj)
            {
                if(obj == null)
                {
                    return 0;
                }

                SeedRange? otherSeedRange = obj as SeedRange?;
                if (otherSeedRange != null){
                    return this.rangeStart.CompareTo(otherSeedRange.Value.rangeStart);
                } else {
                    throw new ArgumentException("Object is not a SeedRange");
                }
            }
        }

        private struct MapRange : IComparable
        {
            public long destinationRangeStart;
            public long sourceRangeStart;
            public long rangeLength;

            public int CompareTo(object? obj)
            {
                if (obj == null) {
                    return 0;
                }

                MapRange? otherMapRange = obj as MapRange?;
                if (otherMapRange != null) {
                    return this.destinationRangeStart.CompareTo(otherMapRange.Value.destinationRangeStart);
                } else {
                    throw new ArgumentException("Object is not MapRange");
                }
            }
        }
    }
}
