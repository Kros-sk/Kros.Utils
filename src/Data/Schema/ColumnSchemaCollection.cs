﻿using Kros.Utils;
using Kros.Properties;
using System;

namespace Kros.Data.Schema
{
    /// <summary>
    /// List of columns for table <see cref="TableSchema"/>.
    /// </summary>
    /// <remarks>To the columns added to this list is automatically set their <see cref="ColumnSchema.Table"/>. The column
    /// can belong only to one table.</remarks>
    public class ColumnSchemaCollection
        : System.Collections.ObjectModel.KeyedCollection<string, ColumnSchema>
    {
        #region Constructors

        /// <summary>
        /// Creates a new column list for <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to which column list belongs.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="table"/> is <see langword="null"/>.</exception>
        public ColumnSchemaCollection(TableSchema table)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Table = Check.NotNull(table, nameof(table));
        }

        #endregion

        #region Common

        /// <summary>
        /// The table to which belongs this <c>ColumnSchemaCollection</c>.
        /// </summary>
        public TableSchema Table { get; }

        #endregion

        #region KeyedCollection

        /// <inheritdoc/>
        protected override string GetKeyForItem(ColumnSchema item) => item.Name;

        /// <inheritdoc/>
        protected override void InsertItem(int index, ColumnSchema item)
        {
            if (item.Table is null)
            {
                item.Table = Table;
            }
            else if (item.Table != Table)
            {
                throw new InvalidOperationException(string.Format(Resources.ColumnBelongsToAnotherTable,
                    item.Name, Table.Name, item.Table.Name));
            }
            base.InsertItem(index, item);
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            if (index < Count)
            {
                base[index].Table = null;
            }
            base.RemoveItem(index);
        }

        #endregion
    }
}
