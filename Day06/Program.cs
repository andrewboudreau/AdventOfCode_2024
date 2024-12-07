// https://adventofcode.com/2024/day/6

var source = Read().Select(x => x!).ToArray();
var grid = new Grid<char>(source, str => [.. str]);
var start = grid.First(x => x == '^');

var angle = 0;
var current = start;
current.SetValue('S');
grid.Until(grid =>
{
    Node<char>? next = NextPosition(grid, current, angle);
    if (next is null)
    {
        current.Visit();
        current.SetValue('E');
        return true;
    }

    if (grid[next.X, next.Y]?.Value != '#')
    {
        current.Visit();
        current.SetValue('█');
        current = next;
    }
    else
    {
        angle = NextAngle(angle);
    }

    return false;
});

Console.WriteLine("Exited at " + current.X + ", " + current.Y);
Console.WriteLine($"Visited {grid.Count(x => x.Visited > 0)}");

// pathTracker helps detect cycles, if we visit the same position with the same angle, we are in a cycle
var pathTracker = new HashSet<(int X, int Y, int Angle)>();
var loopsFound = 0;

// try to find cycles by putting an blocker at each position in the map and then either leaving the grid or pathTracker detects a cycle
var template = new Grid<char>(source, str => [.. str]);
grid = new Grid<char>(source, str => [.. str]);

Console.WriteLine();
Console.WriteLine("Starting Part2");

foreach (var newWall in template.Nodes())
{
    if (newWall == '#' || newWall == '^')
    {
        continue;
    }

    if (newWall.X == 0 && newWall.Y % 10 == 0)
    {
        Console.WriteLine($"Checking {newWall.X}, {newWall.Y}");
    }

    current = start;
    pathTracker.Clear();
    angle = 0;
    grid[newWall.X, newWall.Y]!.SetValue('#');

    grid.Until(grid =>
    {
        Node<char>? next = NextPosition(grid, current, angle);
        if (next is null)
        {
            return true;
        }

        if (grid[next.X, next.Y]?.Value != '#')
        {
            current = next;
        }
        else
        {
            angle = NextAngle(angle);
        }

        if (!pathTracker.Add((current.X, current.Y, angle)))
        {
            loopsFound++;
            return true;
        }

        return false;
    });

    grid[newWall.X, newWall.Y]!.SetValue('.');
}

Console.WriteLine($"Part2: Found {loopsFound} loops");


static Node<char>? NextPosition(Grid<char> grid, Node<char> current, int angle)
{
    var position = angle switch
    {
        0 => (current.X, current.Y - 1),
        90 => (current.X + 1, current.Y),
        180 => (current.X, current.Y + 1),
        270 => (current.X - 1, current.Y),
        _ => throw new InvalidOperationException()
    };

    return grid[position.Item1, position.Item2];
}

static int NextAngle(int current, char direction = 'R')
{
    return direction switch
    {
        'L' => (current + 270) % 360,
        'R' => (current + 90) % 360,
        _ => throw new InvalidOperationException()
    };
}

