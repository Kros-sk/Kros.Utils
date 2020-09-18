using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kros.Extensions
{
    /// <summary>
    /// Extension for easier creating task from value.
    /// </summary>
    public static class AsTaskExtensions
    {
        /// <summary>
        /// Create <see cref="Task{T}"/> of <see cref="IEnumerable{T}"/> from items.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">The items.</param>
        public static Task<IEnumerable<T>> AsTask<T>(this IEnumerable<T> items)
            => Task.FromResult(items);

        /// <summary>
        /// Create <see cref="Task{T}"/> from value.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="value">The value.</param>
        public static Task<T> AsTaskSingleValue<T>(this T value)
            => Task.FromResult(value);
    }
}
