using System.Numerics;

var initialStones = Read().First()!.Split(' ').Select(BigInteger.Parse).ToList();
Console.WriteLine(string.Join(" ", initialStones));

// We'll store counts instead of full expansions to drastically improve performance.
// Key: (stoneValueAsString, stepsRemaining) -> numberOfStonesAfterThatManySteps
Dictionary<(string, int), BigInteger> memo = [];

// We directly compute how many stones after 75 blinks.
// Because we are only computing counts (not full expansions), it should be efficient.
BigInteger total = 0;
foreach (var stone in initialStones)
{
    total += CountStones(stone, 75);
}

Console.WriteLine(total);

BigInteger CountStones(BigInteger stone, int steps)
{
    if (steps == 0)
    {
        return 1;
    }

    var key = (stone.ToString(), steps);
    if (memo.TryGetValue(key, out var cached))
    {
        return cached;
    }

    // Apply rules
    if (stone == 0)
    {
        // Rule 1
        var result = CountStones(1, steps - 1);
        memo[key] = result;
        return result;
    }

    var s = stone.ToString();
    if (s.Length % 2 == 0)
    {
        // Rule 2: split
        int mid = s.Length / 2;
        var left = BigInteger.Parse(s[..mid]);
        var right = BigInteger.Parse(s[mid..]);
        var leftCount = CountStones(left, steps - 1);
        var rightCount = CountStones(right, steps - 1);
        var total = leftCount + rightCount;
        memo[key] = total;
        return total;
    }
    else
    {
        // Rule 3: multiply by 2024
        var multiplied = stone * 2024;
        var result = CountStones(multiplied, steps - 1);
        memo[key] = result;
        return result;
    }
}