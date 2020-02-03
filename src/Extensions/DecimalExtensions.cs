using System;

namespace Kros.Extensions
{
    /// <summary>
    /// Extension methods for decimal <see cref="decimal"/>.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Returns rounded decimal value.
        /// When a number is halfway between two others, it is rounded toward the nearest
        /// number that is away from zero.
        /// </summary>
        /// <param name="value">A decimal number to be rounded.</param>
        /// <param name="decimalPlaces">The number of decimal places in the return value.</param>
        /// <returns>Decimal.</returns>
        public static decimal Round(this decimal value, int decimalPlaces)
            => Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
    }
}
