﻿// https://adventofcode.com/2024/day/18

var locations = Read(str =>
{
    var parts = str.Split(",");
    return (X: int.Parse(parts[0]), Y: int.Parse(parts[1]));
});

var gridSize = locations.Max(n => n.X) <= 7 ? 7 : 71;
var bytesToRead = gridSize == 7 ? 12 : 1024;

var range = Enumerable.Range(0, gridSize).Select(x => Enumerable.Range(0, gridSize).Select(x => '.'))!;
var grid = new Grid<char>(range);

locations.Take(bytesToRead).ToList().ForEach(x => grid[x.X, x.Y]?.SetValue('#'));
grid.SetNeighbors(n => n.Value == '.');

grid.FillDistances(from: grid[0, 0]!);
grid.ResetVisited();
grid.Render(null);

//grid.Render((node, draw) => draw(node.Value != '.' ? $"[{node.Value}  ]" : $"[{node.Distance % 1000:000}]"), null);
Console.WriteLine($"Shortest path is {grid[gridSize - 1, gridSize - 1]!.Distance}");