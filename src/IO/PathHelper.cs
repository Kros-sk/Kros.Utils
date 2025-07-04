using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kros.IO
{
    /// <summary>
    /// Helpers for working with file/folder paths.
    /// </summary>
    public static class PathHelper
    {
        #region Helpers

        private const char DirectorySeparatorChar1 = '/';
        private const char DirectorySeparatorChar2 = '\\';
        private static readonly char[] _directorySeparatorChars = [DirectorySeparatorChar1, DirectorySeparatorChar2];

        private static readonly char[] _invalidPathChars = [
            '*', '?', '\"', '\'', '`', '\0', '|', '<', '>',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31
        ];

        /// <summary>
        /// List of characters, which are not allowed in file or folder names.
        /// </summary>
        public static IReadOnlyList<char> InvalidPathChars { get; } = [.. _invalidPathChars];

        private static readonly Regex _reReplacePathChars = new(CreatePathReplacePattern(), RegexOptions.Compiled);

        private static string CreatePathReplacePattern()
        {
            System.Text.StringBuilder result = new(InvalidPathChars.Count + 5);

            result.Append('[');
            result.Append(Regex.Escape("\\/"));
            foreach (char c in InvalidPathChars)
            {
                result.Append(Regex.Escape(Convert.ToString(c)));
            }
            result.Append("]+");

            return result.ToString();
        }

        #endregion

        /// <summary>
        /// Joins parts <paramref name="parts"/> to one string, representing path to a file/folder.
        /// Path separator is normalized to <c>/</c> because this is allowed in all operating systems.
        /// </summary>
        /// <param name="parts">Path parts.</param>
        /// <returns>Created path.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of <paramref name="parts"/> or any of its item is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Any of the item in <paramref name="parts"/> contains invalid characters
        /// defined in <see cref="InvalidPathChars"/>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// The method works similarly as standard .NET method <see cref="Path.Combine(string[])" autoUpgrade="true"/>,
        /// with some different details:
        /// <list type="bullet">
        /// <item>If any part begins with a slash (forward or backslash), this slash is ignored.
        /// The <see cref="Path.Combine(string[])" autoUpgrade="true"/> ignores all parts before the last part begining
        /// with a slash (if such exists).
        /// <example>
        /// <c>Path.Combine("lorem", "\ipsum", "dolor")</c> returns <c>\ipsum\dolor</c><br />
        /// <c>PathHelper.BuildPath("lorem", "\ipsum", "dolor")</c> returns <c>lorem\ipsum\dolor</c>
        /// </example>
        /// </item>
        /// <item>If any part ends with a disk separator (<c>:</c>), directory separator is appended even after it.
        /// <example>
        /// <c>Path.Combine("c:", "lorem", "ipsum", "dolor")</c> returns <b>c:lorem\ipsum\dolor</b><br />
        /// <c>PathHelper.Build("c:", "lorem", "ipsum", "dolor")</c> returns <b>c:\lorem\ipsum\dolor</b>
        /// </example>
        /// Some of the .NET function are not able to work with a paths like <c>c:lorem</c> and the throw exceptions.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static string BuildPath(params string[] parts)
        {
            Check.NotNull(parts, nameof(parts));

            int capacity = CheckBuildPathParts(parts);
            System.Text.StringBuilder sb = new(capacity);
            foreach (string part in parts)
            {
                if (part.Length > 0)
                {
                    if (sb.Length > 0)
                    {
                        char firstChar = part[0];
                        char lastChar = sb[sb.Length - 1];

                        if (((firstChar == DirectorySeparatorChar1) || (firstChar == DirectorySeparatorChar2)) &&
                            ((lastChar == DirectorySeparatorChar1) || (lastChar == DirectorySeparatorChar2)))
                        {
                            sb.Length -= 1;
                        }
                        else if ((firstChar != DirectorySeparatorChar1) && (firstChar != DirectorySeparatorChar2) &&
                            (lastChar != DirectorySeparatorChar1) && (lastChar != DirectorySeparatorChar2))
                        {
                            sb.Append('/');
                        }
                    }
                    sb.Append(part);
                }
            }
            sb.Replace('\\', '/'); // Normalize path separators.

            return sb.ToString();
        }

        private static int CheckBuildPathParts(string[] parts)
        {
            int capacity = parts.Length;
            int partIndex = 0;

            foreach (string part in parts)
            {
                Check.NotNull(part, nameof(parts));

                if (part.IndexOfAny(_invalidPathChars) >= 0)
                {
                    throw new ArgumentException(string.Format(Resources.PathContainsInvalidCharacters, partIndex, part));
                }

                capacity += part.Length;
                partIndex++;
            }
            return capacity;
        }

        /// <summary>
        /// Replaces invalid characters in <paramref name="pathName"/> with dash (<c>-</c>). If there are
        /// several succesive invalid characters, they all are replaced only by one dash.
        /// </summary>
        /// <param name="pathName">Input path.</param>
        /// <remarks><inheritdoc cref="ReplaceInvalidPathChars(string, string)"/></remarks>
        /// <returns><inheritdoc cref="ReplaceInvalidPathChars(string, string)"/></returns>
        public static string ReplaceInvalidPathChars(string? pathName) => ReplaceInvalidPathChars(pathName, "-");

        /// <summary>
        /// Replaces invalid characters in <paramref name="pathName"/> with <paramref name="replacement"/>. If there are
        /// several succesive invalid characters, they all are replaced only by one <paramref name="replacement"/>.
        /// </summary>
        /// <param name="pathName">Input path.</param>
        /// <param name="replacement">Value, by which are replaced invalid path charactes. If the value is <see langword="null"/>,
        /// empty stirng is used, so invalid characters are removed.</param>
        /// <remarks>
        /// Replaced are all characters in <see cref="InvalidPathChars"/> and <see cref="Path.GetInvalidPathChars"/>.
        /// </remarks>
        /// <returns>
        /// String with invalid path characters replaced. If input <paramref name="pathName"/> is <see langword="null"/>,
        /// empty string is returned.
        /// </returns>
        public static string ReplaceInvalidPathChars(string? pathName, string? replacement)
        {
            if (pathName is null)
            {
                return string.Empty;
            }
            if (replacement is null)
            {
                replacement = string.Empty;
            }
            return _reReplacePathChars.Replace(pathName, replacement);
        }

        /// <summary>
        /// Returns path to system temporary folder (<see cref="Path.GetTempPath"/>) <b>without</b> trailing directory separator.
        /// </summary>
        public static string GetTempPath()
            => Path.GetTempPath().TrimEnd(_directorySeparatorChars);

        /// <summary>
        /// Checks, if specified <paramref name="path"/> is network share path. The path is considered network share path,
        /// if it begins with <c>\\</c>.
        /// </summary>
        /// <param name="path">Checked path.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="path"/> is network share path, <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsNetworkPath(string path) => (GetDriveTypeFromPath(path) == DriveType.Network);

        private static DriveType GetDriveTypeFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return DriveType.Unknown;
            }
            if ((path.Length >= 2) && (path[0] == '\\') && (path[1] == '\\'))
            {
                return DriveType.Network;
            }

            string driveName = path.Length > 3 ? path.Substring(0, 3) : path;
            DriveInfo? drive = DriveInfo.GetDrives()
                .Where(item => item.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (drive is not null)
            {
                return drive.DriveType;
            }

            return DriveType.Unknown;
        }
    }
}
