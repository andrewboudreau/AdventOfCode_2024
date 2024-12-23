// https://adventofcode.com/2024/day/19

var input = Read()!.ToList();

HashSet<string> towels = [.. input[0]!.Split(",", StringSplitOptions.TrimEntries)];
List<string> designs = [.. input.Skip(2).Select(x => x!)];

var largestTowel = towels.Max(x => x.Length);
Console.WriteLine($"Largest towel is {largestTowel}");
//Console.Write(input.First().Replace(", ", "\r\n"));
//Console.WriteLine(string.Join("\r\n", designs));

var failed = new HashSet<string>();
var possible = 0;
long options = 0;
var anyFound = false;

foreach (var design in designs)
{
    anyFound = false;
    Console.WriteLine($"attempting to match {string.Join("", design)}");
    if (IsPossibleDesign(towels, design) > 0)
    {
        //Console.WriteLine($"Design {design} is possible.");
        //Console.WriteLine(design);
        Console.WriteLine("Options: " + options);
        possible++;
    }
    else
    {
        Console.WriteLine($"-NOT possible Design {design} is not possible.");
    }
}

Console.WriteLine($"There are {possible} possible designs with a total of {options} options.");


/// A design is possible if the towels can be arranged in such a way that the design can be seen.
int IsPossibleDesign(HashSet<string> towels, string design)
{
    int count = 0;
    if (failed.Contains(design))
    {
        return 0;
    }

    var isPossible = false;
    if (design.Length == 0)
    {
        return 1;
    }

    for (var i = Math.Min(largestTowel, design.Length); i >= 0; i--)
    {
        var test = design[..i];
        foreach (var towel in towels.Where(t => t.SequenceEqual(test)))
        {
            //Console.WriteLine($"Matched {string.Join("", test)}");
            if (IsPossibleDesign(towels, design[test.Length..]) > 0)
            {
                isPossible = true;
                anyFound = true;
            }
            else
            {
                failed.Add(design);
            }
        }
    }

    return count;
}