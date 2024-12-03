// https://adventofcode.com/2024/day/3

/*
 * Part1:
 * It seems like the goal of the program is just to multiply some numbers. 
 * It does that with instructions like mul(X,Y), where X and Y are each 1-3 digit numbers. 
 * For instance, mul(44,46) multiplies 44 by 46 to get a result of 2024. Similarly, mul(123,4) 
 * would multiply 123 by 4.
 * 
 * many invalid characters that should be ignored, even if they look like part of a mul instruction. 
 * Sequences like mul(4*, mul(6,9!, ?(12,34), or mul ( 2 , 4 ) do nothing.
 * 
 * Part2:
 * There are two new instructions you'll need to handle:
 * 
 * The do() instruction enables future mul instructions.
 * The don't() instruction disables future mul instructions.
 * 
 * Only the most recent do() or don't() instruction applies. At the beginning of the program, mul instructions are enabled.
 */

var input = string.Join("", Read());

Lexer lexer = new();
var tokens = lexer.Tokenize(input);
Console.WriteLine(string.Join(Environment.NewLine, tokens));

Parser parser = new(tokens);
var instructions = parser.Parse().ToArray();
Console.WriteLine(string.Join(Environment.NewLine, instructions));

var partOne = new Evaluator();
foreach (var instruction in instructions)
{
    Console.WriteLine($"Evaluated: {instruction} = {partOne.Evaluate(instruction)}");
}
var sum = instructions.Sum(x => partOne.Evaluate(x));
Console.WriteLine($"Found {instructions.Length} expressions, Part1: {sum}");

var partTwo = new ConditionalEvaluator();
var conditionalSum = instructions.Sum(x => partTwo.Evaluate(x));
Console.WriteLine($"Found {instructions.Length} expressions, Part2: {conditionalSum}");

class Lexer()
{
    private int position;

    public IReadOnlyList<Token> Tokenize(ReadOnlySpan<char> input)
    {
        var tokens = new List<Token>();
        while (position < input.Length)
        {
            var ch = input[position];
            if (IsLetter(ch))
            {
                var start = position;
                var identifier = ReadIdentifier(input);
                tokens.Add(new Token(TokenType.Identifier, identifier, new Range(start, position - 1)));
            }
            else if (char.IsDigit(ch))
            {
                var start = position;
                var number = ReadNumber(input);
                tokens.Add(new Token(TokenType.Number, number, new Range(start, position - 1)));
            }
            else if (IsSymbol(ch))
            {
                tokens.Add(new Token(TokenType.Symbol, ch.ToString(), new Range(position, ++position - 1)));
            }
            else
            {
                tokens.Add(new Token(TokenType.Invalid, ch.ToString(), new Range(position, ++position - 1)));
            }
        }

        return tokens.AsReadOnly();
    }

    private string ReadIdentifier(ReadOnlySpan<char> input)
    {
        var sb = new StringBuilder();
        while (position < input.Length && IsLetter(input[position]))
        {
            sb.Append(input[position]);
            position++;
        }
        return sb.ToString();
    }

    private string ReadNumber(ReadOnlySpan<char> input)
    {
        var sb = new StringBuilder();
        while (position < input.Length && char.IsDigit(input[position]))
        {
            sb.Append(input[position]);
            position++;
        }

        return sb.ToString();
    }

    private static bool IsSymbol(char c)
    {
        return c == '(' || c == ')' || c == ',';
    }

    private static bool IsLetter(char c)
    {
        return char.IsLetter(c) || c == '\'';
    }
}

enum TokenType
{
    Invalid,
    Identifier,
    Symbol,
    Number
}

readonly struct Token(TokenType type, string value, Range range)
{
    public TokenType Type { get; } = type;

    public string Value { get; } = value;

    public int NumberValue { get; } = int.TryParse(value, out int numberValue) ? numberValue : 0;

    public Range Range { get; } = range;

    public override string ToString()
    {
        return $"{'[' + Range.ToString() + ']',-10}{Type.ToString() + ':',-12} '{Value}'";
    }
}

