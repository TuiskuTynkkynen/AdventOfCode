using System.Data;

namespace AdventOfCode2023
{
    internal static class Day20
    {
        public static void Solve()
        {
            StreamReader reader = new("InputFiles\\AOC_input_2023-20.txt");
            Console.WriteLine("Day twenty:\n");
            int result1 = Part1(ref reader);
            int result2 = Part2(ref reader);

            Console.WriteLine("Puzzle 1 = " + result1);
            Console.WriteLine("Puzzle 2 = " + result2);
        }

        private static int Part1(ref StreamReader reader)
        {
            string? input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");
            string[] lines = input.Split(Environment.NewLine);
            int lineCount = lines.Length;
            Dictionary<string, IModule> modules = new();

            for (int i = 0; i < lineCount; i++)
            {
                char type = lines[i][0];
                lines[i] = lines[i].Substring(1);
                string module = lines[i].Split("->")[0].Trim();

                string[] outputs = lines[i].Split("->")[1].Split(',');
                int outputCount = outputs.Length;
                for (int j = 0; j < outputCount; j++)
                {
                    outputs[j] = outputs[j].Trim();
                }

                switch (type)
                {
                    case '%':
                        modules.Add(module, new FlipFlop(ref module, ref outputs));
                        break;
                    case '&':
                        List<string> inputStrings = lines.AsParallel()
                                                         .Where((line, index) => line.Contains(module) && index != i)
                                                         .ToList();
                        
                        int inputCount = inputStrings.Count;
                        for(int j = 0; j < inputCount; j++)
                        {
                            inputStrings[j] = inputStrings[j].Split("->")[0].Trim();
                        }

                        string[] inputs = inputStrings.ToArray();

                        modules.Add(module, new Conjunction(ref module, ref outputs, ref inputs));
                        break;
                    case 'b':
                        modules.Add("b" + module, new Broadcaster(ref outputs));
                        break;
                    default: break;
                }
            }

            List<Pulse> pulses = new();

            int highPulses = 0;
            int lowPulses = 0;
            for(int i = 0; i < 1000; i++)
            {
                pulses.Add(new Pulse(false, "button", "broadcaster"));
                while (pulses.Any())
                {
                    Pulse pulse = pulses.First();
                    pulses.RemoveAt(0);

                    if (pulse.State)
                    {
                        highPulses++;
                    }
                    else
                    {
                        lowPulses++;
                    }

                    modules.TryGetValue(pulse.Destination, out IModule? module);
                    if(module == null) {
                        continue;
                    }

                    List<Pulse>? moduleOutput = module.Update(pulse);
                    if(moduleOutput == null || !moduleOutput.Any())
                    {
                        continue;
                    }
                    pulses.AddRange(moduleOutput);
                }
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return highPulses * lowPulses;
        }

        private static int Part2(ref StreamReader reader)
        {
            string? input = reader.ReadToEnd() ?? throw new Exception("Error reading input file");
            
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return -1;
        }
    

        interface IModule
        {
            bool[] State { get; set; }
            string Name { get; }
            string[] Output { get; }
            int OutputCount { get; }

            List<Pulse>? Update(Pulse signal);
        }

        private struct FlipFlop : IModule
        {
            public bool[] State { get; set; }
            public string Name { get; }
            public string[] Output { get; }
            public int OutputCount { get; }
            public FlipFlop(ref string name, ref string[] Outputs)
            {
                Name = name;
                State = new bool[] { false };
                Output = Outputs;
                OutputCount = Outputs.Length;
            }

            public readonly List<Pulse>? Update(Pulse signal)
            { 
                if (signal.State)
                {
                    return null;
                }

                State[0] = !State[0];
                List<Pulse> pulses = new();
                
                for(int i = 0; i < OutputCount; i++)
                {
                    pulses.Add(new Pulse(State[0], Name, Output[i]));
                }

                return pulses;
            }
        }

        private struct Conjunction : IModule
        {
            public bool[] State { get; set; }
            public string Name { get; }
            public string[] Output { get; }
            public int OutputCount { get; }
            public Dictionary<string, int> Input { get; }

            public Conjunction(ref string name, ref string[] Outputs, ref string[] Inputs)
            {
                State = new bool[Inputs.Length];
                Array.Fill(State, false);
                Name = name;
                Output = Outputs;
                OutputCount = Outputs.Length;

                Input = new();
                for (int i = 0; i < Inputs.Length; i++)
                {
                    Input.Add(Inputs[i], i);
                }
            }

            public readonly List<Pulse>? Update(Pulse signal)
            {
                Input.TryGetValue(signal.Source, out int index);

                State[index] = signal.State;

                bool outputState = !State.All(item => item == true);
                List<Pulse> pulses = new();

                for (int i = 0; i < OutputCount; i++)
                {
                    pulses.Add(new Pulse(outputState, Name, Output[i]));
                }

                return pulses;
            }
        }

        private struct Broadcaster : IModule
        {
            public bool[] State { get; set; }
            public string Name { get; }
            public string[] Output { get; }
            public int OutputCount { get; }

            public Broadcaster(ref string[] Outputs)
            {
                Name = "broadcaster";
                State = Array.Empty<bool>();
                Output = Outputs;
                OutputCount = Outputs.Length;
            }

            public readonly List<Pulse>? Update(Pulse signal)
            {
                List<Pulse> pulses = new();

                for (int i = 0; i < OutputCount; i++)
                {
                    pulses.Add(new Pulse(signal.State, Name, Output[i]));
                }

                return pulses;
            }
        }

        private readonly struct Pulse
        {
            public readonly bool State { get; }
            public readonly string Source { get; }
            public readonly string Destination { get;}

            public Pulse(bool state, string source, string destination)
            {
                State = state;
                Source = source;
                Destination = destination;
            }
        }
    }
}
