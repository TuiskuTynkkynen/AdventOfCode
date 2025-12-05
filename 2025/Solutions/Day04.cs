namespace AdventOfCode2025.Solutions
{
    class Day04 : ISolution
    {
        public long? Part1(TextReader input)
        {
            char[][] lines = input.ReadToEnd().Split('\n').Select(s => s.ToArray()).ToArray();
            long result = 0;

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] != '@')
                    {
                        continue;
                    }

                    if (GetNeighbors(lines, x, y).Count(c => c == '@') < 4)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            char[][] lines = input.ReadToEnd().Split('\n').Select(s => s.ToArray()).ToArray();
            long result = 0;

            while(true)
            {
                bool done = true;
                
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[y].Length; x++)
                    {
                        if (lines[y][x] != '@'
                            || GetNeighbors(lines, x, y).Count(c => c == '@') >= 4)
                        {
                            continue;
                        } 

                        lines[y][x] = '.';
                        done = false;
                        result++;
                    }
                }

                if (done)
                {
                    break;
                }
            }

            return result;
        }

        public TextReader GetExample()
        {
            return new StringReader("..@@.@@@@.\n@@@.@.@.@@\n@@@@@.@.@@\n@.@@@@..@.\n@@.@@@@.@@\n.@@@@@@@.@\n.@.@.@.@@@\n@.@@@.@@@@\n.@@@@@@@@.\n@.@.@@@.@.");
        }

        private static IEnumerable<char> GetNeighbors(char[][] lines, int x, int y)
        {
            int[][] neighbors = [
                [ -1, -1 ],  [ 0, -1 ], [ 1, -1 ],
                [ -1, 0 ],              [ 1,  0 ],
                [ -1,  1 ],  [ 0,  1 ], [ 1,  1 ],
            ];

            foreach (var neighbor in neighbors)
            {
                (int x, int y) coord = (x + neighbor[0], y + neighbor[1]);
                if((uint)coord.y >= lines.Length || (uint)coord.x >= lines[y + neighbor[1]].Length)
                {
                    continue;
                }

                yield return lines[coord.y][coord.x];
            }
        }
    }
}
