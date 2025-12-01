using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day8
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-08.txt");
            int result1 = Part1(ref reader);
            long result2 = Part2(ref reader);

            Console.WriteLine("Day eight:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string? input = reader.ReadLine() ?? throw new Exception("Error reading input file");
            string directions = input;
            int directionCount = directions.Length;
            Regex capitals = new Regex(@"([A-Z]+)");
            Dictionary<string, (string left, string right)> instructions = new Dictionary<string, (string left, string right)>();
            string start = "AAA";
            string end = "ZZZ";

            input = reader.ReadLine();
            while (input != null)
            {
                MatchCollection matches = capitals.Matches(input);
                if (matches.Count > 0) { 
                    string key = matches[0].Value;
                    string left = matches[1].Value;
                    string right = matches[2].Value;
                    instructions.Add(key, (left, right));
                }
                input = reader.ReadLine();
            }

            string currentElement = start;
            int steps = 0;
            while (currentElement != end) {
                int directionIndex = steps % directionCount;
                instructions.TryGetValue(currentElement, out (string left, string right) instruction);

                if (directions[directionIndex] == 'L')
                {
                    currentElement = instruction.left;
                }
                else
                {
                    currentElement = instruction.right;
                }
                steps++;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return steps;
        }
        
        private static long Part2(ref StreamReader reader)
        {
            string? input = reader.ReadLine() ?? throw new Exception("Error reading input file"); ;
            int[] directions = new int[input.Length];
            Regex capitals = new Regex(@"([0-9]*[A-Z]+)");
            Dictionary<string, string[]> instructions = new Dictionary<string, string[]>();
            List<string> startElements = new List<string>();
           
            for (int i = 0; i < input.Length; i++)
            {   
                if (input[i] == 'R')
                {
                    directions[i] = 1;
                }
            }
            int directionCount = directions.Length;
            int lastChar = 0;

            input = reader.ReadLine();
            while (input != null)
            {
                MatchCollection matches = capitals.Matches(input);
                if (matches.Count > 0)
                {
                    string key = matches[0].Value;
                    string left = matches[1].Value;
                    string right = matches[2].Value;
                    instructions.Add(key, new string[] { left, right });

                    lastChar = key.Length - 1;
                    if (key[lastChar] == 'A')
                    {
                        startElements.Add(key);
                    }
                }
                input = reader.ReadLine();
            }

            int startCount = startElements.Count;
            int[] loopLengths = new int[startCount];
            for(int i = 0; i < startElements.Count; i++)
            {
                int steps = 0;
                int directionIndex = 0;
                string currentElement = startElements[i];
                HashSet<string> visitedElements = new HashSet<string>();

                while (!visitedElements.Contains(currentElement + directionIndex))
                {
                    visitedElements.Add(currentElement + directionIndex);
                    directionIndex = (int)(steps % directionCount);
                    instructions.TryGetValue(currentElement, out string[] instruction);
                    currentElement = instruction[directions[directionIndex]];
                    steps++;
                }

                string loopStart = currentElement;
                currentElement = startElements[i];
                directionIndex = 0;
                int loopStartIndex = 0;
                while (currentElement != loopStart)
                {
                    directionIndex = (int)(loopStartIndex % directionCount);
                    instructions.TryGetValue(currentElement, out string[] instruction);
                    currentElement = instruction[directions[directionIndex]];
                    loopStartIndex++;
                }

                loopLengths[i] = steps - loopStartIndex;
            }

            List<int> loopPrimes = new List<int>();
            
            for (int i = 0; i < startCount; i++)
            {
                List<int> primes = new List<int>();

                for (int div = 2; div <= loopLengths[i]; div++)
                {
                    while (loopLengths[i] % div == 0)
                    {
                        primes.Add(div);
                        loopLengths[i] = loopLengths[i] / div;
                    }

                }

                foreach(int prime in primes.Distinct())
                {
                    int primeCount = primes.Count(item => item == prime) - loopPrimes.Count(item => item == prime);
                    for (int j = 0; j < primeCount; j++)
                    {
                        loopPrimes.Add(prime);
                    }
                }
            }

            long LCM = 1;
            foreach (int prime in loopPrimes)
            {
                LCM *= prime;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return LCM;
        }

    }
}
