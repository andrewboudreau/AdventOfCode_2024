// https://adventofcode.com/2024/day/2

/*
 * The levels are either all increasing or all decreasing.
 * Any two adjacent levels differ by at least one and at most three.
 */

var reports = Read(Report.Factory).ToArray();
var part1 = reports.Count(r => r.IsSafe);

Console.WriteLine(string.Join("\r\n", reports.Select(r => r.ToString())));
Console.WriteLine($"Solution to part 1: {part1}");

int part2 = 0;
foreach (var report in reports.Where(x => x.SingleOutlierIndex.HasValue))
{
    var index1 = report.SingleOutlierIndex!.Value;
    var index2 = report.SingleOutlierIndex!.Value + 1;

    var report1 = Report.Factory(report.Levels.WithoutElementAt(index1));
    var report2 = Report.Factory(report.Levels.WithoutElementAt(index2));
    
    if (report1.IsSafe || report2.IsSafe)
    {
        part2++;
    }

    //Console.WriteLine($"report1: {report1}");
    //Console.WriteLine($"report2: {report2}");
}

Console.WriteLine($"Solution to part 2: {part1 + part2}");


class Report
{
    public Report(IEnumerable<int> levels)
    {
        Levels = levels.ToArray();
        Differences = Levels.Skip(1).Zip(Levels, (a, b) => a - b).ToArray();

        var decreasingIndexes = Differences.Select((d, i) => (d, i)).Where(x => x.d <= -1).Select(x => x.i).ToArray();
        var increasingIndexes = Differences.Select((d, i) => (d, i)).Where(x => x.d >= 1).Select(x => x.i).ToArray();
        var noChangingIndexes = Differences.Select((d, i) => (d, i)).Where(x => x.d == 0).Select(x => x.i).ToArray();
        var outOfRangeIndexes = Differences.Select((d, i) => (d, i)).Where(x => IsNotWithinRange(x.d)).Select(x => x.i).ToArray();

        var isIncreasing = increasingIndexes.Length == Differences.Length;
        var isDecreasing = decreasingIndexes.Length == Differences.Length;

        IsSafe = (isIncreasing || isDecreasing) && Differences.All(IsWithinRange);

        if (!IsSafe)
        {
            if (decreasingIndexes.Length == 1)
            {
                SingleOutlierIndex = decreasingIndexes[0];
            }
            else if (increasingIndexes.Length == 1)
            {
                SingleOutlierIndex = increasingIndexes[0];
            }
            else if (noChangingIndexes.Length == 1)
            {
                SingleOutlierIndex = noChangingIndexes[0];
            }
            else if (outOfRangeIndexes.Length == 1)
            {
                SingleOutlierIndex = outOfRangeIndexes[0];
            }
        }
    }

    public int[] Levels { get; }
    public int[] Differences { get; }
    public bool IsSafe { get; }
    public int? SingleOutlierIndex { get; }

    public override string ToString()
    {
        return $"{nameof(Levels)}: {string.Join(", ", Levels)}\t{nameof(Differences)}: {string.Join(", ", Differences)}\t {nameof(IsSafe)}: {IsSafe},\t{nameof(SingleOutlierIndex)}: [{(SingleOutlierIndex.HasValue ? SingleOutlierIndex : " ")}]{(SingleOutlierIndex.HasValue ? "-> " + Differences[SingleOutlierIndex.Value] : "")}";
    }

    public static Report Factory(IEnumerable<int> levels) => new(levels);

    private static bool IsWithinRange(int difference)
    {
        return Math.Abs(difference) >= 1 && Math.Abs(difference) <= 3;
    }

    private static bool IsNotWithinRange(int difference)
    {
        return Math.Abs(difference) < 1 || Math.Abs(difference) > 3;
    }
}
