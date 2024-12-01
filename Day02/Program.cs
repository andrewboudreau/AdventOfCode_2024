// https://adventofcode.com/2024/day/2


var reads = Read(line =>
{
    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
    return true;
});

Console.WriteLine(reads.Count());