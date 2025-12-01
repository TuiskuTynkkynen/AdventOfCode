using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day4
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-04.txt");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Day two:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            int sum = 0;
            string? line = reader.ReadLine();
            Regex numbers = new Regex(@"([0-9]+)");
            
            while (line != null)
            {
                line = line.Split(':')[1];
                int cardValue = 0;
                string winningString = line.Split("|")[0];
                string cardNumbers = line.Split("|")[1];
                HashSet<string> winningNumbers = new HashSet<string>();

                foreach (Match match in numbers.Matches(winningString))
                {
                    winningNumbers.Add(match.Value);
                }

                foreach (Match match in numbers.Matches(cardNumbers))
                {
                    if (winningNumbers.Contains(match.Value))
                    {
                        cardValue = (cardValue != 0) ? cardValue * 2 : 1;
                    }

                }
                
                sum += cardValue;
                line = reader.ReadLine();
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            int sum = 0;
            string? line = reader.ReadLine();
            Regex numbers = new Regex(@"([0-9]+)");
            Dictionary<int, int[]> cards = new Dictionary<int, int[]>();
            
            while (line != null)
            {
                int id = Int32.Parse(numbers.Match(line.Split(":")[0]).Value);
                line = line.Split(':')[1];
                int cardValue = 0;
                string winningString = line.Split("|")[0];
                string cardNumbers = line.Split("|")[1];
                HashSet<string> winningNumbers = new HashSet<string>();

                foreach (Match match in numbers.Matches(winningString))
                {
                    winningNumbers.Add(match.Value);
                }

                foreach (Match match in numbers.Matches(cardNumbers))
                {
                    if (winningNumbers.Contains(match.Value))
                    {
                        cardValue++;
                    }
                }

                cards[id] = new int[] { 1, cardValue };
                line = reader.ReadLine();
            }

            foreach (KeyValuePair<int, int[]> card in cards)
            {
                int cardId = card.Key;
                int cardCount = card.Value[0];
                int cardValue = card.Value[1];

                for(int i =0; i < cardCount; i++)
                {
                    for (int j = 1; j <= cardValue; j++)
                    {
                        cards[cardId + j][0]++;
                    }
                }
                sum += cardCount;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
    }
}
