// https://adventofcode.com/2024/day/7
// part1: 6083020304036

using System.Numerics;

var equations = Read(line =>
{
    var parts = line.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    var operands = parts[1]
        .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse);

    return new Equation(operands, BigInteger.Parse(parts[0]));
}).ToArray();

int count = 0;
BigInteger part1 = 0;
foreach (var candidate in equations)
{
    foreach (var operators in candidate.OperatorPermutations())
    {
        if (candidate.Evaluate(operators) == candidate.Result)
        {
            Console.Write("{0,13}= ", candidate.Result);
            var eq = candidate.Operands
                .Zip(
                    operators,
                    (operand, operation) => $"{operand}{(operation == Operator.Add ? "+" : "*")}")
                .Append(candidate.Operands.Last().ToString())
                .ToList();
            Console.WriteLine(string.Join("", eq));

            part1 += candidate.Result;
            count++;
            break;
        }
    }
}

Console.WriteLine("{0} : is the Part1 checksum", part1);
Console.WriteLine("Found {0} solvable inputs", count);

enum Operator { Add, Multiply }

class Equation
{
    public Equation(IEnumerable<int> operands, BigInteger result)
    {
        Operands = operands.ToArray();
        Result = result;
    }

    public int[] Operands { get; }

    public BigInteger Result { get; }

    public IEnumerable<Operator[]> OperatorPermutations() => OperatorPermutations(Operands);

    public BigInteger Evaluate(Operator[] operators)
    {
        BigInteger result = Operands[0];
        for (var i = 0; i < operators.Length; i++)
        {
            result = operators[i] switch
            {
                Operator.Add => result + Operands[i + 1],
                Operator.Multiply => result * Operands[i + 1],
                _ => throw new ArgumentOutOfRangeException(nameof(operators), "Invalid operator encountered")
            };

            if (result > Result)
            {
                return result;
            }
        }
        return result;
    }

    private static IEnumerable<Operator[]> OperatorPermutations(int[] operands)
    {
        if (operands.Length == 1)
        {
            yield return [];
        }
        else
        {
            foreach (var op in Enum.GetValues<Operator>())
            {
                foreach (var rest in OperatorPermutations(operands.Skip(1).ToArray()))
                {
                    yield return [op, .. rest];
                }
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            $"{string.Join(" ", Operands)} = {Result}");
        sb.AppendLine("Operator Combinations:");
        sb.AppendLine(string.Join(Environment.NewLine, OperatorPermutations().Select(x => string.Join(" ", x))));

        return sb.ToString();
    }
}
