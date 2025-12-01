using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day2
    {
        public static void Solve()
        {
            StreamReader reader = new StreamReader("InputFiles\\AOC_input_2023-02.txt");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Day two:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            Dictionary<string, int> colour = new Dictionary<string, int>()
            {
                { "red", 0 },
                { "green", 1 },
                { "blue", 2 },
            };

            int limitRed = 12; 
            int limitGreen = 13; 
            int limitBlue = 14;

            Dictionary<int, int[]> games = new Dictionary<int, int[]>();
            int sum = 0;
            string? line = reader.ReadLine();
            while (line != null)
            {
                int id = Int32.Parse(line.Split(':')[0].Split(' ')[1].Trim());
                games[id] = new int[3];
                line = line.Split(':')[1];
                string[] gameStrings = line.Split(";");

                for (int i = 0; i < gameStrings.Length; i++) {
                    string[] roundStrings = gameStrings[i].Split(",");
                    
                    for (int j = 0; j < roundStrings.Length; j++) {
                        string[] cubeStrings = roundStrings[j].Trim().Split(' ');
                        int value = Int32.Parse(cubeStrings[0]);
                        colour.TryGetValue(cubeStrings[1], out int index);

                        if (games[id][index] < value){
                            games[id][index] = value;
                        }
                    }
                }

                if (games[id][0] <= limitRed && games[id][1] <= limitGreen && games[id][2] <= limitBlue){
                    sum += id;
                }

                line = reader.ReadLine();
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }
        
        private static int Part2(ref StreamReader reader)
        {
            Dictionary<string, int> colour = new Dictionary<string, int>()
            {
                { "red", 0 },
                { "green", 1 },
                { "blue", 2 },
            };

            Dictionary<int, int[]> games = new Dictionary<int, int[]>();
            int sum = 0;
            string? line = reader.ReadLine();
            while (line != null)
            {
                int id = Int32.Parse(line.Split(':')[0].Split(' ')[1].Trim());
                games[id] = new int[3];
                line = line.Split(':')[1];
                string[] gameStrings = line.Split(";");

                for (int i = 0; i < gameStrings.Length; i++) {
                    string[] roundStrings = gameStrings[i].Split(",");
                    
                    for (int j = 0; j < roundStrings.Length; j++) {
                        string[] cubeStrings = roundStrings[j].Trim().Split(' ');
                        int value = Int32.Parse(cubeStrings[0]);
                        colour.TryGetValue(cubeStrings[1], out int index);

                        if (games[id][index] < value){
                            games[id][index] = value;
                        }
                    }
                }

                sum += games[id][0] * games[id][1] * games[id][2];
                line = reader.ReadLine();
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

    }
}
