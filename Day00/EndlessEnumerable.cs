using System.Collections;

namespace Day00;

public sealed class EndlessEnumerable<T> : IEnumerable<T>, IDisposable
{
    private readonly IEnumerable<T> source;

    private readonly IEnumerator<T> enumerator;

    public EndlessEnumerable(IEnumerable<T>? source)
    {
        this.source = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
        this.enumerator = GetEnumerator();
    }

    public T Current => enumerator.Current;

    public T Next()
    {
        enumerator.MoveNext();
        return enumerator.Current;
    }

    public void Dispose()
    {
        enumerator.Dispose();
    }

    public IEnumerator<T> GetEnumerator()
    {
        while (true)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
