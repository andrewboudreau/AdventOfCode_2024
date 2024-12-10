using System.Numerics;

// https://adventofcode.com/2024/day/9

ReadOnlySpan<char> chars = Read()!.First();
var values = chars.ToArray().Select(x => x - '0').ToArray();
var capacity = values.Sum();

Console.WriteLine("Expanding '{0}' to {1}", chars.ToString(), capacity);

var maxFileId = chars.Length / 2;
Console.WriteLine($"Max file id: {maxFileId}");

var memory = new List<DiskNode>(capacity);
for (int i = 0; i < chars.Length; i++)
{
    int value = chars[i] - '0';
    for (int k = 0; k < value; k++)
    {
        var isFree = i % 2 == 1;
        var lengthIndex = isFree ? value - k : k;
        memory.Add(new(lengthIndex, isFree, i / 2));
    }
}


Dump(memory);

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
        memory[fileIndex] = new(1, true, 0);
        freeIndex++;
        fileIndex--;
    }
    //Dump(memory, capacity);
}

//Dump(memory);

BigInteger checksum = 0;
for (var i = 0; i <= fileIndex; i++)
{
    checksum = BigInteger.Add(checksum, (i * memory[i].FileId));
}

// 6346871685398
Console.WriteLine("Part1 checksum: {0}", checksum);

static void Dump(IReadOnlyList<DiskNode> memory)
{
    Console.WriteLine(
        string.Join("", memory
            .ToArray()
            .Select(x => $"[{(x.IsFree ? '.' : "" + x.FileId % 10 + "")}|{x.LengthIndex}]"))
    );
}

Console.WriteLine(SolvePart2(chars.ToString()));
static long SolvePart2(string diskMap)
{
    var disk = ParseDiskMap(diskMap);
    Console.WriteLine(string.Join("", disk.Select(x => x.HasValue ? x.Value.ToString() : ".")));
    var fileBlocks = GetFileBlockLengths(disk);
    MoveFiles(disk, fileBlocks);
    Console.WriteLine(string.Join("", disk.Select(x => x.HasValue ? x.Value.ToString() : ".")));
    return CalculateChecksum(disk);
}

static List<int?> ParseDiskMap(string diskMap)
{
    var disk = new List<int?>();
    bool isFile = true;
    int fileId = 0;

    foreach (var c in diskMap)
    {
        int length = c - '0';
        disk.AddRange(Enumerable.Repeat(isFile ? (int?)fileId : null, length));

        if (isFile)
            fileId++;
        isFile = !isFile;
    }

    return disk;
}

static Dictionary<int, int> GetFileBlockLengths(List<int?> disk)
{
    return disk
        .Where(block => block.HasValue)
        .GroupBy(block => block.Value)
        .ToDictionary(group => group.Key, group => group.Count());
}

static void MoveFiles(List<int?> disk, Dictionary<int, int> fileBlocks)
{
    foreach (var fileId in fileBlocks.Keys.OrderByDescending(id => id))
    {
        int length = fileBlocks[fileId];
        var freeSpaces = FindFreeSpaces(disk);

        // Get the current leftmost position of the file
        int fileStart = disk.IndexOf(fileId);

        // Check if there are free spaces to the left of the file
        var validFreeSpaces = freeSpaces.Where(f => f.Start < fileStart).ToList();

        if (!validFreeSpaces.Any())
        {
            // No free space to the left of the file, stop processing this file
            continue;
        }

        foreach (var (start, span) in validFreeSpaces)
        {
            if (span >= length)
            {
                // Clear old blocks
                for (int i = 0; i < disk.Count; i++)
                {
                    if (disk[i] == fileId)
                        disk[i] = null;
                }

                // Move file to the new free space
                for (int i = 0; i < length; i++)
                {
                    disk[start + i] = fileId;
                }

                break;
            }
        }
    }
}

static List<(int Start, int Length)> FindFreeSpaces(List<int?> disk)
{
    var freeSpaces = new List<(int Start, int Length)>();
    int? start = null;

    for (int i = 0; i < disk.Count; i++)
    {
        if (disk[i] == null)
        {
            if (start == null)
                start = i;
        }
        else if (start.HasValue)
        {
            freeSpaces.Add((start.Value, i - start.Value));
            start = null;
        }
    }

    if (start.HasValue)
    {
        freeSpaces.Add((start.Value, disk.Count - start.Value));
    }

    return freeSpaces;
}

static long CalculateChecksum(List<int?> disk)
{
    long checksum = 0;

    for (int i = 0; i < disk.Count; i++)
    {
        if (disk[i].HasValue)
        {
            checksum += i * disk[i].Value;
        }
    }

    return checksum;
}

class DiskNode(int lengthIndex, bool isFree, int file)
{
    public int LengthIndex { get; private set; } = lengthIndex;
    public bool IsFree { get; private set; } = isFree;
    public int FileId { get; private set; } = file;
}