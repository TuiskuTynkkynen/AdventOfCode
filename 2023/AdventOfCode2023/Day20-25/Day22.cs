using System;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023
{
    internal static class Day22
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-22.txt");
            Console.WriteLine("Day twenty two:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string[] input = reader.ReadToEnd().Split(Environment.NewLine) ?? throw new Exception("Error reading input file");
            Vector3 X = new Vector3(1, 0, 0);
            Vector3 Y = new Vector3(0, 1, 0);
            Vector3 Z = new Vector3(0, 0, 1);
            List<Brick> bricks = new();

            foreach(string line in input)
            {
                string[] positionStrings = line.Split('~')[0].Split(',').Concat(line.Split('~')[1].Split(',')).ToArray();
                float[] position = new float[3];
                float[] dimensions = new float[3];

                for(int i = 0; i < 3; i++)
                {
                    position[i] = (float)(int.Parse(positionStrings[i]) + int.Parse(positionStrings[i + 3])) / 2; 
                    dimensions[i] = MathF.Abs(int.Parse(positionStrings[i]) - int.Parse(positionStrings[i + 3])) / 2; 
                }

                Vector3 pos = new Vector3(position[0], position[1], position[2]);
                Vector3 dim = new Vector3(dimensions[0], dimensions[1], dimensions[2]);
                bricks.Add(new Brick(ref pos, ref dim));
            }

            bricks.Sort();
            int brickCount = bricks.Count;
            HashSet<Vector3> positions = new();
            List<int>[] supports = new List<int>[brickCount];
            bool[] supportedBy = new bool[brickCount];
            for(int i = 0; i < brickCount; i++)
            {
                Brick brick = bricks[i];
                if (i == 6)
                {
                    Console.WriteLine("Postions:");
                    foreach (Vector3 position in positions)
                    {
                        Console.WriteLine($"({position.X}, {position.Y}, {position.Z}))");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine(i + ": ");

                List<Vector3> currentPositions = new();

                Vector3 min = brick.Postion - brick.Dimensions;
                Vector3 max = brick.Postion + brick.Dimensions;

                Console.WriteLine($"starts at: ({min.X}, {min.Y}, {min.Z}), ({max.X}, {max.Y}, {max.Z})");
                
                if(i == 6)
                {
                    Console.WriteLine($"centre: ({brick.Postion.X}, {brick.Postion.Y}, {brick.Postion.Z}))");
                    Console.WriteLine($"dims: ({brick.Dimensions.X}, {brick.Dimensions.Y}, {brick.Dimensions.Z}))");
                    Console.WriteLine();
                }

                for (float x = 0.5f; x <= brick.Dimensions.X; x++)
                {
                    if (x == 0.5f && brick.Dimensions.X % 1 == 0)
                    {
                        x = 1;
                    }
                    Vector3 temp = X * x;
                    currentPositions.Add(brick.Postion + temp);
                    currentPositions.Add(brick.Postion - temp);
                }
                for(float y = 0.5f; y <= brick.Dimensions.Y; y++)
                {
                    if(y == 0.5f && brick.Dimensions.Y % 1 == 0)
                    {
                        y = 1;
                    }
                    Vector3 temp = Y * y;
                    currentPositions.Add(brick.Postion + temp);
                    currentPositions.Add(brick.Postion - temp);
                }
                for(float z = 0.5f; z <= brick.Dimensions.Z; z++)
                {
                    if(z == 0.5f && brick.Dimensions.Z % 1 == 0)
                    {
                        z = 1;
                    }
                    Vector3 temp = Z * z;
                    positions.Add(brick.Postion + temp);
                    positions.Add(brick.Postion - temp);
                    if (i == 6)
                    {
                        Console.WriteLine($"z: ({z}, {brick.Dimensions.Z}, {z < brick.Dimensions.Z}))");
                        Console.WriteLine($"temp: ({temp.X}, {temp.Y}, {temp.Z}))");
                        Console.WriteLine($"pos - temp: ({(brick.Postion - temp).X}, {(brick.Postion - temp).Y}, {(brick.Postion - temp).Z}))");
                        Console.WriteLine($"pos + temp: ({(brick.Postion + temp).X}, {(brick.Postion + temp).Y}, {(brick.Postion + temp).Z}))");
                    }
                }

                if (brick.Dimensions.Z != 0)
                {
                    currentPositions.Add(min);
                }
                else if(brick.Dimensions.X % 1 == 0 && brick.Dimensions.Y % 1 == 0)
                { 
                    currentPositions.Add(brick.Postion);
                }


                int positionCount = currentPositions.Count;
                bool supported = false;
                int offsetZ;
                for (offsetZ = 1; offsetZ < min.Z && !supported; offsetZ++)
                {
                    for(int j = 0; j < positionCount; j++)
                    {
                        Vector3 temp = currentPositions[j] - Z * offsetZ;
                        if(i == 6)
                        {
                          Console.WriteLine($"{positionCount}, ({temp.X}, {temp.Y}, {temp.Z}))");
                        }

                        if (temp.X < min.X || temp.X > max.X)
                        {
                            throw new Exception("FML X");
                        }
                        if (temp.Y < min.Y || temp.Y > max.Y)
                        {
                            throw new Exception("FML Y");
                        }
                        if (temp.Z < 1)
                        {
                            throw new Exception("FML Z");
                        }
                        if (positions.Contains(temp))
                        {
                            offsetZ--; 
                            supported = true;
                            break;
                        }
                    }
                }
                offsetZ--;

                for (int j = 0; j < positionCount; j++)
                {
                    positions.Add(currentPositions[j] - Z * offsetZ);
                }

                brick.Postion -= Z * offsetZ;
                bricks[i] = new Brick(ref brick.Postion, ref brick.Dimensions);

                min -= Z * offsetZ;
                max -= Z * offsetZ;
                Console.WriteLine($"(ends at: {min.X}, {min.Y}, {min.Z}), ({max.X}, {max.Y}, {max.Z})");

                if (i == 3)
                {
                    Vector3 amin = bricks[2].Postion - bricks[2].Dimensions;
                    Vector3 amax = bricks[2].Postion + bricks[2].Dimensions;
                    Console.WriteLine($"a: ( {amin.X}, {amin.Y}, {amin.Z}), ({amax.X}, {amax.Y}, {amax.Z})");
                    Console.Write($"a: ( {min.X <= amin.X}, {max.X >= amax.X} ) = {min.X <= amax.X || max.X >= amin.X}, ");
                    Console.WriteLine($"({min.Y <= amin.Y}, {max.Y >= amax.Y}) = {min.Y <= amax.Y || max.Y >= amin.Y}");
                    Console.WriteLine($"a: ( {min.Z - 1 == amax.Z})");
                }

                List<int> supportedByBricks = bricks.AsParallel()
                                                    .Select((itm, indx) => new {
                                                                min = itm.Postion - itm.Dimensions,
                                                                max = itm.Postion + itm.Dimensions,
                                                                index = indx,})
                                                    .Where(item =>
                                                                item.index < i
                                                                && min.Z - 1 == item.max.Z
                                                                && (min.X <= item.max.X || max.X >= item.min.X)
                                                                && (min.Y <= item.max.Y || max.Y >= item.min.Y))
                                                    .Select(item => item.index)
                                                    .ToList();
                
                Console.Write("Supported by: ");
                foreach(int id in supportedByBricks)
                {
                    Console.Write(id + ", ");
                    supportedBy[id] |= (supportedByBricks.Count == 1);
                }
                Console.WriteLine();
                Console.WriteLine();

            }
            //ans < 1133

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return supportedBy.Where(val => !val).Count();
        }
        
        private static int Part2(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }

        private struct Brick : IComparable
        {
            public Vector3 Postion;
            public Vector3 Dimensions;

            public Brick(ref Vector3 pos, ref Vector3 dim) {
                Postion = pos;
                Dimensions = dim;
            }

            public readonly int CompareTo(object? obj) {
                if (obj == null) return 1;

                Brick? otherBrick = obj as Brick?;
                if (otherBrick != null)
                {
                    float minZ = this.Postion.Z - this.Dimensions.Z;
                    return minZ.CompareTo(otherBrick.Value.Postion.Z - otherBrick.Value.Dimensions.Z);
                }
                else throw new ArgumentException("Object is not a Brick");
            }
        } 
    }
}
