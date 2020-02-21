using System;

namespace Kros.Extensions
{
    /// <summary>
    /// Extension methods for numbers.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Returns rounded decimal value.
        /// When a number is halfway between two others, it is rounded toward the nearest
        /// number that is away from zero.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d, int decimals)
            => Math.Round(d, decimals, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Returns rounded decimal value.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <param name="mode">Specification for how to round d if it is midway between two other numbers.</param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d, int decimals, MidpointRounding mode)
            => Math.Round(d, decimals, mode);

        /// <summary>
        /// Returns rounded decimal value.
        /// When a number is halfway between two others, it is rounded toward the nearest
        /// number that is away from zero.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d)
            => Round(d, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Rounds a decimal value to the nearest integer. A parameter specifies how to round
        /// the value if it is midway between two numbers.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="mode">Specification for how to round d if it is midway between two other numbers.</param>
        /// <returns>Rounded decimal number.</returns>
        public static decimal Round(this decimal d, MidpointRounding mode)
            => Math.Round(d, mode);

        /// <summary>
        /// Returns rounded double value.
        /// When a number is halfway between two others, it is rounded toward the nearest
        /// number that is away from zero.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <param name="digits">The number of decimal places in the return value.</param>
        /// <returns>Rounded double number.</returns>
        public static double Round(this double d, int digits)
            => Math.Round(d, digits, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Returns rounded double value.
        /// </summary>
        /// <param name="d">A double number to be rounded.</param>
        /// <param name="digits">The number of decimal places in the return value.</param>
        /// <param name="mode">Specification for how to round d if it is midway between two other numbers.</param>
        /// <returns>Rounded double number.</returns>
        public static double Round(this double d, int digits, MidpointRounding mode)
            => Math.Round(d, digits, mode);

        /// <summary>
        /// Returns rounded double value.
        /// When a number is halfway between two others, it is rounded toward the nearest
        /// number that is away from zero.
        /// </summary>
        /// <param name="d">A decimal number to be rounded.</param>
        /// <returns>Rounded double number.</returns>
        public static double Round(this double d)
            => Round(d, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Rounds a double-precision floating-point value to the nearest integer. A parameter
        /// specifies how to round the value if it is midway between two numbers.
        /// </summary>
        /// <param name="d">A double-precision floating-point number to be rounded.</param>
        /// <param name="mode">Specification for how to round value if it is midway between two other numbers.</param>
        /// <returns>Rounded double number.</returns>
        public static double Round(this double d, MidpointRounding mode)
            => Math.Round(d, mode);
    }
}
