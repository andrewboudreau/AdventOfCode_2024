// https://adventofcode.com/2024/day/4

var grid = new Grid<char>(Read()!, str => [.. str]);

var part1 = 0;
string xmas = "XMAS";

foreach (var node in grid.Nodes())
{
    part1 += grid.SequenceEqual(xmas, node, grid.Up) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.Down) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.Left) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.Right) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.UpLeft) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.UpRight) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.DownLeft) ? 1 : 0;
    part1 += grid.SequenceEqual(xmas, node, grid.DownRight) ? 1 : 0;

}

Console.WriteLine($"Part 1: 'XMAS' appears {part1} times");

int part2 = 0;
foreach (var node in grid.Nodes())
{
    // at each node check if it is an A,
    // If it is an A check if th the 4 diagonal corners are M, and S respectively
    // If they are, then we have found a the 'X' MAS

    if (node == 'A')
    {
        var upLeft = grid.UpLeft(node);
        var downRight = grid.DownRight(node);

        var downLeft = grid.DownLeft(node);
        var upRight = grid.UpRight(node);

        if (upLeft != null && upRight != null && downLeft != null && downRight != null)
        {
            var firstDiagonal = ((upLeft == 'M' && downRight == 'S') || (upLeft == 'S' && downRight == 'M'));
            var secondDiagonal = ((upRight == 'M' && downLeft == 'S') || (upRight == 'S' && downLeft == 'M'));

            if (firstDiagonal && secondDiagonal)
            {
                part2++;
            }
        }
    }
}


Console.WriteLine($"Part 2: X-'MAS' appears {part2} times");
