namespace AdventOfCode2025.Solutions
{
    class Day08 : ISolution
    {
        public long? Part1(TextReader input)
        {
            var boxes = GetBoxes(input);
            var distances = GetDistances(ref boxes);

            // Example needs less connections :(
            int connectionCount = boxes.Length > 20 ? 1000 : 10;

            List<HashSet<int>> circuits = [];
            for (int i = 0; i < connectionCount; i++) 
            {
                var (A, B) = distances.Dequeue();

                Connect(A, B, circuits);
            }

            return circuits.OrderByDescending(c => c.Count)
                .Take(3)
                .Aggregate(1, (total, set) => total * set.Count);
        }

        public long? Part2(TextReader input)
        {
            var boxes = GetBoxes(input);
            var distances = GetDistances(ref boxes);

            List<HashSet<int>> circuits = [];
            while (true)
            {
                var (A, B) = distances.Dequeue();

                Connect(A, B, circuits);

                if (circuits[0].Count != boxes.Length)
                {
                    continue;
                }

                return boxes[A].X * boxes[B].X;
            }
        }

        public TextReader GetExample()
        {
            return new StringReader("162,817,812\n57,618,57\n906,360,560\n592,479,940\n352,342,300\n466,668,158\n542,29,236\n431,825,988\n739,650,466\n52,470,668\n216,146,977\n819,987,18\n117,168,530\n805,96,715\n346,949,466\n970,615,88\n941,993,340\n862,61,35\n984,92,344\n425,690,689");
        }

        private JunctionBox[] GetBoxes(TextReader input)
        {
            static JunctionBox ParseJunction(string s)
            {
                return s.Split(',')
                    .Index()
                    .Aggregate(new JunctionBox(),
                               static (box, tuple) => {
                                   box[tuple.Index] = long.Parse(tuple.Item);
                                   return box;
                               });
            }

            return [.. input.ReadToEnd()
                .Split('\n')
                .Select(ParseJunction)];
        }

        private PriorityQueue<(int A, int B), long> GetDistances(ref readonly JunctionBox[] boxes)
        {
            PriorityQueue<(int A, int B), long> distances = new();
            for (int i = 0; i < boxes.Length - 1; i++)
            {
                for (int j = i + 1; j < boxes.Length; j++)
                {
                    long distance = boxes[i].DistanceSquared(boxes[j]);
                    distances.Enqueue((i, j), distance);
                }
            }

            return distances;
        }

        private void Connect(int A, int B, List<HashSet<int>> circuits)
        {
            var foo = circuits.Find(set => set.Contains(A));
            
            int circuitA = circuits.FindIndex(0, set => set.Contains(A));
            int circuitB = circuits.FindIndex(0, set => set.Contains(B));

            if (circuitA == -1 && circuitB == -1)
            {
                circuits.Add([A, B]);
            }
            else if (circuitA == -1)
            {
                circuits[circuitB].Add(A);
            }
            else if (circuitB == -1)
            {
                circuits[circuitA].Add(B);
            }
            else if (circuitA != circuitB)
            {
                circuits[circuitA].UnionWith(circuits[circuitB]);
                circuits.RemoveAt(circuitB);
            }
        }
    }


    internal class JunctionBox
    {
        public long X;
        public long Y;
        public long Z;

        public long DistanceSquared(JunctionBox other)
        {
            long dx = other.X - X;
            long dy = other.Y - Y;
            long dz = other.Z - Z;

            return dx*dx + dy*dy + dz*dz;
        }

        public long this[int index]
        {
            get{
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set{
                switch (index)
                {
                    case 0: 
                        X = value; 
                        break;
                    case 1: 
                        Y = value; 
                        break;
                    case 2: 
                        Z = value; 
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
