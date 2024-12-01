// https://adventofcode.com/2024/day/1

var left = new List<int>();
var right = new List<int>();
var frequency = new Dictionary<int, int>();

Read(line =>
{
    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
    left.Add(parts[0]);
    right.Add(parts[1]);

    frequency.TryGetValue(parts[1], out int value);
    frequency[parts[1]] = ++value;
});

var sum = left.Order()
    .Zip(right.Order(), (l, r) => Math.Abs(l - r))
    .Sum();

Console.WriteLine($"Solution to Part1: {sum}");

var similarity = 
    left.Sum(x => x * (frequency.TryGetValue(x, out int count) ? count : 0));

Console.WriteLine($"Solution to Part2: {similarity}");

