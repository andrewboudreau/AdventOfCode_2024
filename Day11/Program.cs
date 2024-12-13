// https://adventofcode.com/2024/day/11

/*
 * Every time you blink, the stones each simultaneously change 
 * according to the first applicable rule in this list:
 * 
 * If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
 * 
 * If the stone is engraved with a number that has an even number of digits, it is replaced by two stones. 
 * The left half of the digits are engraved on the new left stone, and the right half of the digits 
 * are engraved on the new right stone. (The new numbers don't keep extra leading zeroes: 1000 would 
 * become stones 10 and 0.)
 * 
 * If none of the other rules apply, the stone is replaced by a new stone; 
 * the old stone's number multiplied by 2024 is engraved on the new stone.
 */

using System.Numerics;

var stones = new LinkedList<BigInteger>(Read().First()!.Split(' ').Select(BigInteger.Parse));
Console.WriteLine(string.Join(" ", stones));

var blinks = 25;
var memorize = new Dictionary<int, BigInteger>();
foreach (var stone in stones)
{
    var foo = new LinkedList<BigInteger>([stone]);
    for (var blink = 0; blink < blinks; blink++)
    {
        var current = foo.First;
        while (current != null)
        {
            if (current.Value == 0)
            {
                current.Value = 1;
                current = current.Next;
            }
            else if (current.Value.ToString().Length % 2 == 0)
            {
                var mid = current.Value.ToString().Length / 2;
                var left = BigInteger.Parse(current.Value.ToString()[..mid]);
                var right = BigInteger.Parse(current.Value.ToString()[mid..]);

                current.Value = left;
                current = stones.AddAfter(current, right).Next;
            }
            else
            {
                current.Value *= 2024;
                current = current.Next;
            }
        }
        memorize.Add(stone, foo.Count);
    }
}

for (var blink = 0; blink < blinks; blink++)
{
    var stone = stones.First;
    while (stone != null)
    {
        if (stone.Value == 0)
        {
            stone.Value = 1;
            stone = stone.Next;
        }
        else if (stone.Value.ToString().Length % 2 == 0)
        {
            var mid = stone.Value.ToString().Length / 2;
            var left = BigInteger.Parse(stone.Value.ToString()[..mid]);
            var right = BigInteger.Parse(stone.Value.ToString()[mid..]);

            stone.Value = left;
            stone = stones.AddAfter(stone, right).Next;
        }
        else
        {
            stone.Value *= 2024;
            stone = stone.Next;
        }
    }

    Console.WriteLine("There are " + stones.Count + " stones after " + (blink + 1) + " blinks.");
}

