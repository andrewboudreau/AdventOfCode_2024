// https://adventofcode.com/2024/day/24

using System.Numerics;

var circuit = ReadTo(rows =>
{
    var readingWires = true;
    var components = new List<Component>();

    foreach (var row in rows)
    {
        if (string.IsNullOrEmpty(row))
        {
            readingWires = false;
            continue;
        }

        if (readingWires)
        {
            var wire = row.Split(':', StringSplitOptions.TrimEntries);
            components.Add(new(wire[0], wire[1] == "1"));
        }
        else
        {
            var chip = row.Split(' ', StringSplitOptions.TrimEntries);
            components.Add(new([chip[0], chip[2]], chip[1], chip[4]));
        }
    }

    return components;
});

foreach (var component in circuit)
{
    foreach (var inputName in component.InputNames)
    {
        var input = circuit.First(c => c.Name == inputName);
        component.AttachComponentInput(input);
    }
}

while (circuit.Any(c => !c.Active))
{
    foreach (var component in circuit)
    {
        component.Tick();
    }
}
var zBits = circuit.Where(x => x.Name.StartsWith('z')).OrderByDescending(x => x.Name).ToList();
Console.WriteLine(string.Join(Environment.NewLine, zBits));

var number = zBits.Aggregate(BigInteger.Zero, (acc, x) => acc * 2 + x.Bit!.Value);
Console.WriteLine($"Number: {number}");

class Component
{
    private readonly List<Component> inputs;
    private readonly string outputName;

    public Component(string outputName, bool on)
    {
        inputs = [];
        InputNames = [];
        this.outputName = outputName;
        On = on;
        Operation = "INPUT";
    }

    public Component(IEnumerable<string> inputNames, string operation, string outputName)
    {
        inputs = [];
        InputNames = [.. inputNames];
        this.outputName = outputName;
        Operation = operation;
    }

    public string[] InputNames { get; private set; }
    public IReadOnlyList<Component> Inputs => inputs;
    public string Operation { get; private set; }
    public string Name => outputName;
    public bool? On { get; private set; }
    public bool Active => On is not null;
    public int? Bit => On is null ? null : On!.Value ? 1 : 0;

    public void AttachComponentInput(Component component)
    {
        inputs.Add(component);
    }

    public Component Tick()
    {
        if (inputs.Any(i => !i.Active))
        {
            Console.WriteLine($"Cannot activate {outputName} because not all inputs are active");
            On = null;
        }
        else if (!On.HasValue)
        {
            if (Operation == "XOR")
            {
                On = inputs[0].On ^ inputs[1].On;
            }
            else if (Operation == "AND")
            {
                On = inputs[0].On & inputs[1].On;
            }
            else if (Operation == "OR")
            {
                On = inputs[0].On | inputs[1].On;
            }
        }

        return this;
    }

    public override string ToString()
    {
        return $"{outputName} = {Operation} {string.Join(", ", inputs.Select(i => i.Name))} {Bit}";
    }
}