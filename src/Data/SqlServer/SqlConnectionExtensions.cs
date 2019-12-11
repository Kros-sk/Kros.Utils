﻿using Kros.Utils;
using Microsoft.Data.SqlClient;
using System;

namespace Kros.Data.SqlServer
{
    /// <summary>
    /// Extension methods for <see cref="SqlConnection"/> class.
    /// </summary>
    public static class SqlConnectionExtensions
    {
        /// <summary>
        /// Returns <see cref="SqlConnection.ServerVersion"/> string as <see cref="Version"/> object.
        /// </summary>
        /// <param name="connection">Connection from which SQL Server version is returned.</param>
        /// <returns>SQL Server version as <see cref="Version"/> object.</returns>
        public static Version GetVersion(this SqlConnection connection)
        {
            Check.NotNull(connection, nameof(connection));
            using (ConnectionHelper.OpenConnection(connection))
            {
                return new Version(connection.ServerVersion);
            }
        }
    }
}
