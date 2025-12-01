using System;
using System.Drawing;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2023
{
    internal static class Day10
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-10.txt");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Day ten:\n");
            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split('\n') ?? throw new Exception("Error reading input file");
            int gridLength = input.Length;
            int gridWidth = input[0].Length;
            bool[][] seen = new bool[gridLength ][];
            for(int i = 0; i < gridLength; i++)
            {
                seen[i] = new bool[gridWidth];
            }
            int maxDistance = 0;
            List<Coordinate> directions = new()
            {
                new Coordinate( -1, 0),
                new Coordinate( 0, -1),
                new Coordinate( 0, 1 ),
                new Coordinate( 1, 0 ),
            };
            Dictionary<char, Coordinate> tiles = new()
            {
                { '.', new Coordinate( 0, 0 ) },
                { '|', new Coordinate( 2, 0 ) },
                { '-', new Coordinate( 0, 2 ) },
                { 'L', new Coordinate( -1, 1 ) },
                { 'J', new Coordinate( -1, -1 )  },
                { '7', new Coordinate( 1, -1 ) },
                { 'F', new Coordinate( 1, 1 ) },
                { 'S', new Coordinate( 0, 0 ) },
            };
            PriorityQueue<Pipe, int> pipes = new();

            Coordinate startCord =  input.AsParallel()
                                         .Select((line, index) => new Coordinate(index, line.IndexOf('S')))
                                         .Where(coord => coord.x != -1)
                                         .First();

            foreach (Coordinate direction in directions)
            {
                int posY = startCord.y + direction.y;
                int posX = startCord.x + direction.x;
                
                if( posY < 0 || posX < 0 || posY > gridLength || posX > gridWidth)
                {
                    continue;
                }

                char c = input[posY][posX];
                tiles.TryGetValue(c, out Coordinate tile);
                int total = tile.y * direction.y + tile.x * direction.x;

                if (total == 2 || total < 0)
                {
                    pipes.Enqueue(new Pipe(startCord.y, startCord.x, direction), 0);
                }
            }


            while (pipes.Count > 0)
            {
                pipes.TryDequeue(out Pipe pipe, out int distance);
                int posY = pipe.y + pipe.next.y;
                int posX = pipe.x + pipe.next.x;

                maxDistance = (distance > maxDistance) ? distance : maxDistance;
                if (seen[posY][posX] == true || posY < 0 || posX < 0 || posY > gridLength || posX > gridWidth)
                {
                    continue;
                }

                seen[posY][posX] = true;
                char c = input[posY][posX];
                tiles.TryGetValue(c, out Coordinate tile);
                int total = tile.y * pipe.next.y + tile.x * pipe.next.x;

                if (total == 2 || total == -2)
                {
                    pipes.Enqueue(new Pipe(posY, posX, pipe.next), distance + 1);
                    continue;
                }

                if (total != 0)
                {
                    Coordinate next = (pipe.next.y == 0) ? new Coordinate(tile.y, 0) : new Coordinate(0, tile.x);
                    pipes.Enqueue(new Pipe(posY, posX, next), distance + 1);
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return maxDistance;
        }

        private static int Part2(ref StreamReader reader)
        {
            //Tiles between two outer walls should be regarded as outside the loop
            //F----7    
            //|....|    <- "...." Not enclosed
            //L-7S-J                                                                     "-",  "J",  "J"
            //..||..    <- Can pass through "||", "7F", "JL", etc. vertically or through "-",  "7",  "F", etc. horizontally 

            string[] input = reader.ReadToEnd().Split('\n') ?? throw new Exception("Error reading input file");
            int gridLength = input.Length;
            int gridWidth = input[0].Length;
            List<Coordinate> directions = new()
            {
                new Coordinate( -1, 0),
                new Coordinate( 0, -1),
                new Coordinate( 0, 1 ),
                new Coordinate( 1, 0 ),
            };
            Dictionary<char, Coordinate> tiles = new()
            {
                { '.', new Coordinate( 0, 0 ) },
                { '|', new Coordinate( 2, 0 ) },
                { '-', new Coordinate( 0, 2 ) },
                { 'L', new Coordinate( -1, 1 ) },
                { 'J', new Coordinate( -1, -1 )  },
                { '7', new Coordinate( 1, -1 ) },
                { 'F', new Coordinate( 1, 1 ) },
                { 'S', new Coordinate( 0, 0 ) },
            };
            PriorityQueue<Pipe, int> pipes = new();
            int[][] seen = new int[gridLength + 2][];
            for (int i = 0; i < gridLength + 2; i++)
            {
                seen[i] = new int[gridWidth + 2];
            }

            Coordinate startCord = input.AsParallel()
                                         .Select((line, index) => new Coordinate(index, line.IndexOf('S')))
                                         .Where(coord => coord.x != -1)
                                         .First();

            foreach (Coordinate direction in directions)
            {
                int posY = startCord.y + direction.y;
                int posX = startCord.x + direction.x;

                if (posY < 0 || posX < 0 || posY > gridLength || posX > gridWidth)
                {
                    continue;
                }

                char c = input[posY][posX];
                tiles.TryGetValue(c, out Coordinate tile);
                int total = tile.y * direction.y + tile.x * direction.x;

                if (total == 2 || total < 0)
                {
                    pipes.Enqueue(new Pipe(startCord.y, startCord.x, direction), 0);
                }
            }


            while (pipes.Count > 0)
            {
                pipes.TryDequeue(out Pipe pipe, out int distance);
                int posY = pipe.y + pipe.next.y;
                int posX = pipe.x + pipe.next.x;

                if (posY < 0 || posX < 0 || posY > gridLength || posX > gridWidth || seen[posY + 1][posX + 1] == 1)
                {
                    continue;
                }

                seen[posY + 1][posX + 1] = 1;
                char c = input[posY][posX];
                tiles.TryGetValue(c, out Coordinate tile);
                int total = tile.y * pipe.next.y + tile.x * pipe.next.x;

                if (total == 2 || total == -2)
                {
                    pipes.Enqueue(new Pipe(posY, posX, pipe.next), distance + 1);
                    continue;
                }

                if (total != 0)
                {
                    Coordinate next = (pipe.next.y == 0) ? new Coordinate(tile.y, 0) : new Coordinate(0, tile.x);
                    pipes.Enqueue(new Pipe(posY, posX, next), distance + 1);
                }
            }


            List<Coordinate> outsideLoop = new() {
                new Coordinate(0, 0)
            };
            while (outsideLoop.Any())
            {
                Coordinate postion = outsideLoop.Last();
                outsideLoop.Remove(postion);

                foreach (Coordinate direction in directions)
                {
                    int posY = postion.y + direction.y;
                    int posX = postion.x + direction.x;
                    if (posY >= 0 && posX >= 0 && posY < gridLength + 2 && posX < gridWidth + 2 && seen[posY][posX] == 0)
                    {
                        seen[posY][posX] = 2;
                        outsideLoop.Add(new Coordinate(posY, posX));
                    }
                }

            }

            int foo = 0;
            foreach (int[] arr in seen)
            {
                int count = arr.Count(num => num == 0);
                foo += count;
                foreach (int num in arr)
                {
                    Console.Write((num == 2) ? ' ' : (char)('0' | num));
                } 
                Console.WriteLine(count +  ", " + foo);
            }

            int enclosedCount  = seen.AsParallel()
                                     .Select(item => item.Count(num => num == 0))
                                     .Aggregate((result, next) => result + next);

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }

        private struct Coordinate
        {
            public int y;
            public int x;

            public Coordinate(int Y, int X)
            {
                y = Y;
                x = X;
            }

            public static bool operator ==(Coordinate coord, Coordinate other)
            {
                return (coord.x == other.x && coord.y == other.y);
            
            }

            public static bool operator !=(Coordinate coord, Coordinate other)
            {
                return (coord.x != other.x || coord.y != other.y);
            }
        }

        private struct Pipe
        {
            public int y;
            public int x;
            public Coordinate next;

            public Pipe(int Y, int X, Coordinate N)
            {
                y = Y;
                x = X;
                next = N;
            }
        }
    }
}
