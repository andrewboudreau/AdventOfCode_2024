
// https://adventofcode.com/2024/day/12

/*
 * Each garden plot grows only a single type of plant and is indicated by a single letter on your map. 
 * When multiple garden plots are growing the same type of plant and are 
 * touching (horizontally or vertically), they form a region
 * 
 * In order to accurately calculate the cost of the fence around a single region, you 
 * need to know that region's area and perimeter.
 * 
 * AAAA
 * BBCD
 * BBCC
 * EEEC
 * 
 * Each garden plot is a square and so has four sides. The perimeter of a region is 
 * the number of sides of garden plots in the region that do not touch another garden 
 * plot in the same region. The type A and C plants are each in a region with perimeter 10. 
 * The type B and E plants are each in a region with perimeter 8. The lone D plot forms 
 * its own region with perimeter 4.
 * 
 * Due to "modern" business practices, the price of fence required for a region is found by multiplying that region's area by its perimeter. 
 * The total price of fencing all regions on a map is found by adding together the price of fence for every region on the map.
 * 
 */

var grid = new Grid<char>(Read()!, str => [.. str]);
var player = new Player(grid, (player, next) => next != null && player.Location.Value == next.Value);

grid.Render(Console.WriteLine);

var regions = grid.GetRegions().ToList();

var cost = regions.Sum(x => x.Count * CalculatePerimeter(x));
//Console.WriteLine("The part1 cost is " + cost);

int loop = 0;
var sum = 0;

foreach (var region in regions)
{
    var r = new CornerCounter([.. region.Select(n => (n.X, n.Y))]);
    var count = region.Count;
    var turns = r.CountCorners();
    Console.WriteLine("Counting turns in {0}", turns);
    sum += count * turns;
}

Console.WriteLine("The part2 cost is " + sum);

int CalculatePerimeter(List<Node<char>> region)
{
    var perimeter = region.Count * 4;
    foreach (var node in region)
    {
        foreach (var neighbor in grid.Neighbors(node, withDiagonals: false))
        {
            if (region.Contains(neighbor))
            {
                perimeter--;
            }
        }
    }

    //Console.WriteLine($"Region with {region.Count} has perimeter {perimeter}");
    return perimeter;
}

public class CornerCounter
{
    private readonly HashSet<(int x, int y)> region;
    private readonly HashSet<(int x, int y)> boundary;

    public CornerCounter(IEnumerable<(int x, int y)> regionCells)
    {
        region = [.. regionCells];
        boundary = FindBoundary();
    }

    private HashSet<(int x, int y)> FindBoundary()
    {
        var boundary = new HashSet<(int, int)>();

        // Directions: up, right, down, left
        var directions = new (int dx, int dy)[]
        {
            (-1, 0),  // up
            (0, 1),   // right
            (1, 0),   // down
            (0, -1)   // left
        };

        foreach (var (x, y) in region)
        {
            foreach (var (dx, dy) in directions)
            {
                var neighbor = (x + dx, y + dy);
                if (!region.Contains(neighbor))
                {
                    boundary.Add((x, y)); // It's on the boundary
                    break;
                }
            }
        }

        return boundary;
    }

    public int CountCorners()
    {
        // Directions: up, right, down, left
        var directions = new (int dx, int dy)[]
        {
            (-1, 0),  // up
            (0, 1),   // right
            (1, 0),   // down
            (0, -1)   // left
        };

        int cornerCount = 0;

        foreach (var (x, y) in boundary)
        {
            // Check neighbors
            var neighbors = new List<(int, int)>();
            foreach (var (dx, dy) in directions)
            {
                var neighbor = (x + dx, y + dy);
                if (boundary.Contains(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            // A corner occurs when exactly 2 neighbors exist and are orthogonal
            if (neighbors.Count == 2)
            {
                var (n1x, n1y) = neighbors[0];
                var (n2x, n2y) = neighbors[1];

                // Check if neighbors are orthogonal (not in a straight line)
                if (!((n1x == n2x) || (n1y == n2y)))
                {
                    cornerCount += 2; // Orthogonal neighbors -> 2 corners
                }
            }
        }

        return cornerCount == 0 ? 4 : cornerCount;
    }
}

class Player
{
    private readonly Func<Player, Node<char>?, bool>? canPlayerMoveHere;

    public Player(Grid<char> grid, Func<Player, Node<char>?, bool>? canPlayerMoveHere = default)
    {
        Grid = grid;

        Location = Grid[0, 0]
            ?? throw new InvalidOperationException("Invalid location");

        this.canPlayerMoveHere = canPlayerMoveHere;
    }

    public Grid<char> Grid { get; }

    public Node<char> Location { get; private set; }

    public int Direction { get; private set; }

    public int TurnRight() => Direction = (Direction + 1) % 4;

    public bool Forward()
    {
        var next = Direction switch
        {
            0 => Grid.Up(Location),
            1 => Grid.Right(Location),
            2 => Grid.Down(Location),
            3 => Grid.Left(Location),
            _ => null
        };

        if (next is null)
        {
            return false;
        }

        if (canPlayerMoveHere != null && !canPlayerMoveHere(this, next))
        {
            return false;
        }

        Location = next;
        return true;
    }

    public bool MoveTo(Node<char> location)
    {
        if (location == null)
        {
            return false;
        }

        if (canPlayerMoveHere != null && !canPlayerMoveHere(this, location))
        {
            return false;
        }

        Location = location;
        return true;
    }
}
