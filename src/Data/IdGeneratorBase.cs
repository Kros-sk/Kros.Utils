using System;
using System.Data.Common;

namespace Kros.Data
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [Obsolete("Class is deprecated. Use DbIntIdGeneratorBase class instead.")]
    public abstract class IdGeneratorBase : DbIntIdGeneratorBase
    {
        protected IdGeneratorBase(DbConnection connection, string tableName, int batchSize)
            : base(connection, tableName, batchSize)
        {
        }

        protected IdGeneratorBase(string connectionString, string tableName, int batchSize)
            : base(connectionString, tableName, batchSize)
        {
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
