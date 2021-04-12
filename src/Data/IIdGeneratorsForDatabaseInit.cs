using System;
using System.Collections.Generic;

namespace Kros.Data
{
    /// <summary>
    /// Collection of ID generators for database initialization.
    /// </summary>
    public interface IIdGeneratorsForDatabaseInit : IEnumerable<IIdGenerator>, IDisposable
    {
    }
}
