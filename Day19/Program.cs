// https://adventofcode.com/2024/day/19

//300 is too low
//318 is too high

var input = Read()!.ToList();
var towels = input[0]!.Split(",", StringSplitOptions.TrimEntries).Select(x => x.ToCharArray()).ToHashSet();
List<string> designs = [.. input.Skip(2).Select(x => x!)];

var largestTowel = towels.Max(x => x.Length);
Console.WriteLine($"Largest towel is {largestTowel}");
//Console.Write(input.First().Replace(", ", "\r\n"));
//Console.WriteLine(string.Join("\r\n", designs));

var failed = new HashSet<string>();
var possible = 0;
foreach (var design in designs)
{
    if (IsPossibleDesign(towels, design))
    {
        //Console.WriteLine($"Design {design} is possible.");
        possible++;
    }
    else
    {
        //Console.WriteLine($"-NOT possible Design {design} is not possible.");
    }
}

Console.WriteLine($"There are {possible} possible designs.");


/// A design is possible if the towels can be arranged in such a way that the design can be seen.
bool IsPossibleDesign(HashSet<char[]> towels, string design)
{
    //Console.WriteLine($"attempting to match {string.Join("", design)}");
    if (failed.Contains(design))
    {
        return false;
    }

    if (design.Length == 0)
    {
        return true;
    }

    for (var i = Math.Min(largestTowel, design.Length); i >= 0; i--)
    {
        var test = design[..i];
        foreach (var towel in towels.Where(t => t.SequenceEqual(test)))
        {
            //Console.WriteLine($"Matched {string.Join("", test)}");
            if (IsPossibleDesign(towels, design[test.Length..]))
            {
                return true;
            }
        }
    }

    failed.Add(design);
    return false;
}