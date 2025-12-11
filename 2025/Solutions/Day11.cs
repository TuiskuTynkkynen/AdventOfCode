namespace AdventOfCode2025.Solutions
{
    class Day11 : ISolution
    {
        public long? Part1(TextReader input)
        {
            Dictionary<string, List<string>> devices = new();
            string? line = input.ReadLine();

            // FML different examples
            if(line == "example") { 
                input = new StringReader("aaa: you hhh\nyou: bbb ccc\nbbb: ddd eee\nccc: ddd eee fff\nddd: ggg\neee: out\nfff: out\nggg: out\nhhh: ccc fff iii\niii: out");
                line = input.ReadLine();
            }

            while (line != null)
            {
                string[] substrings = line.Split(':');
                var outputs = substrings[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                devices[substrings[0]] = [.. outputs];
         
                line = input.ReadLine();
            }

            long result = 0;

            Queue<string> queue= new(devices.GetValueOrDefault("you")!);
            while(queue.Count != 0)
            {
                string current = queue.Dequeue();

                if(current == "out")
                {
                    result++;
                    continue;
                }
                
                devices.GetValueOrDefault(current)!.ForEach(queue.Enqueue);
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            return null;
        }

        public TextReader GetExample()
        {
            return new StringReader("example");
        }
    }
}
