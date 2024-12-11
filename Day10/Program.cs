// https://adventofcode.com/2024/day/10

var grid = new Grid<char>(Read()!, str => [.. str]);

var trailheads = new List<Node<char>>();
foreach (var node in grid.Nodes())
{
    var neighbors = grid
        .Neighbors(node, withDiagonals: false)
        .Where(x => node.Value + 1 == x.Value)
        .ToArray();

    node.AddNeighbors(neighbors);

    if (node.Value == '0')
    {
        trailheads.AddRange(node);
    }
}

var next = new Stack<Node<char>>();
var trails = new Dictionary<Node<char>, HashSet<Node<char>>>();
var uniquePaths = 0;

Node<char> current = new('.');
foreach (var trailhead in trailheads)
{
    trails.Add(trailhead, []);
    next.Push(trailhead);

    while (next.Count > 0)
    {
        var node = next.Pop();
        if (node.Value == '9')
        {
            trails[trailhead].Add(node);
            uniquePaths++;
        }
        else
        {
            foreach (var neighbor in node.Neighbors)
            {
                next.Push(neighbor);
            }
        }
    }
}

Console.WriteLine("Trailheads: " + trails.Count);
Console.WriteLine("Trail Score: " + trails.Values.Select(x => x.Count).Sum());
Console.WriteLine("Unique Paths: " + uniquePaths);