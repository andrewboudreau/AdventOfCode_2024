// https://adventofcode.com/2024/day/9

using System.Numerics;

ReadOnlySpan<char> chars = Read()!.First();
Console.WriteLine("Expanding '{0}'", chars.ToString());

var maxFileId = chars.Length / 2;
Console.WriteLine($"Max file id: {maxFileId}");

int capacity = 0;
Span<(bool IsFree, int File)> memory = stackalloc (bool, int)[chars.Length * 9];

for (int i = 0; i < chars.Length; i++)
{
    int value = chars[i] - '0';
    for (int k = 0; k < value; k++)
    {
        memory[capacity++] = (i % 2 == 1, i / 2);
    }
}

Dump(memory, capacity);

int fileIndex = capacity - 1;
int freeIndex = 0;

while (freeIndex < fileIndex)
{
    while (freeIndex < capacity && !memory[freeIndex].IsFree)
    {
        freeIndex++;
    }

    while (fileIndex >= 0 && memory[fileIndex].IsFree)
    {
        fileIndex--;
    }

    if (fileIndex >= 0 && freeIndex < capacity && freeIndex < fileIndex)
    {
        memory[freeIndex] = memory[fileIndex];
        memory[fileIndex] = (true, 0);
        freeIndex++;
        fileIndex--;
    }
    //Dump(memory, capacity);
}

Dump(memory, capacity);

BigInteger checksum = 0;
for (var i = 0; i <= fileIndex; i++)
{
    checksum = BigInteger.Add(checksum, (i * memory[i].File));
}

// 6346871685398
Console.WriteLine("Part1 checksum: {0}", checksum);

static void Dump(Span<(bool IsFree, int File)> memory, int capacity)
{
    Console.WriteLine(
        string.Join("", memory[..capacity]
            .ToArray()
            .Select(x => $"{(x.IsFree ? '.' : "" + x.File % 10 + "")}"))
    );
}