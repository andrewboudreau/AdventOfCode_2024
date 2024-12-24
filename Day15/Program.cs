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
    player = MovePlayer(moves[currentMove], player);
    currentMove = (currentMove + 1) % moves.Count;

    //Console.Clear();
    //grid.Render(null);
    //Thread.Sleep(10);
}
grid.Render(null);
Console.WriteLine($"Steps: {steps}");
Console.WriteLine($"Goods Position: {GetAllGoods().Sum(box => (box.Y * 100) + box.X)} should equal 1489116 for part 2");

IEnumerable<Node<char>> RayCast(char? move = default, Node<char>? from = default)
{
    from ??= player;
    move ??= moves[currentMove];
    return move switch
    {
        '<' => grid.LeftFrom(from),
        '>' => grid.RightFrom(from),
        'v' => grid.DownFrom(from),
        '^' => grid.UpFrom(from),
        _ => throw new InvalidOperationException("")
    };
}

(bool CanMove, IReadOnlyList<Node<char>> Touching) CanMove(char move, Node<char> from)
{
    var next = RayCast(move, from).First();
    if (next == '#')
    {
        return (false, []);
    }

    else if (next == '.')
    {
        return (true, [from]);
    }

    else if (next == '[' || next == ']')
    {
        var isPushingUp = move == '^';
        var otherBoxSideOffset = next == '[' ? 1 : -1;

        List<Node<char>> touching = [from];
        var impactedBoxHalf = CanMove(move, next);
        var otherBoxHalf = CanMove(move, grid[next.X + otherBoxSideOffset, next.Y]!);

        if (impactedBoxHalf.CanMove && otherBoxHalf.CanMove)
        {
            return (true, OrderNodesForDrawUpdate(isPushingUp, [from, .. impactedBoxHalf.Touching, .. otherBoxHalf.Touching]));
        }

        return (false, []);
    }

    static IReadOnlyList<Node<char>> OrderNodesForDrawUpdate(bool isPushingUp, IEnumerable<Node<char>> nodes)
    {
        return isPushingUp ?
            [.. nodes.OrderBy(n => n.Y).ThenBy(n => n.X).Distinct()] :
            [.. nodes.OrderByDescending(n => n.Y).ThenBy(n => n.X).Distinct()];
    }

    throw new InvalidOperationException($"Cannot handle char = '{next}'");
}

Node<char> MovePlayer(char move, Node<char> player)
{
    var ray = RayCast(move, player).ToList();
    var candidatePosition = ray.First();

    if (candidatePosition == '#')
    {
        return player;
    }

    if (candidatePosition == '.')
    {
        return MovePlayerTo(player, candidatePosition);
    }


    if ((move == '<' || move == '>'))
    {
        var search = ray.First(n => n == '#' || n == '.');
        if (search == '#')
        {
            return player;
        }
        else
        {
            var playerIndex = ray.IndexOf(candidatePosition);
            var firstEmptyIndex = ray.IndexOf(ray.First(n => n == '.'));
            for (int i = firstEmptyIndex; i > playerIndex; i--)
            {
                ray[i].SetValue(ray[i - 1].Value);
            }
        }
    }
    else if ((move == 'v' || move == '^'))
    {
        var result = CanMove(move, player);
        //Console.Clear();
        //grid.Render((node, draw) =>
        //{
        //    if (result.Touching.Contains(node))
        //    {
        //        draw((tick++ % 10).ToString());
        //    }
        //    else
        //    {
        //        draw(node.Value.ToString());
        //    }
        //}, null);

        if (result.CanMove)
        {
            var direction = move == 'v' ? 1 : -1;

            // move any node in touching one in the move direction
            foreach (var source in result.Touching)
            {
                var destination = grid[source.X, source.Y + direction]
                    ?? throw new InvalidOperationException();

                //Console.Clear();
                //grid.Render((node, draw) =>
                //{
                //    if (node == source)
                //    {
                //        draw($"S");
                //    }
                //    else if(node == destination)
                //    {
                //        draw($"D{source.Value}");
                //    }
                //    else
                //    {
                //        draw(node.Value.ToString());
                //    }
                //}, null);

                destination.SetValue(source.Value);
                source.SetValue('.');
            }
            //Console.WriteLine($"Can move {move}");
        }
        else
        {
            //Console.WriteLine($"Cannot move {move}");
            return player;
        }
    }

    return MovePlayerTo(player, candidatePosition);
}

IEnumerable<Node<char>> GetAllGoods() => grid.Nodes().Where(n => n == 'O' || n == '[');

void RenderPlayer(Node<char> player) => Console.WriteLine($"Player: {player.Value} X:{player.X} Y:{player.Y}");

Node<char> MovePlayerTo(Node<char> player, Node<char> destination)
{
    destination.SetValue('@');
    player.SetValue('.');
    return destination;
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

