using Kros.Extensions;
using System;
using System.Collections.Generic;

namespace Kros.Utils;

/// <summary>
/// Utility class for generating different things
/// </summary>
public static class Generator
{
    /// <summary>
    /// Creates a lazy IEnumerable of type T. In other words,
    /// the values are created on the fly during enumeration
    /// </summary>
    /// <param name="indexedGenerator">Value generator function with a passed index and expecting a return instance of T</param>
    /// <param name="count">Length of IEnumerable</param>
    /// <typeparam name="T">Type for IEnumerable</typeparam>
    /// <returns>Lazy IEnumerable of type T</returns>
    public static IEnumerable<T> GenerateEnumerableLazy<T>(Func<int, T> indexedGenerator, int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return indexedGenerator(i);
        }
    }

    /// <summary>
    /// Creates an eager IEnumerable of type T. In other words,
    /// the values are all created before returned
    /// </summary>
    /// <param name="indexedGenerator">Value generator function with a passed index and expecting a return instance of T</param>
    /// <param name="count">Length of IEnumerable</param>
    /// <typeparam name="T">Type for IEnumerable</typeparam>
    /// <returns>Lazy IEnumerable of type T</returns>
    public static IEnumerable<T> GenerateEnumerableEager<T>(Func<int, T> indexedGenerator, int count)
    {
        T[] arr = new T[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = indexedGenerator(i);
        }

        return arr;
    }

    /// <summary>
    /// Geneates a random string with the given chars
    /// </summary>
    /// <param name="chars">Chars to potentially include in the result string</param>
    /// <param name="length">Length of string to generate</param>
    /// <param name="random">Random instance to use</param>
    /// <returns></returns>
    public static string GenerateString(char[] chars, int length, Random random)
    {
        var str = new char[length];
        for (var i = 0; i < str.Length; i++)
        {
            str[i] = random.SelectFrom(chars);
        }

        return new string(str);
    }
}
