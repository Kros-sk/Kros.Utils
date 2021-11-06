using System;

namespace Kros.Data
{
    /// <summary>
    /// GUID type ID generator.
    /// </summary>
    public sealed class GuidIdGenerator : IIdGenerator<Guid>
    {
        /// <summary>
        /// Static instance of the generator, ready to use.
        /// </summary>
        public static GuidIdGenerator Instance { get; } = new GuidIdGenerator();

        /// <summary>
        /// Returns new GUID value.
        /// </summary>
        /// <returns>GUID.</returns>
        public Guid GetNext() => Guid.NewGuid();

        /// <summary>
        /// Returns new GUID value (calling strongly typed <see cref="GetNext"/>).
        /// </summary>
        /// <returns>GUID.</returns>
        object IIdGenerator.GetNext() => GetNext();

        /// <summary>
        /// Method does nothing. It is here only to match the interface.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Method does nothing. It is here only to match the interface.
        /// </summary>
        public void InitDatabaseForIdGenerator() { }
    }
}
