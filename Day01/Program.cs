// https://adventofcode.com/2024/day/1

Read(input => input!)
    .Aggregate(0,
        (checksum, lineOfText) =>
        {
            var first = lineOfText.FirstOrDefault(x => char.IsDigit(x)) - '0';
            var last = lineOfText.LastOrDefault(x => char.IsDigit(x)) - '0';
            var value = first * 10 + last;
            checksum += value;
            return checksum;
        })
    .ToConsole(sum => $"Part1: {sum}\r\n");


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

bool TryGetDigit(string line, int offset, out int digit)
{
    Dictionary<string, int> stringDigitMapping = new() { ["one"] = 1, ["two"] = 2, ["three"] = 3, ["four"] = 4, ["five"] = 5, ["six"] = 6, ["seven"] = 7, ["eight"] = 8, ["nine"] = 9 };

    digit = 0;
    if (offset >= line.Length)
    {
        throw new ArgumentOutOfRangeException($"Offset {offset} is outside bounds of string with length {line.Length}.");
    }

    if (char.IsDigit(line[offset]))
    {
        digit = line[offset] - '0';
        return true;
    }

    foreach (var pair in stringDigitMapping)
    {
        if (offset + pair.Key.Length <= line.Length && line.Substring(offset, pair.Key.Length) == pair.Key)
        {
            digit = pair.Value;
            return true;
        }
    }

    return false;
}

int FindFirstDigit(string line)
{
    var digit = 0;
    var offset = 0;
    while (offset < line.Length && !TryGetDigit(line, offset, out digit))
    {
        offset++;
    }

    return digit;
}

int FindLastDigit(string line)
{
    var digit = 0;
    var offset = line.Length - 1;
    while (offset >= 0 && !TryGetDigit(line, offset, out digit))
    {
        offset--;
    }

    return digit;
}
