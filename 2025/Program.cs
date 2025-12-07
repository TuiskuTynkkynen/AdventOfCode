namespace AdventOfCode2025
{
    class Program
    {
        static void Main()
        {

            while (true)
            {
                uint day = 0;
                try
                {
                    Console.Write("Day to get the solution for (1 - 12): ");
                    day = uint.Parse(Console.ReadLine()!);
                    if (day == 0 || day > 12) { throw new IndexOutOfRangeException(); }
                }
                catch 
                {
                    Console.WriteLine("Not a valid day number\n");
                    continue;
                }

                ISolution? solution = GetSolution(day);
                if(solution == null) {
                    Console.WriteLine($"Day {day} hasn't been implemented yet\n");
                    continue;
                }

                Console.WriteLine($"\nDay {day}: ");
                solution.Print(day, GetInput(day));
                Console.WriteLine();
            }
        }

        private static ISolution? GetSolution(uint day)
        {
            switch (day)
            {
                case 1: return new Solutions.Day01();
                case 2: return new Solutions.Day02();
                case 3: return new Solutions.Day03();
                case 4: return new Solutions.Day04();
                case 5: return new Solutions.Day05();
                case 6: return new Solutions.Day06();
                case 7: return new Solutions.Day07();
            }

            return null;
        }

        private static StreamReader? GetInput(uint day)
        {
            try
            {
                return new StreamReader($"Input/Day{day:d2}.txt");
            }
            catch
            { 
                return null;
            }
        }
    }

    public interface ISolution
    {
        public long? Part1(TextReader input);
        public long? Part2(TextReader input);

        public TextReader GetExample();

        public void Print(uint day, StreamReader? input)
        {
            long? example1 = Part1(GetExample());
            long? example2 = Part2(GetExample());

            Console.WriteLine("Puzzle 1 Example " + (example1.HasValue ? $"= {example1}" : "Not Solved"));
            Console.WriteLine("Puzzle 2 Example " + (example2.HasValue ? $"= {example2}" : "Not Solved"));
            Console.WriteLine();

            if (input == null)
            {
                Console.WriteLine($"Input for Day {day} is missing. Be sure to get it from the AoC site");
                return;
            }

            long? part1 = Part1(input);
            input.BaseStream.Position = 0;
            long? part2 = Part2(input);

            Console.WriteLine("Puzzle 1 " + (part1.HasValue ? $"= {part1}" : "Not Solved"));
            Console.WriteLine("Puzzle 2 " + (part2.HasValue ? $"= {part2}" : "Not Solved"));
        }
    }
}