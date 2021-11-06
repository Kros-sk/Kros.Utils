using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kros.Data
{
    /// <summary>
    /// Factory for creating GUID ID generators.
    /// </summary>
    /// <seealso cref="GuidIdGenerator"/>
    /// <seealso cref="IdGeneratorFactories"/>
    /// <example>
    /// <code language="cs" source="..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public sealed class GuidIdGeneratorFactory : IIdGeneratorFactory<Guid>
    {
        internal static GuidIdGeneratorFactory Instance { get; } = new GuidIdGeneratorFactory();

        /// <inheritdoc/>
        public IIdGenerator<Guid> GetGenerator(string tableName) => GuidIdGenerator.Instance;

        /// <inheritdoc/>
        public IIdGenerator<Guid> GetGenerator(string tableName, int batchSize) => GuidIdGenerator.Instance;

        /// <inheritdoc/>
        IIdGenerator IIdGeneratorFactory.GetGenerator(string tableName) => GetGenerator(tableName, 1);

        /// <inheritdoc/>
        IIdGenerator IIdGeneratorFactory.GetGenerator(string tableName, int batchSize) => GetGenerator(tableName, batchSize);

        /// <summary>
        /// Registers factory methods for creating an instance of factory into <see cref="IdGeneratorFactories"/>.
        /// </summary>
        public static void Register()
            => IdGeneratorFactories.Register<SqlConnection>(
                typeof(Guid),
                SqlServerDataHelper.ClientId,
                (conn) => Instance,
                (connString) => Instance);
    }
}
