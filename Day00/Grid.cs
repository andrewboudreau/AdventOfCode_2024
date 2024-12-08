using System.Collections;
using System.Collections.Immutable;

namespace Day00;

public class Grid<T> : IEnumerable<Node<T>>
{
    private readonly List<Node<T>> nodes;
    private readonly int width;
    private readonly int height;

    public Grid(IEnumerable<string> rows, Func<string, IEnumerable<T>> factory)
       : this(rows.Select(factory))
    {
    }

    public Grid(IEnumerable<IEnumerable<T>> map, Action<Node<T>>? onCreate = default)
    {
        nodes = [];
        int x = 0;
        int y = 0;

        foreach (var row in map)
        {
            foreach (var value in row)
            {
                var node = new Node<T>(x++, y, value);
                nodes.Add(node);
                onCreate?.Invoke(node);
            }

            if (width == 0)
            {
                width = x;
            }
            x = 0;
            y++;
            height++;
        }
    }

    public Node<T>? this[int x, int y]
    {
        get
        {
            if (x < 0) return default;
            if (x >= width) return default;
            if (y < 0) return default;
            if (y >= height) return default;

            int offset = y * width + x;
            if (offset < 0 || offset >= nodes.Count) return default;
            return nodes[offset];
        }
    }

    public Node<T>? this[(int X, int Y) position]
    {
        get => this[position.X, position.Y];
    }

    public int Width => width;
    public int Height => height;

    public IEnumerable<Node<T>> Neighbors(Node<T> of, bool withDiagonals = true)
    {
        if (withDiagonals && this[of.X + 1, of.Y - 1] is Node<T> downRight)
        {
            yield return downRight;
        }

        if (this[of.X + 1, of.Y] is Node<T> right)
        {
            yield return right;
        }

        if (this[of.X, of.Y - 1] is Node<T> down)
        {
            yield return down;
        }

        if (withDiagonals && this[of.X - 1, of.Y - 1] is Node<T> downLeft)
        {
            yield return downLeft;
        }

        if (this[of.X - 1, of.Y] is Node<T> left)
        {
            yield return left;
        }

        if (withDiagonals && this[of.X - 1, of.Y + 1] is Node<T> upLeft)
        {
            yield return upLeft;
        }

        if (this[of.X, of.Y + 1] is Node<T> up)
        {
            yield return up;
        }

        if (withDiagonals && this[of.X + 1, of.Y + 1] is Node<T> upRight)
        {
            yield return upRight;
        }
    }

    public virtual IEnumerable<Node<T>> Nodes() => nodes;

    public void FillDistances(Node<T> from)
    {
        this.ResetVisited();
        this.ResetDistances();

        from.SetDistance(0);
        var nodes = new Queue<Node<T>>();
        nodes.Enqueue(from);

        while (nodes.TryDequeue(out var current))
        {
            foreach (var node in Nodes().Where(x => x.Neighbors.Contains(current)).Except(nodes))
            {
                if (current.Distance + 1 < node.Distance)
                {
                    nodes.Enqueue(node);
                    node.SetDistance(current.Distance + 1);
                }
            }
        }
    }

    public Grid<T> WhileTrue(Func<Grid<T>, bool> operation)
    {
        while (operation(this)) ;
        return this;
    }

    public Grid<T> Until(Func<Grid<T>, bool> operation)
    {
        while (!operation(this)) ;
        return this;
    }

    public Grid<T> Each(Action<Node<T>> action)
    {
        foreach (var node in Nodes())
        {
            action(node);
        }

        return this;
    }

    public int ManhattanDistance(Node<T> from, Node<T> to)
        => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

    public virtual IEnumerable<IEnumerable<Node<T>>> Rows()
    {
        for (var row = 0; row < nodes.Count / width; row++)
        {
            yield return nodes.Skip(row * width).Take(width);
        }
    }

    /// <summary>
    /// Returns true if the sequence of the nodes matches for the given function and direction.
    /// </summary>
    /// <param name="sequenceToMatch">The sequence to match.</param>
    /// <param name="startAt">The starting node, this node is compared to the first node in <paramref name="sequenceToMatch"/>.</param>
    /// <param name="nextNode">The function to get the next node, given the current node.</param>
    /// <returns>True if the sequence was matched, false otherwise.</returns>
    public bool SequenceEqual(ReadOnlySpan<T> sequenceToMatch, Node<T> startAt, Func<Node<T>, Node<T>?> nextNode, Func<T, T, bool>? areEqual = default)
    {
        areEqual ??= (a, b) => a is not null && a.Equals(b);
        Node<T>? current = startAt;

        foreach (var test in sequenceToMatch)
        {

            if (current is null || current.Value is null || test is null)
            {
                return false;
            }
            
            //Console.WriteLine($"Testing {test} at {current}");
            if (!areEqual(current.Value, test))
            {
                return false;
            }

            current = nextNode(current);
        }

        return true;
    }

