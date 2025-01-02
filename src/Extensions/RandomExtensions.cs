using System;
using System.Collections.Generic;

namespace Kros.Extensions;

/// <summary>
/// Extension methods for randomness
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Randomly selects an element from the given IList
    /// </summary>
    /// <param name="random">The random instance to use</param>
    /// <param name="list">List to select from</param>
    /// <typeparam name="T">Generic type of the IList</typeparam>
    /// <returns>Randomly selected element from the IList</returns>
    public static T SelectFrom<T>(this Random random, IList<T> list)
        => list[random.Next(list.Count)];

    /// <summary>
    /// Shuffles an IList
    /// </summary>
    /// <param name="random">Random instance to use</param>
    /// <param name="list">List to shuffle</param>
    /// <typeparam name="T">Generic type of the IList</typeparam>
    public static void Shuffle<T>(this Random random, IList<T> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var randomIndex = random.Next(list.Count);
            var t = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = t;
        }
    }
}
