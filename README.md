# AdventOfCode 2024

[2024 Advent of Code](https://adventofcode.com) Solutions in C#

This repository contains my personal solutions for the Advent of Code challenges, implemented in C#. As an advanced C# software engineer, I've approached these puzzles with a focus on efficient and effective programming techniques. Each folder in the repository corresponds to a specific day of the challenge, containing the C# code that I've written to solve the daily puzzles. This is not just a collection of solutions, but a reflection of my problem-solving journey and coding skills in C#. Feel free to explore my approaches and share your thoughts or alternative solutions!

# Day 15: Warehouse Woes
![Day15_Part2](Day15/part2_solution.png)


# Day 1: Historian Hysteria
 [Read the full details](Day01/readme.md) about the solution.

```csharp
Read(line =>
{
    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
    left.Add(parts[0]);
    right.Add(parts[1]);

    frequency.TryGetValue(parts[1], out int value);
    frequency[parts[1]] = ++value;
});

left.Order()
    .Zip(right.Order(), (l, r) => Math.Abs(l - r))
    .Sum()
    .ToConsole(sum => $"Solution to Part 1: {sum}");

left.Sum(x => x * (frequency.TryGetValue(x, out int count) ? count : 0))
    .ToConsole(similarity => $"Solution to Part 2: {similarity}");
```
