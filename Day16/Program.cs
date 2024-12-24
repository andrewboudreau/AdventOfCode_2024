// https://adventofcode.com/2024/day/16

var grid = new Grid<char>(Read()!, str => str.ToCharArray());

var start = grid.First(n => n == 'S');
var end = grid.First(n => n == 'E');

grid.SetNeighbors(n => n == '.' || n == 'E' || n == 'S');
grid.FillDistances(start);

grid.Render(null);
Console.WriteLine();
grid.Render((node, draw) => draw(node.Value == '#' ? "_" : node.Value == '.' ? node.Neighbors.Count().ToString() : node.Value.ToString()) , null);
Console.WriteLine();

grid.RenderDistances();
