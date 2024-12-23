// https://adventofcode.com/2024/day/15

var (grid, moves) = ReadTo(rows =>
{
    var grid = new Grid<char>(
        rows: rows.TakeWhile(r => !string.IsNullOrEmpty(r))!,
        factory: row => row.ToCharArray());

    var moves = rows
        .SkipWhile(r => !string.IsNullOrEmpty(r)).Skip(1)
        .TakeWhile(r => !string.IsNullOrEmpty(r))
        .SelectMany(r => r!.ToCharArray())
        .ToList();

    return (grid, moves);
});

var player = grid.Nodes().First(g => g == '@');
var currentMove = 0;

RenderPlayer(player);

grid.Render(null);
var steps = 0;
for (var i = 0; i < moves.Count; i++)
{
    steps++;
    Move();
    Console.Clear();
    grid.Render(null);
    Thread.Sleep(10);
}
grid.Render(null);
Console.WriteLine($"Steps: {steps}");
Console.WriteLine($"Goods: {GetAllGoods().Sum(box => (box.Y * 100) + box.X)}");

IEnumerable<Node<char>> RayCast()
{
    var move = moves[currentMove];
    return move switch
    {
        '<' => grid.LeftFrom(player),
        '>' => grid.RightFrom(player),
        'v' => grid.DownFrom(player),
        '^' => grid.UpFrom(player),
        _ => throw new InvalidOperationException("")
    };
}

bool Move()
{
    var ray = RayCast().ToList();
    var candidatePosition = ray.First();
    currentMove = (currentMove + 1) % moves.Count;

    if (candidatePosition == '#')
    {
        return false;
    }
    else if (candidatePosition == 'O')
    {
        var firstOption = ray.First(n => n == '#' || n == '.');
        if (firstOption == '#')
        {
            return false;
        }

        var playerIndex = ray.IndexOf(candidatePosition);
        var firstEmptyIndex = ray.IndexOf(firstOption);

        for (int i = firstEmptyIndex; i > playerIndex; i--)
        {
            ray[i].SetValue(ray[i - 1].Value);
        }
    }

    candidatePosition.SetValue('@');
    player.SetValue('.');
    player = candidatePosition;

    return true;
}

IEnumerable<Node<char>> GetAllGoods() => grid.Nodes().Where(n => n == 'O');

void RenderPlayer(Node<char> player) => Console.WriteLine($"Player: {player.Value} X:{player.X} Y:{player.Y}");
