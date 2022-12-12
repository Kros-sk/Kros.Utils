using Kros.Utils;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Database schema.
    /// </summary>
    public class DatabaseSchema
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of database schema with specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the database.</param>
        public DatabaseSchema(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            Tables = new TableSchemaCollection(this);
        }

        #endregion

        #region Common

        /// <summary>
        /// Name of the database.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of tables in database.
        /// </summary>
        public TableSchemaCollection Tables { get; }

        /// <inheritdoc/>
        public override string ToString() => $"Database {Name}";

        #endregion
    }
}
