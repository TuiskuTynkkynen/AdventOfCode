namespace AdventOfCode2025.Solutions
{
    class Day11 : ISolution
    {
        public long? Part1(TextReader input)
        {
            Dictionary<string, List<string>> devices = new();
            string? line = input.ReadLine();

            // FML different examples
            if(line == "example") { 
                input = new StringReader("aaa: you hhh\nyou: bbb ccc\nbbb: ddd eee\nccc: ddd eee fff\nddd: ggg\neee: out\nfff: out\nggg: out\nhhh: ccc fff iii\niii: out");
                line = input.ReadLine();
            }

            while (line != null)
            {
                string[] substrings = line.Split(':');
                var outputs = substrings[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                devices[substrings[0]] = [.. outputs];
         
                line = input.ReadLine();
            }

            long result = 0;

            Queue<string> queue= new(devices.GetValueOrDefault("you")!);
            while(queue.Count != 0)
            {
                string current = queue.Dequeue();

                if(current == "out")
                {
                    result++;
                    continue;
                }
                
                devices.GetValueOrDefault(current)!.ForEach(queue.Enqueue);
            }

            return result;
        }

        public long? Part2(TextReader input)
        {
            Dictionary<string, List<Connection>> devices = new();
            Dictionary<string, List<string>> reverse = new();
            Queue<string> Outs = [];

            string? line = input.ReadLine();
            // FML different examples
            if (line == "example")
            {
                input = new StringReader("svr: aaa bbb\naaa: fft\nfft: ccc\nbbb: tty\ntty: ccc\nccc: ddd eee\nddd: hub\nhub: fff\neee: dac\ndac: fff\nfff: ggg hhh\nggg: out\nhhh: out");
                line = input.ReadLine();
            }

            while (line != null)
            { 
                string[] substrings = line.Split(':');
                var outputs = substrings[1].Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new Connection(s));

                devices[substrings[0]] = [.. outputs];
                foreach (var output in outputs)
                {
                    if (output.Device == "out")
                    {
                        Outs.Enqueue(substrings[0]);
                        continue;
                    }

                    if (reverse.TryGetValue(output.Device, out var inputs))
                    {
                        inputs.Add(substrings[0]);
                        continue;
                    }

                    reverse[output.Device] = [substrings[0]];
                }

                line = input.ReadLine();
            }
            
            while (Outs.Count != 0)
            {
                string device = Outs.Dequeue();
                if (device == "svr") continue;

                var connection = devices[device].Single();
                connection.DAC |= device == "dac";
                connection.FFT |= device == "fft";

                var inputs = reverse[device];
                reverse.Remove(device); 

                foreach (var inputName in inputs)
                {
                    var outputs = devices[inputName];
                    outputs.RemoveAt(outputs.FindIndex(x => x.Device == device));

                    int outIndex = outputs.FindIndex(x => x.Device == "out");
                    if (outIndex == -1)
                    {
                        outputs.Add(new("out", connection.Count, connection.FFT, connection.DAC));
                    }
                    else
                    {
                        var outConnection = outputs[outIndex];

                        if ((connection.DAC && !outConnection.DAC) || (connection.FFT && !outConnection.FFT))
                        {
                            outConnection.DAC |= connection.DAC;
                            outConnection.FFT |= connection.FFT;

                            outConnection.Count = connection.Count;
                        }
                        else if (connection.DAC == outConnection.DAC && connection.FFT == outConnection.FFT) 
                        {
                            outConnection.Count += connection.Count;
                        }
                    }
                    
                    if (outputs.Count == 1)
                    {
                        Outs.Enqueue(inputName);
                    }
                }
            }

            return devices["svr"].Find(c => c.Device == "out")!.Count;
        }

        public TextReader GetExample()
        {
            return new StringReader("example");
        }
    }
    
    internal class Connection(string device, long count = 1, bool fft = false, bool dac = false)
    {
        public string Device = device;
        public long Count = count;

        public bool FFT = fft;
        public bool DAC = dac;
    }
}