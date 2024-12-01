## AsIntegers(string source)
This method converts a string into a sequence of integers, where each character is treated as a separate digit.

Example Usage:
```csharp
string input = "12345";
var result = ReadInputs.AsIntegers(input).ToList();
// result is a List<int> with values [1, 2, 3, 4, 5]
```

## ReadAsRowsOfIntegers()
Reads lines from an input source and converts each line into a sequence of integers.

Example Usage:
```csharp
// Assuming an input file with lines like "123", "456"
foreach (var row in ReadInputs.ReadAsRowsOfIntegers())
{
    // Process each row as IEnumerable<int>
}
```

## Read<T>(Func<string, T> factory)
Reads lines from an input source and applies a factory function to convert each line into a specific type T.

Example Usage:
```csharp
// Converting each line to an integer
foreach (var number in ReadInputs.Read(int.Parse))
{
    // number is of type int
}
```

## ReadRecords<T>(Func<string[], T> factory)
Processes a sequence of lines as records, separated by empty lines, and applies a factory function to each record.

Example Usage:
```csharp
// Assuming input like "abc", "def", "", "ghi", "jkl"
foreach (var record in ReadInputs.ReadRecords(lines => string.Join(",", lines)))
{
    // record is a string of concatenated lines
}
```

## ReadIntegers()
Reads lines from an input source and converts each line to an integer.

Example Usage:
```csharp
// Assuming an input file with lines like "123", "456"
foreach (var number in ReadInputs.ReadIntegers())
{
    // number is of type int
}
```

## Read()
Yields lines from the input source, either from a file or from the console.

Example Usage:
```csharp
foreach (var line in ReadInputs.Read())
{
    // Process each line
}
```

## ReadTo<T>(Func<IEnumerable<string?>, T> factory)
Reads all lines from an input source and applies a factory function to the entire sequence of lines.

Example Usage:
```csharp
Copy code
// Concatenating all lines into a single string
var result = ReadInputs.ReadTo(lines => string.Join(" ", lines));
```
