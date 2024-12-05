// https://adventofcode.com/2024/day/5

var pageOrderingRules = new Dictionary<int, List<int>>();
var pageUpdates = new List<List<int>>();

Read((string line) =>
{
    if (line.Contains('|'))
    {
        var parts = line.Split('|').Select(int.Parse).ToArray();
        if (pageOrderingRules.TryGetValue(parts[0], out var rules))
        {
            rules.Add(parts[1]);
        }
        else
        {
            pageOrderingRules[parts[0]] = [parts[1]];
        }
    }
    else if (line.Contains(','))
    {
        var values = line.Split(',').Select(int.Parse).ToList();
        pageUpdates.Add(values);
    }
});


// enumerate the sets of pages to update
// for each set keep track of a list of updated pages seen so far
// for each page in the set see if any of the pages the page must come before have already been seen
// the update is invalid if any of the pages being updated has a seen value from the ordering rules

List<List<int>> validUpdates = [];
List<List<int>> invalidUpdates = [];

foreach (var update in pageUpdates)
{
    var alreadySeen = new HashSet<int>();
    var invalid = false;

    foreach (var page in update)
    {
        if (pageOrderingRules.TryGetValue(page, out var mustComeBefore))
        {
            if (alreadySeen.Overlaps(mustComeBefore))
            {
                invalid = true;
                invalidUpdates.Add(update);
                break;
            }
        }
        alreadySeen.Add(page);
    }
    if (!invalid)
    {
        validUpdates.Add(update);
    }
}

int part1 = 0;
foreach (var update in validUpdates)
{
    part1 += update[update.Count / 2];
}

Console.WriteLine($"Part 1: {validUpdates.Count} valid sets page updates with a checksum of {part1}");


// Part 2
int swaps;
int passes = 0;
do
{
    Console.WriteLine("Starting pass {0}", passes++);

    swaps = 0;
    foreach (var update in invalidUpdates)
    {
        //Console.WriteLine("Original set {0}", string.Join(", ", update));
        foreach (var rules in pageOrderingRules)
        {
            var page = rules.Key;
            var indexOfPage = update.IndexOf(page);

            foreach (var mustComeBefore in rules.Value)
            {
                var indexOfMustComeBefore = update.IndexOf(mustComeBefore);
                if (
                    indexOfPage != -1 &&
                    indexOfMustComeBefore != -1 &&
                    indexOfPage > indexOfMustComeBefore)
                {
                    // swap the pages
                    Console.WriteLine("  Swapping {0} and {1}", update[indexOfPage], update[indexOfMustComeBefore]);
                    (update[indexOfPage], update[indexOfMustComeBefore]) = (update[indexOfMustComeBefore], update[indexOfPage]);
                    swaps++;
                }
            }
        }
        Console.WriteLine("Updated set {0}", string.Join(", ", update));
    }
} while (swaps > 0);

int part2 = 0;
foreach (var update in invalidUpdates)
{
    part2 += update[update.Count / 2];
}
Console.WriteLine();
Console.WriteLine($"Part 2: fixed after {passes} passes with a checksum of {part2}");
