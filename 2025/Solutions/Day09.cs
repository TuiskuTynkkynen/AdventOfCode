using System.Diagnostics;

namespace AdventOfCode2025.Solutions
{
    class Day09 : ISolution
    {
        public long? Part1(TextReader input)
        {
            var tiles = input.ReadToEnd()
                .Split('\n')
                .Select(s => 
                    s.Split(',')
                    .Select(n => long.Parse(n))
                    .ToArray())
                .Select(tile => (X: tile[0], Y: tile[1]))
                .ToArray();

            long result = 0;
            for(int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    long dx = long.Abs(tiles[i].X - tiles[j].X);
                    long dy = long.Abs(tiles[i].Y - tiles[j].Y);
                    
                    result = long.Max(result, (dx + 1) * (dy + 1));
                }
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            var tiles = input.ReadToEnd()
                .Split('\n')
                .Select(s =>
                    s.Split(',')
                    .Select(n => long.Parse(n))
                    .ToArray())
                .Select(tile => (X: tile[0], Y: tile[1]))
                .ToArray();

            var edges = tiles.Append(tiles.First())
                .Index()
                .Skip(1)
                .Select(tile => (Start: tile.Item, End: tiles[tile.Index - 1]))
                .ToArray();

            List<(int Corner1, int Corner2, long Area)> areas = [];
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    long dx = long.Abs(tiles[i].X - tiles[j].X);
                    long dy = long.Abs(tiles[i].Y - tiles[j].Y);

                    long area = (dx + 1) * (dy + 1);

                    areas.Add((i, j, area));
                }
            }

            return areas.AsParallel()
                .OrderByDescending(r => r.Area)
                .First(rect => RectInsidePolygon(edges, tiles[rect.Corner1], tiles[rect.Corner2]))
                .Area;
        }

        public TextReader GetExample()
        {
            return new StringReader("7,1\n11,1\n11,7\n9,7\n9,5\n2,5\n2,3\n7,3");
        }

        private static bool InRange(long start, long end, long value)
        {
            return start < end ? start <= value && value <= end : end <= value && value <= start;
        }

        private static bool RectInsidePolygon(((long X, long Y) Start, (long X, long Y) End)[] edges, (long X, long Y) corner1, (long X, long Y) corner2)
        {
            long minX = long.Min(corner1.X, corner2.X) + 1;
            long maxX = long.Max(corner1.X, corner2.X) - 1;
            long minY = long.Min(corner1.Y, corner2.Y) + 1;
            long maxY = long.Max(corner1.Y, corner2.Y) - 1;

            foreach (var edge in edges)
            {
                if (edge.Start.X == edge.End.X)
                {
                    if (InRange(minX, maxX, edge.Start.X) && (InRange(edge.Start.Y, edge.End.Y, minY) || InRange(edge.Start.Y, edge.End.Y, maxY)))
                    {
                        return false;
                    }

                    continue;
                }

                if (InRange(minY, maxY, edge.Start.Y) && (InRange(edge.Start.X, edge.End.X, minX) || InRange(edge.Start.X, edge.End.X, maxX)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
