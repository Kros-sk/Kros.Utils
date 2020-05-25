using Kros.Utils;
using System;
using System.Linq;

namespace Kros.Extensions
{
    /// <summary>
    /// Extensions for better work with URI.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Gets the domain name from URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Domain name.</returns>
        /// <remarks>
        /// From gmail.google.com return google.com.
        /// </remarks>
        public static string GetDomain(this string uri)
            => new UriBuilder(Check.NotNullOrWhiteSpace(uri, nameof(uri))).Uri.GetDomain();

        /// <summary>
        /// Gets the domain name from URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Domain name.</returns>
        /// <remarks>
        /// From gmail.google.com return google.com.
        /// </remarks>
        public static string GetDomain(this Uri uri)
        {
            Check.NotNull(uri, nameof(uri));

            string[] parts = uri.Host.Split('.');
            if (parts.Length <= 2)
            {
                return uri.Host;
            }

            return string.Join(".", parts.Skip(parts.Length - 2));
        }
    }
}