    public IEnumerator<Node<T>> GetEnumerator()
        => Nodes().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public Node<T>? Up(Node<T> node) => this[node.X, node.Y - 1];
    public Node<T>? Down(Node<T> node) => this[node.X, node.Y + 1];
    public Node<T>? Left(Node<T> node) => this[node.X - 1, node.Y];
    public Node<T>? Right(Node<T> node) => this[node.X + 1, node.Y];
    public Node<T>? UpRight(Node<T> node) => this[node.X + 1, node.Y - 1];
    public Node<T>? UpLeft(Node<T> node) => this[node.X - 1, node.Y - 1];
    public Node<T>? DownRight(Node<T> node) => this[node.X + 1, node.Y + 1];
    public Node<T>? DownLeft(Node<T> node) => this[node.X - 1, node.Y + 1];

}

public static class GridExtensions
{
    public static IEnumerable<T> UpFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.Y - 1; i >= 0; i--)
        {
            yield return grid[node.X, i]!;
        }
    }

    public static IEnumerable<T> DownFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.Y + 1; i < grid.Height; i++)
        {
            yield return grid[node.X, i]!;
        }
    }

    public static IEnumerable<T> LeftFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.X - 1; i >= 0; i--)
        {
            yield return grid[i, node.Y]!;
        }
    }

    public static IEnumerable<T> RightFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.X + 1; i < grid.Width; i++)
        {
            yield return grid[i, node.Y]!;
        }
    }

    public static IEnumerable<T> UpLeftFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var x = node.X - 1; x >= 0; x--)
        {
            for (var y = node.Y - 1; y >= 0; y--)
            {
                yield return grid[x, y]!;
            }
        }
    }

    public static IEnumerable<T> UpRightFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var x = node.X + 1; x < grid.Width; x++)
        {
            for (var y = node.Y - 1; y >= 0; y--)
            {
                yield return grid[x, y]!;
            }
        }
    }

    public static IEnumerable<T> DownLeftFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var x = node.X - 1; x >= 0; x--)
        {
            for (var y = node.Y + 1; y < grid.Height; y++)
            {
                yield return grid[x, y]!;
            }
        }
    }

    public static IEnumerable<T> DownRightFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var x = node.X + 1; x < grid.Width; x++)
        {
            for (var y = node.Y + 1; y < grid.Height; y++)
            {
                yield return grid[x, y]!;
            }
        }
    }

    public static void ResetVisited<T>(this Grid<T> grid)
        => grid.Each(node => node.ResetVisited());

    public static void ResetDistances<T>(this Grid<T> grid)
        => grid.Each(node => node.ResetDistance());
}

public static class GridRenderExtensions
{
    public static void Render<T>(this Grid<T> grid, Action<Node<T>, Action<string>> drawCell, Action<string>? draw = default)
    {
        draw ??= Console.Write;
        foreach (var row in grid.Rows())
        {
            foreach (var node in row)
            {
                drawCell(node, draw);
            }

            draw(Environment.NewLine);
        }
    }

    public static void Render<T>(this Grid<T> grid, Action<Node<T>, Action<string>>? drawCell = default, Action<string>? draw = default, int? minX = 0, int? minY = 0, int? maxX = 1000, int? maxY = 1000)
    {
        draw ??= Console.Write;
        drawCell ??= (node, render) => render(node.Value?.ToString() ?? "C");
        foreach (var row in grid.Rows())
        {
            var any = false;
            foreach (var node in row)
            {
                if (minX <= node.X && node.X <= maxX && minY <= node.Y && node.Y <= maxY)
                {
                    drawCell(node, draw);
                    any = true;
                }
            }

            if (any)
            {
                draw(Environment.NewLine);
            }
        }
    }

    public static void Render<T>(this Grid<T> grid, Dictionary<(int X, int Y), string> display, Action<string>? draw = default)
    {
        draw ??= Console.Write;
        foreach (var row in grid.Rows())
        {
            foreach (var node in row)
            {
                if (display.TryGetValue((node.X, node.Y), out var sprite))
                {
                    draw(sprite);
                }
                else
                {
                    draw(node.Value.ToString());
                }
            }

            draw(Environment.NewLine);
        }
    }

    public static void Render<T>(this Grid<T> grid, int x = 25, int y = 2, Action<IEnumerable<Node<T>>>? draw = default, Action<int, int>? setPosition = default)
    {
        draw ??= Console.WriteLine;
        setPosition ??= Console.SetCursorPosition;
        foreach (var row in grid.Rows())
        {
            setPosition(x, y++);
            draw(row);
        }
    }

    public static void Render<T>(this Grid<T> grid, Action<string>? draw)
    {
        draw ??= Console.WriteLine;
        foreach (var row in grid.Rows())
        {
            draw(string.Join("", row.Select(x => x.Value)));
            //draw(string.Join("", row.Select(x => $"({x.X},{x.Y})[{x.Value}]")));
        }
    }

    public static void Render<T>(this Grid<T> grid, (int X, int Y, int Size) window)
    {
        bool needsLine = false;
        grid.Each(node =>
        {
            if (node.X > window.X - window.Size && node.X < window.X + window.Size)
            {
                if (node.Y > window.Y - window.Size && node.Y < window.Y + window.Size)
                {
                    needsLine = true;
                    Console.Write(node.Value);
                }
            }

            if (node.X == grid.Width - 1 && needsLine == true)
            {
                Console.WriteLine();
                needsLine = false;
            }
        });
    }
}