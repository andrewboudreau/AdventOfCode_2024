// https://adventofcode.com/2024/day/8
//713 is too low

var grid = new Grid<char>(Read()!, str => [.. str]);
var frequencies = new Dictionary<char, List<Node<char>>>();

foreach (var node in grid.Nodes())
{
    if (node != '.')
    {
        if (frequencies.TryGetValue(node, out var list))
        {
            list.Add(node);
        }
        else
        {
            frequencies[node] = [node];
        }
    }
}

Console.WriteLine("Found {0} unique frequencies", frequencies.Count);
Console.WriteLine("They are " + string.Join(", ", frequencies.Keys));
Console.WriteLine();
Console.WriteLine("There are a total of {0} locations", frequencies.Values.Sum(x => x.Count));

Node<char>? current;
foreach (var (key, value) in frequencies)
{
    Console.WriteLine("Found {0} locations for {1}", value.Count, key);
    foreach (var pair in GetPairs(value))
    {
        var xDiff = pair.Item2.X - pair.Item1.X;
        var yDiff = pair.Item2.Y - pair.Item1.Y;

        current = grid[pair.Item1.X, pair.Item1.Y]
            ?? throw new InvalidOperationException("Invalid pair");

        grid.Until(grid =>
        {
            current = grid[(current.X + xDiff, current.Y + yDiff)];
            if (current is not null)
            {
                if (!current.IsVisited)
                {
                    current.Visit();
                    current.SetValue('#');
                }

                return false ;
            }

            return true;
        });


        current = grid[pair.Item2.X, pair.Item2.Y]
            ?? throw new InvalidOperationException("Invalid pair");

        grid.Until(grid =>
        {
            current = grid[(current.X - xDiff, current.Y - yDiff)];
            if (current is not null)
            {
                if (!current.IsVisited)
                {
                    current.Visit();
                    current.SetValue('#');
                }

                return false;
            }

            return true;
        });
    }
}

Console.WriteLine();
Console.WriteLine("Found {0} anti-nodes", grid.Nodes().Sum(x => x.Visited));
grid.Render(Console.WriteLine);

static IEnumerable<(Node<char>, Node<char>)> GetPairs(List<Node<char>> nodes)
{
    for (int i = 0; i < nodes.Count; i++)
    {
        for (int j = i + 1; j < nodes.Count; j++)
        {
            yield return (nodes[i], nodes[j]);
        }
    }
}