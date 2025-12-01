using System.Numerics;

namespace AdventOfCode2023
{
    internal static class Day24
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-24.txt");
            Console.WriteLine("Day twenty four:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string? input = reader.ReadLine() ?? throw new Exception("Error reading input file");
            List<Vector2> hailstonePositions = new(); 
            List<Vector2> hailstoneVelocities = new();
            long min = 200000000000000;
            long max = 400000000000000;
            int sum = 0;

            while (input != null)
            {
                string[] positionStrings = input.Split('@')[0].Split(',');
                string[] velocityStrings = input.Split('@')[1].Split(',');

                long positionX = long.Parse(positionStrings[0].Trim());
                long positionY = long.Parse(positionStrings[1].Trim());
                long velocityX = long.Parse(velocityStrings[0].Trim());
                long velocityY = long.Parse(velocityStrings[1].Trim());

                hailstonePositions.Add(new(positionX, positionY));
                hailstoneVelocities.Add(new(velocityX, velocityY));
                input = reader.ReadLine();
            }

            int hailstoneCount = hailstonePositions.Count;
            for(int i = 0; i < hailstoneCount - 1; i++)
            {
                Vector2 A = hailstonePositions[i];

                decimal slopeA = (decimal)hailstoneVelocities[i].Y / (decimal)hailstoneVelocities[i].X;
                decimal C = (decimal)A.Y - slopeA * (decimal)A.X;

                int multAX = (hailstoneVelocities[i].X > 0) ? 1 : -1; 
                int multAY = (hailstoneVelocities[i].Y > 0) ? 1 : -1; 
                for(int j = i + 1; j < hailstoneCount; j++)
                {
                    Vector2 B = hailstonePositions[j];

                    decimal slopeB = (decimal)hailstoneVelocities[j].Y / (decimal)hailstoneVelocities[j].X;
                    decimal D = (decimal)B.Y - slopeB * (decimal)B.X;

                    int multBX = (hailstoneVelocities[j].X > 0) ? 1 : -1;
                    int multBY = (hailstoneVelocities[j].Y > 0) ? 1 : -1;
                    if (slopeA != slopeB)
                    {                     
                        decimal intersectionX = (D - C) / (slopeA - slopeB);
                        decimal intersectionY = slopeA * intersectionX + C;

                        if(intersectionX >= max || intersectionY >= max || intersectionX <= min || intersectionY <= min)
                        {
                            continue;
                        }

                        if (intersectionX * multAX <= (decimal)A.X * multAX || intersectionY * multAY <= (decimal)A.Y * multAY)
                        {
                            continue;
                        }

                        if (intersectionX * multBX <= (decimal)B.X * multBX || intersectionY * multBY <= (decimal)B.Y * multBY)
                        {
                            continue;
                        }

                        sum++;
                    }
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sum;
        }

        private static int Part2(ref StreamReader reader)
        {
            string input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    }
}
