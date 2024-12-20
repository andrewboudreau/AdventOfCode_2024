// https://adventofcode.com/2024/day/17

using System.Numerics;

int[] program = default!;

var computer = ReadTo(str =>
{
    var state = str.ToList();
    program = [.. state[4]!.Split(":")[1].Split(",").Select(int.Parse)];

    var registerA = BigInteger.Parse(state[0]!.Split(":")[1]);
    return new Computer(registerA, program);
});

Console.WriteLine("start: " + string.Join(",", program));
Console.WriteLine("  end: " + computer.Run().Output);
Console.WriteLine();
Console.WriteLine(Find(program, 0));

BigInteger Find(int[] target, BigInteger ans)
{
    if (target.Length == 0)
    {
        return ans;
    }

    foreach (var t in Enumerable.Range(0, 8))
    {
        BigInteger a = (ans << 3) | (long)t;
        BigInteger b = 0;
        BigInteger c = 0;
        BigInteger output = -1;

        BigInteger Combo(int operand) => operand switch
        {
            4 => a,
            5 => b,
            6 => c,
            _ => operand
        };

        for (var ptr = 0; ptr < program.Length - 2; ptr += 2)
        {
            var opcode = program[ptr];
            int operand = program[ptr + 1];

            if (opcode == 1)
            {
                b ^= operand;
            }
            else if (opcode == 2)
            {
                b = Combo(operand) % 8;
            }
            else if (opcode == 4)
            {
                b ^= c;
            }
            else if (opcode == 5)
            {
                output = Combo(operand) % 8;
            }
            else if (opcode == 6)
            {
                throw new NotImplementedException();
            }
            else if (opcode == 7)
            {
                c = a >> (int)Combo(operand);
            }

            if ((int)output == target[^1])
            {
                var sub = Find(target[..^1], a);
                if (sub == -1)
                {
                    continue;
                }

                return sub;
            }
        }
    }

    return -1;

    // 2,4,1,1,7,5,0,3,4,3,1,6,5,5,3,0

    // Problem Program
    // 2,4  // B = A % 8
    // 1,1  // B = B ^ 1
    // 7,5  // C = A >> B
    // 0,3  // A = A >> 3
    // 4,3  // B = B ^ C
    // 1,6  // B = B ^ 0b0110

    // 5,5  // output.Add(B % 8)
    // 3,0  // If RegisterA != 0 Restart the InstructionPointer
}


class Computer(BigInteger registerA, int[] program)
{
    private readonly List<int> output = new(program.Length);
    private readonly int[] program = program;

    private BigInteger A = registerA;
    private BigInteger B;
    private BigInteger C;
    private int InstructionPointer;

    public string Output => string.Join(",", output);
    public string Program => string.Join(",", program);

    public bool IsOutputEqualInput => output.SequenceEqual(program);

    public Computer Tick()
    {
        var opcode = program[InstructionPointer++];
        var operand = program[InstructionPointer++];

        var combo = operand switch
        {
            4 => A,
            5 => B,
            6 => C,
            _ => (ulong)operand
        };

        if (opcode == 0)
        {
            A >>= (int)combo;
        }
        else if (opcode == 1)
        {
            B ^= (ulong)operand;
        }
        else if (opcode == 2)
        {
            B = combo % 8;
        }
        else if (opcode == 3 && A != 0)
        {
            InstructionPointer = operand;
        }
        else if (opcode == 4)
        {
            B ^= C;
        }
        else if (opcode == 5)
        {
            output.Add((int)(combo % 8));
        }
        else if (opcode == 6)
        {
            B = A >> (int)combo;
        }
        else if (opcode == 7)
        {
            C = A >> (int)combo;
        }

        return this;
    }

    public Computer Run()
    {
        while (InstructionPointer < program.Length && output.Count <= program.Length)
        {
            _ = Tick();
        }

        return this;
    }
}