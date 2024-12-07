using System.Numerics;

// https://adventofcode.com/2024/day/7

var equations = Read(line =>
{
    var parts = line.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    var operands = parts[1]
        .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse);

    return new Equation(operands, BigInteger.Parse(parts[0]));
}).ToArray();

BigInteger checksum = 0;
Lock lockObj = new();

// Parallel Solution
Parallel.ForEach(equations, candidate =>
{
    foreach (var operators in candidate.OperatorPermutations())
    {
        if (candidate.Evaluate(operators) == candidate.Result)
        {
            using (lockObj.EnterScope())
            {
                checksum = BigInteger.Add(checksum, candidate.Result);
            };
            break;
        }
    }
});
Console.WriteLine("{0} : is the checksum", checksum);

// Sequential Solution
checksum = 0;
int count = 0;
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
                    (operand, operation) =>
                        $"{operand}{OperatorToString(operation)}")
                .Append(candidate.Operands.Last().ToString())
                .ToList();
            Console.WriteLine(string.Join("", eq));

            checksum += candidate.Result;
            count++;
            break;
        }
    }
}
Console.WriteLine("{0} : is the checksum", checksum);
Console.WriteLine("Found {0} solvable inputs", count);

static string OperatorToString(Operator op) => op switch
{
    Operator.Add => "+",
    Operator.Multiply => "*",
    Operator.Concatenate => "||",
    _ => throw new ArgumentOutOfRangeException(nameof(op), "Invalid operator encountered")
};

enum Operator { Add, Multiply, Concatenate }

class Equation(IEnumerable<int> operands, BigInteger result)
{
    public int[] Operands { get; } = operands.ToArray();

    public BigInteger Result { get; } = result;

    public IEnumerable<Operator[]> OperatorPermutations() => OperatorPermutations(Operands);

    public BigInteger Evaluate(Operator[] operators)
    {
        BigInteger result = Operands[0];
        for (var i = 0; i < operators.Length; i++)
        {
            result = operators[i] switch
            {
                Operator.Add => BigInteger.Add(result, Operands[i + 1]),
                Operator.Multiply => BigInteger.Multiply(result, Operands[i + 1]),
                Operator.Concatenate => BigInteger.Parse($"{result}{Operands[i + 1]}"),
                _ => throw new ArgumentOutOfRangeException(nameof(operators), "Invalid operator encountered")
            };

            if (result > Result)
            {
                return int.MinValue;
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
