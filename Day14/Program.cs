// https://adventofcode.com/2024/day/14

var sample = false;

int width = sample ? 11 : 101;
int height = width == 11 ? 7 : 103;

var robots = Read(row =>
{
    var parts = row.Split(' ').Select(s => s.Split(',')).ToList();
    return new Particle(
        x: int.Parse(parts[0][0].Split('=')[1]),
        y: int.Parse(parts[0][1]),
        vx: int.Parse(parts[1][0].Split('=')[1]),
        vy: int.Parse(parts[1][1]),
        width,
        height);
}).ToList();

Render();

// write a method to give a single value to the total distance all robots are apart from each other
// the distance between two robots is the sum of the absolute values of the differences in their x and y coordinates
int TotalDistance()
{
    int total = 0;
    Parallel.For(0, robots.Count, i =>
    {
        for (int j = i + 1; j < robots.Count; j++)
        {
            Interlocked.Add(ref total, Math.Abs(robots[i].X - robots[j].X) + Math.Abs(robots[i].Y - robots[j].Y));
        }
    });
    return total;
}

var min = int.MaxValue;
for (var i = 0; i < 10_000; i++)
{
    var current = TotalDistance();
    if (current < min)
    {
        Console.WriteLine($"New min was found, down from {min} to {current} after {i} iterations");
        min = current;
    }

    StepAll();
}

//var step = 0;
//for (var i = 0; i < min; i++)
//{
//    StepAll();
//    step = i;
//}
Render();

//while (true)
//{
//    Console.WriteLine($"current step is {step} - Press 1, 2, 3, or 4 to advance 1, 10, 20, or 50 steps respectively, or any other key to exit.");
//    var key = Console.ReadKey(intercept: true).KeyChar;

//    int steps = key switch
//    {
//        '1' => 1,
//        '2' => 10,
//        '3' => 20,
//        '4' => 50,
//        _ => 0
//    };
//    step += steps;
//    if (steps == 0) break;

//    for (int i = 0; i < steps; i++)
//    {
//        StepAll();
//    }

//    Render();
//}

void StepAll() => Parallel.ForEach(robots, robot => robot.Move());

void Render()
{
    var grid = new char[height, width];
    foreach (var robot in robots)
    {
        grid[robot.Y, robot.X] = '█';
    }
    Console.Clear();
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            Console.Write(grid[y, x] == '█' ? '█' : '.');
        }
        Console.WriteLine();
    }
}

////print out the number of robots in each quadrant
var counts = robots.Where(r => r.Quadrant() > 0).GroupBy(r => r.Quadrant()).ToDictionary(g => g.Key, g => g.Count());
Console.WriteLine(string.Join(", ", counts.Where(c => c.Key > 0).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value)));

//// multiple the number of drones in each quandrant to get the answer
Console.WriteLine(counts.Values.Aggregate(1, (a, b) => a * b));

class Particle(int x, int y, int vx, int vy, int width, int height)
{
    public int X { get; private set; } = x;
    public int Y { get; private set; } = y;
    public int VX { get; private set; } = vx;
    public int VY { get; private set; } = vy;
    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;

    // The quadrant is a calculated by dividing the grid into 4 equal parts, if the particle is in the top left quadrant, the quadrant is 1, if it is in the top right quadrant, the quadrant is 2, if it is in the bottom left quadrant, the quadrant is 3, and if it is in the bottom right quadrant, the quadrant is 4.

    public int Quadrant()
    {
        if (X == Width / 2 || Y == Height / 2)
        {
            return 0;
        }

        return
            X < Width / 2 ?
                Y < Height / 2 ?
                    1 :
                    3 :
                Y < Height / 2 ?
                    2 :
                    4;
    }

    public void Move()
    {
        X = (X + VX + Width) % Width;
        Y = (Y + VY + Height) % Height;
    }
}
