# AdventOfCode 2024

[2024 Advent of Code](https://adventofcode.com) Solutions in C#

This repository contains my personal solutions for the Advent of Code challenges, implemented in C#. As an advanced C# software engineer, I've approached these puzzles with a focus on efficient and effective programming techniques. Each folder in the repository corresponds to a specific day of the challenge, containing the C# code that I've written to solve the daily puzzles. This is not just a collection of solutions, but a reflection of my problem-solving journey and coding skills in C#. Feel free to explore my approaches and share your thoughts or alternative solutions!

# Day 01
 [Read the full details](Day01/readme.md) about the solution.

## Part Two - Numeric and Spelled-Out Digits
```csharp
Read(input => input!)
    .Aggregate(0,
        (checksum, lineOfText) =>
        {
            var first = FindFirstDigit(lineOfText);
            var last = FindLastDigit(lineOfText);
            var value = first * 10 + last;

            Console.WriteLine($"{lineOfText}: {value}");
            checksum += value;
            return checksum;
        })
    .ToConsole(sum => $"Part2: {sum}");
```
