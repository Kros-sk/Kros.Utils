using System;

namespace Kros.Extensions
{
    /// <summary>
    /// Extension methods for numbers.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Rounds a decimal value to the nearest integer.
        /// Numbers in midway between two numbers are rounded
        /// using <see cref="MidpointRounding">MidpointRounding.AwayFromZero</see> method.
        /// This is the difference from <c>Math.Round</c>, where the default method is
        /// <see cref="MidpointRounding">MidpointRounding.ToEven</see>.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d)
            => Math.Round(d, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Rounds a decimal value to a specified number of fractional digits.
        /// Numbers in midway between two numbers are rounded
        /// using <see cref="MidpointRounding">MidpointRounding.AwayFromZero</see> method.
        /// This is the difference from <c>Math.Round</c>, where the default method is
        /// <see cref="MidpointRounding">MidpointRounding.ToEven</see>.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d, int decimals)
            => Math.Round(d, decimals, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Rounds a decimal value to the nearest integer. A <paramref name="mode"/> specifies how to round
        /// the value if it is midway between two numbers.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="mode">Specification for how to round <paramref name="d"/> if it is midway between two other numbers.
        /// </param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d, MidpointRounding mode)
            => Math.Round(d, mode);

        /// <summary>
        /// Rounds a decimal value to the nearest integer. A <paramref name="mode"/> specifies how to round
        /// the value if it is midway between two numbers.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <param name="mode">Specification for how to round <paramref name="d"/> if it is midway between two other numbers.
        /// </param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d, int decimals, MidpointRounding mode)
            => Math.Round(d, decimals, mode);

        /// <summary>
        /// Rounds a decimal value to the nearest integer.
        /// Numbers in midway between two numbers are rounded
        /// using <see cref="MidpointRounding">MidpointRounding.AwayFromZero</see> method.
        /// This is the difference from <c>Math.Round</c>, where the default method is
        /// <see cref="MidpointRounding">MidpointRounding.ToEven</see>.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <returns>Rounded decimal number.</returns>
        public static double Round(this double d)
            => Math.Round(d, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Rounds a decimal value to a specified number of fractional digits.
        /// Numbers in midway between two numbers are rounded
        /// using <see cref="MidpointRounding">MidpointRounding.AwayFromZero</see> method.
        /// This is the difference from <c>Math.Round</c>, where the default method is
        /// <see cref="MidpointRounding">MidpointRounding.ToEven</see>.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>Rounded decimal number.</returns>
        public static double Round(this double d, int decimals)
            => Math.Round(d, decimals, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Rounds a decimal value to the nearest integer. A <paramref name="mode"/> specifies how to round
        /// the value if it is midway between two numbers.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="mode">Specification for how to round <paramref name="d"/> if it is midway between two other numbers.
        /// </param>
        /// <returns>Rounded decimal number.</returns>
        public static double Round(this double d, MidpointRounding mode)
            => Math.Round(d, mode);

        /// <summary>
        /// Rounds a decimal value to the nearest integer. A <paramref name="mode"/> specifies how to round
        /// the value if it is midway between two numbers.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <param name="mode">Specification for how to round <paramref name="d"/> if it is midway between two other numbers.
        /// </param>
        /// <returns>Rounded decimal number.</returns>
        public static double Round(this double d, int decimals, MidpointRounding mode)
            => Math.Round(d, decimals, mode);
    }
}
