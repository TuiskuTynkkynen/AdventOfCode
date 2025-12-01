using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal static class Day7
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-07.txt");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Day seven:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            Dictionary<char, int> cardValue = new Dictionary<char, int>() {
                {'A', 13},
                {'K', 12},
                {'Q', 11},
                {'J', 10},
                {'T', 9},
                {'9', 8},
                {'8', 7},
                {'7', 6},
                {'6', 5},
                {'5', 4},
                {'4', 3},
                {'3', 2},
                {'2', 1},
            };
            Regex numbers = new Regex(@"([0-9]+)");
            List<Hand> hands = new List<Hand>();
            int winnings = 0;

            string? input = reader.ReadLine();
            while (input != null)
            {
                string[] line = input.Split(' ');
                int bid = Int32.Parse(line[1]);
                string hand = line[0];
                int cardCount = hand.Length;
                int[] cardValues = new int[cardCount];

                List<int> matches = hand.AsParallel()
                                        .Distinct()
                                        .Select(item => hand.Count(f => f == item))
                                        .ToList();

                int handType = matches.Max();
                switch (handType)
                {
                    case 5:
                    case 4:
                        handType++;
                        break;
                    case 3:
                        if (matches.Contains(2))
                        {
                            handType = 4;
                        }
                        break;
                    case 2:
                        handType = matches.Count(f => f == 2);
                        break;
                    default:
                        handType = 0;
                        break;
                }

                for (int i = 0; i < cardCount; i++)
                {
                    cardValue.TryGetValue(hand[i], out int value);
                    cardValues[i] = value;
                }

                hands.Add(new Hand(handType, bid, cardValues));

                input = reader.ReadLine();
            }

            hands.Sort();

            for (int i = 0; i < hands.Count; i++)
            {
                winnings += hands[i].bid * (i + 1);
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return winnings;
        }

        private static int Part2(ref StreamReader reader)
        {
            Dictionary<char, int> cardValue = new Dictionary<char, int>() {
                {'A', 13},
                {'K', 12},
                {'Q', 11},
                {'T', 10},
                {'9', 9},
                {'8', 8},
                {'7', 7},
                {'6', 6},
                {'5', 5},
                {'4', 4},
                {'3', 3},
                {'2', 2},
                {'J', 1},
            };
            Regex numbers = new Regex(@"([0-9]+)");
            List<Hand> hands = new List<Hand>();
            int winnings = 0;

            string? input = reader.ReadLine();
            while (input != null)
            {
                string[] line = input.Split(' ');
                int bid = Int32.Parse(line[1]);
                string hand = line[0];
                int cardCount = hand.Length;
                int[] cardValues = new int[cardCount];

                for (int i = 0; i < cardCount; i++)
                {
                    cardValue.TryGetValue(hand[i], out int value);
                    cardValues[i] = value;
                }

                List<int> matches = hand.AsParallel()
                                        .Distinct()
                                        .Select(item => { if (item == 'J') { return 0; } return hand.Count(f => f == item); })
                                        .ToList();

                int jokerCount = hand.Count(item => item == 'J');
                int handType = matches.Max() + jokerCount;

                matches.Remove(matches.Max());
                switch (handType)
                {
                    case 5:
                    case 4:
                        handType++;
                        break;
                    case 3:
                        if (matches.Contains(2))
                        {
                            handType = 4;
                        }
                        break;
                    case 2:
                        if(jokerCount == 0)
                        {
                            handType = matches.Contains(2) ? 2 : 1;
                        } else
                        {
                            handType = 1;
                        }
                        break;
                    default:
                        handType = 0;
                        break;
                }

                hands.Add(new Hand(handType, bid, cardValues));

                input = reader.ReadLine();
            }

            hands.Sort();

            for (int i = 0; i < hands.Count; i++)
            {
                winnings += hands[i].bid * (i + 1);
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return winnings;
        }

        private struct Hand : IComparable
        {
            public int type;
            public int bid;
            public int[] cardValues;

            public Hand(int t, int b, int[] cV)
            {
                type = t;
                bid = b;
                cardValues = cV;
            }
        
            public int CompareTo(object? obj)
            {
                if (obj == null)
                { 
                    return 0; 
                }

                Hand? otherHand = obj as Hand?;
                if (otherHand != null)
                {
                    int typeComparison = this.type.CompareTo(otherHand.Value.type);
                    if(typeComparison == 0)
                    {
                        int cardComparison = 0;
                        for (int i = 0; cardComparison == 0 && i < cardValues.Length; i++)
                        {
                            cardComparison = this.cardValues[i].CompareTo(otherHand.Value.cardValues[i]);
                        }
                        return cardComparison;
                    } else
                    {
                        return typeComparison;
                    }
                }
                else
                {
                    throw new ArgumentException("Object is not MapRange");
                }
            }
        }
        
    }
}
