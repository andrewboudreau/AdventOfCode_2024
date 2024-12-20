﻿using System.Collections;

namespace Day00
{
    public static partial class EnumerableExtensions
    {
        public static int ToInt32(this string bits)
            => ToInt32(bits.Select(x => x == '1'));

        public static int ToInt32(this IEnumerable<int> bits)
            => ToInt32(bits.Select(x => x > 0));

        public static int ToInt32(this IEnumerable<bool> bools)
            => ToInt32(new BitArray(bools.Reverse().ToArray()));

        public static int ToInt32(this BitArray bits)
        {
            var bytes = new int[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        public static IEnumerable<int> SplitToInts(this IEnumerable<string> source, char split)
            => source.SelectMany(
                x => x.Split(split, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), 
                (a, b) => int.Parse(b));

        public static T Second<T>(this IEnumerable<T> source) => source.ElementAt(1);
        public static T Third<T>(this IEnumerable<T> source) => source.ElementAt(2);

        public static IEnumerable<T> WithoutElementAt<T>(this IEnumerable<T> source, int index)
            => source.Where((_, i) => i != index);
    }
}
