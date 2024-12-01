namespace Day00;

public class Line(int x1, int y1, int x2, int y2)
{
    public Line(int x1, int x2)
        : this(x1, 0, x2, 0)
    {
    }

    public Line(params int[] coords)
        : this(coords[0], coords[1], coords[2], coords[3])
    {
    }

    public int X1 { get; } = x1;
    public int Y1 { get; } = y1;
    public int X2 { get; } = x2;
    public int Y2 { get; } = y2;

    public int MinX => Math.Min(X1, X2);
    public int MinY => Math.Min(Y1, Y2);
    public int MaxX => Math.Max(X1, X2);
    public int MaxY => Math.Max(Y1, Y2);

    public bool Horizontal => Y1 == Y2;

    public bool Vertical => X1 == X2;

    public bool Diagonal => !Horizontal && !Vertical;

    public IEnumerable<(int X, int Y)> Path()
    {
        var current = (X1, Y1);
        while (current != (X2, Y2))
        {
            yield return current;
            current = (
                current.X1 + Math.Sign(X2 - X1),
                current.Y1 + Math.Sign(Y2 - Y1));
        }

        yield return current;
    }

    public override string ToString()
        => $"{X1},{Y1} -> {X2},{Y2}";

    /// <summary>
    /// Splits things like "1,2 -> 9,2" into two x.y points.
    /// </summary>
    /// <param name="input">the string input</param>
    /// <param name="points">The separator for the points</param>
    /// <param name="coords">The separator for the coordinates</param>
    /// <returns>an integer line from the input string</returns>
    public static Line Create(string input, string points, char coords)
        => new(input.Split(points, StringSplitOptions.TrimEntries).Split(coords).ToArray());

    public static Line Create(string input)
        => Create(input, "->", ',');
}