class Parser(IReadOnlyList<Token> tokens)
{
    private readonly IReadOnlyList<Token> tokens = tokens;
    private int position;

    public IEnumerable<Instruction> Parse()
    {
        while (position < tokens.Count)
        {
            if (IsValidMulInstruction(out Instruction mul))
            {
                yield return mul;
                position += 6;
            }
            else if (IsValidControlInstruction(out Instruction control))
            {
                yield return control;
                position += 3;
            }
            else
            {
                position++;
            }
        }
    }

    private bool IsValidControlInstruction(out Instruction instruction)
    {
        // requires atleast 2 more tokens to make a valid control expression
        if (position + 2 >= tokens.Count)
        {
            instruction = Instruction.Invalid;
            return false;
        }

        if (tokens[position].Type == TokenType.Identifier && tokens[position].Value.EndsWith("do")
            && tokens[position + 1].Type == TokenType.Symbol && tokens[position + 1].Value == "("
            && tokens[position + 2].Type == TokenType.Symbol && tokens[position + 2].Value == ")")
        {
            instruction = new Instruction(InstructionType.Do);
            return true;
        };

        if (tokens[position].Type == TokenType.Identifier && tokens[position].Value.EndsWith("don't")
            && tokens[position + 1].Type == TokenType.Symbol && tokens[position + 1].Value == "("
            && tokens[position + 2].Type == TokenType.Symbol && tokens[position + 2].Value == ")")
        {
            instruction = new Instruction(InstructionType.Dont);
            return true;
        }

        instruction = Instruction.Invalid;
        return false;
    }

    private bool IsValidMulInstruction(out Instruction instruction)
    {
        // requires atleast 5 more tokens to make a valid mul expression
        if (position + 5 >= tokens.Count)
        {
            instruction = Instruction.Invalid;
            return false;
        }

        if (tokens[position].Type == TokenType.Identifier && tokens[position].Value.EndsWith("mul")
            && tokens[position + 1].Type == TokenType.Symbol && tokens[position + 1].Value == "("
            && tokens[position + 2].Type == TokenType.Number
            && tokens[position + 3].Type == TokenType.Symbol && tokens[position + 3].Value == ","
            && tokens[position + 4].Type == TokenType.Number
            && tokens[position + 5].Type == TokenType.Symbol && tokens[position + 5].Value == ")")
        {
            instruction = new Instruction(InstructionType.Mul, tokens[position + 2].NumberValue, tokens[position + 4].NumberValue);
            return true;
        }

        instruction = Instruction.Invalid;
        return false;
    }

}

readonly struct Instruction(InstructionType operation, int? firstOperand = null, int? secondOperand = null)
{
    public InstructionType Operation { get; } = operation;
    public int? FirstOperand { get; } = firstOperand;
    public int? SecondOperand { get; } = secondOperand;

    public override string ToString()
    {
        if (FirstOperand.HasValue && SecondOperand.HasValue)
            return $"{Operation}({FirstOperand}, {SecondOperand})";
        else
            return $"{Operation}()";
    }

    public static Instruction Invalid { get; } = new(InstructionType.Invalid);
}

enum InstructionType
{
    Invalid,
    Mul,
    Do,
    Dont
}

class Evaluator
{
    public int Evaluate(Instruction instruction)
    {
        if (instruction.Operation == InstructionType.Mul)
        {
            return instruction.FirstOperand!.Value * instruction.SecondOperand!.Value;
        }

        return 0;
    }
}

class ConditionalEvaluator
{
    private bool isEnabled = true;

    public int Evaluate(Instruction instruction)
    {
        if (isEnabled && instruction.Operation == InstructionType.Mul)
        {
            return instruction.FirstOperand!.Value * instruction.SecondOperand!.Value;
        }
        else if (instruction.Operation == InstructionType.Do)
        {
            isEnabled = true;
        }
        else if (instruction.Operation == InstructionType.Dont)
        {
            isEnabled = false;
        }

        return 0;
    }
}
