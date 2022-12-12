using System;

namespace Kros.Utils
{
    /// <summary>
    /// Class for "freezing" date and time to constant value. Usable for example in tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Current time is accessible in <see cref="Now"/> property. Own time can be injected using
    /// <see cref="InjectActualDateTime(DateTime)"/>.
    /// </para>
    /// <code language="cs" source="..\..\Documentation\Examples\Kros.Utils\DateTimeProviderExamples.cs" region="BasicExample"/>
    /// <para>
    /// Set value is valid for current thread only, so it is possible to have different values in different threads.
    /// </para>
    /// </remarks>
    /// <seealso cref="System.IDisposable" />
    public sealed class DateTimeProvider : IDisposable
    {
        [ThreadStatic]
        private static DateTime? _injectedDateTime;

        [ThreadStatic]
        private static DateTimeOffset? _injectedDateTimeOffset;

        private DateTimeProvider()
        {
        }

        /// <summary>
        /// Returns own date and time, if it was set by <see cref="InjectActualDateTime(DateTime)"/>. If it was not set,
        /// <see cref="DateTime.Now">DateTime.Now</see> is returned.
        /// </summary>
        public static DateTime Now => _injectedDateTime?.ToLocalTime() ?? DateTime.Now;

        /// <summary>
        /// Returns own date and time, if it was set by <see cref="InjectActualDateTime(DateTime)"/>. If it was not set,
        /// <see cref="DateTime.UtcNow">DateTime.UtcNow</see> is returned.
        /// </summary>
        public static DateTime UtcNow => _injectedDateTime ?? DateTime.UtcNow;

        /// <summary>
        /// Returns own date and time, if it was set by <see cref="InjectActualDateTime(DateTimeOffset)"/>.
        /// If it was not set, <see cref="DateTimeOffset.Now">DateTimeOffset.Now</see> is returned.
        /// </summary>
        public static DateTimeOffset DateTimeOffsetNow => _injectedDateTimeOffset?.ToLocalTime() ?? DateTimeOffset.Now;

        /// <summary>
        /// Returns own date and time, if it was set by <see cref="InjectActualDateTime(DateTimeOffset)"/>.
        /// If it was not set, <see cref="DateTimeOffset.UtcNow">DateTimeOffset.UtcNow</see> is returned.
        /// </summary>
        public static DateTimeOffset DateTimeOffsetUtcNow => _injectedDateTimeOffset ?? DateTimeOffset.UtcNow;

        /// <summary>
        /// Sets datetime <paramref name="value"/>, which will be returned by all properties.
        /// Use it in <c>using</c> block.
        /// </summary>
        /// <param name="value">Required date and time value.</param>
        public static IDisposable InjectActualDateTime(DateTime value)
        {
            _injectedDateTime = value.ToUniversalTime();
            _injectedDateTimeOffset = new DateTimeOffset(_injectedDateTime.Value);
            return new DateTimeProvider();
        }

        /// <summary>
        /// Sets time <paramref name="value"/>, which will be returned by all properties.
        /// Use it in <c>using</c> block.
        /// </summary>
        /// <param name="value">Required date and time value.</param>
        public static IDisposable InjectActualDateTime(DateTimeOffset value)
        {
            _injectedDateTimeOffset = value;
            _injectedDateTime = _injectedDateTimeOffset.Value.UtcDateTime;
            return new DateTimeProvider();
        }

        /// <summary>
        /// Clear injected datetime.
        /// </summary>
        public void Dispose()
        {
            _injectedDateTime = null;
            _injectedDateTimeOffset = null;
        }
    }
}
