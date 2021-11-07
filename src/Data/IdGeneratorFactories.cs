using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Kros.Data
{
    /// <summary>
    /// Helper class for ID generator factories (<see cref="IIdGeneratorFactory"/>) for different databases.
    /// Factories are registered in the class using
    /// <see cref="IdGeneratorFactories.Register{TConnection}(Type, string, Func{IDbConnection, IIdGeneratorFactory}, Func{string, IIdGeneratorFactory})"/>
    /// method. Two factory methods are registered for every connection (database) type. One for creating generator
    /// with connection instance and one with connection string.
    /// </summary>
    /// <remarks>
    /// <see cref="SqlServer.SqlServerIntIdGeneratorFactory"/> and <see cref="SqlServer.SqlServerLongIdGeneratorFactory"/>
    /// are automatically registered.
    /// </remarks>
    /// <seealso cref="IIdGeneratorFactory"/>
    /// <seealso cref="IIdGenerator"/>
    public static class IdGeneratorFactories
    {
        #region Nested types

        private class FactoryInfo
        {
            public FactoryInfo(Type dataType, Type connectionType, Func<IDbConnection, IIdGeneratorFactory> factory)
            {
                DataType = dataType;
                ConnectionType = connectionType;
                FactoryByConnection = factory;
            }

            public FactoryInfo(Type dataType, string clientName, Func<string, IIdGeneratorFactory> factory)
            {
                DataType = dataType;
                ClientName = clientName;
                FactoryByClientName = factory;
            }

            public Type DataType { get; }
            public Type ConnectionType { get; }
            public Func<IDbConnection, IIdGeneratorFactory> FactoryByConnection { get; }
            public string ClientName { get; }
            public Func<string, IIdGeneratorFactory> FactoryByClientName { get; }
        }

        private sealed class IdGeneratorCollection : IIdGeneratorsForDatabaseInit
        {
            private readonly List<IIdGenerator> _factories;

            internal IdGeneratorCollection(IEnumerable<IIdGeneratorFactory> factories)
            {
                _factories = factories.Select(item => item.GetGenerator("_NonExistingTableName", 1)).ToList();
            }

            public IEnumerator<IIdGenerator> GetEnumerator() => _factories.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _factories.GetEnumerator();

            public void Dispose()
            {
                foreach (IIdGenerator factory in _factories)
                {
                    factory.Dispose();
                }
                _factories.Clear();
            }
        }

        #endregion

        static IdGeneratorFactories()
        {
            GuidIdGeneratorFactory.Register();
            SqlServer.SqlServerIntIdGeneratorFactory.Register();
            SqlServer.SqlServerLongIdGeneratorFactory.Register();
        }

        private static readonly Dictionary<string, List<FactoryInfo>> _byConnection
            = new Dictionary<string, List<FactoryInfo>>();
        private static readonly Dictionary<string, List<FactoryInfo>> _byClientName
            = new Dictionary<string, List<FactoryInfo>>();

        /// <summary>
        /// Registers ID generator factory methods <paramref name="factoryByConnection"/> and
        /// <paramref name="factoryByConnectionString"/> for database specified by connection type
        /// <typeparamref name="TConnection"/> and client name <paramref name="clientName"/>.
        /// </summary>
        /// <typeparam name="TConnection">Database connection type.</typeparam>
        /// <param name="dataType">Data type of generator.</param>
        /// <param name="clientName">
        /// Name of the database client. It identifies specific database. For example client name for
        /// <see cref="SqlServer.SqlServerIntIdGeneratorFactory"/> is "Microsoft.Data.SqlClient"
        /// (<see cref="SqlServer.SqlServerDataHelper.ClientId"/>).
        /// </param>
        /// <param name="factoryByConnection">
        /// Factory method for creating <see cref="IIdGeneratorFactory"/> with connection instance.
        /// </param>
        /// <param name="factoryByConnectionString">
        /// Factory method for creating <see cref="IIdGeneratorFactory"/> with connection string.
        /// </param>
        /// <exception cref="ArgumentNullException">Value of any argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="clientName"/> is empty string, or string
        /// containing only whitespace characters.</exception>
        public static void Register<TConnection>(
            Type dataType,
            string clientName,
            Func<IDbConnection, IIdGeneratorFactory> factoryByConnection,
            Func<string, IIdGeneratorFactory> factoryByConnectionString)
            where TConnection : IDbConnection
        {
            Check.NotNull(dataType, nameof(dataType));
            Check.NotNullOrWhiteSpace(clientName, nameof(clientName));
            Check.NotNull(factoryByConnection, nameof(factoryByConnection));
            Check.NotNull(factoryByConnectionString, nameof(factoryByConnectionString));

            if (!_byConnection.TryGetValue(typeof(TConnection).FullName, out List<FactoryInfo> factories))
            {
                factories = new List<FactoryInfo>();
                _byConnection.Add(typeof(TConnection).FullName, factories);
            }
            AddFactory(new FactoryInfo(dataType, typeof(TConnection), factoryByConnection), factories);

            if (!_byClientName.TryGetValue(clientName, out factories))
            {
                factories = new List<FactoryInfo>();
                _byClientName.Add(clientName, factories);
            }
            AddFactory(new FactoryInfo(dataType, clientName, factoryByConnectionString), factories);
        }

        private static void AddFactory(FactoryInfo info, List<FactoryInfo> factories)
        {
            FactoryInfo current = factories.FirstOrDefault(item => item.DataType == info.DataType);
            if (current != null)
            {
                factories.Remove(current);
            }
            factories.Add(info);
        }

        /// <summary>
        /// Returns <see cref="IIdGeneratorFactory"/> for specified <paramref name="connection"/>.
        /// </summary>
        /// <param name="dataType">ID generator data type.</param>
        /// <param name="connection">Database connection.</param>
        /// <returns>Instance of <see cref="IIdGeneratorFactory"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Factory for type of <paramref name="connection"/> is not registered.
        /// </exception>
        public static IIdGeneratorFactory GetFactory(Type dataType, IDbConnection connection)
        {
            if (_byConnection.TryGetValue(connection.GetType().FullName, out List<FactoryInfo> factories))
            {
                FactoryInfo factory = factories.FirstOrDefault(item => item.DataType == dataType);
                if (factory != null)
                {
                    return factory.FactoryByConnection(connection);
                }
            }
            throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForConnection,
                nameof(IIdGeneratorFactory), connection.GetType().FullName));
        }

        /// <summary>
        /// Returns <see cref="IIdGeneratorFactory"/> for specified <paramref name="connectionString"/> and database
        /// type <paramref name="clientName"/>.
        /// </summary>
        /// <param name="dataType">ID generator data type.</param>
        /// <param name="connectionString">Connection string for database.</param>
        /// <param name="clientName">Name, which specifies database type.</param>
        /// <returns>Instance of <see cref="IIdGeneratorFactory"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Factory for database type specified by <paramref name="clientName"/> is not registered.
        /// </exception>
        public static IIdGeneratorFactory GetFactory(Type dataType, string connectionString, string clientName)
        {
            if (_byClientName.TryGetValue(clientName, out List<FactoryInfo> factories))
            {
                FactoryInfo factory = factories.FirstOrDefault(item => item.DataType == dataType);
                if (factory != null)
                {
                    return factory.FactoryByClientName(connectionString);
                }
            }
            throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForClient,
                nameof(IIdGeneratorFactory), clientName));
        }

        /// <summary>
        /// Get all ID generators for specified connection type.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Collection of ID generators.</returns>
        public static IIdGeneratorsForDatabaseInit GetGeneratorsForDatabaseInit(IDbConnection connection)
        {
            if (_byConnection.TryGetValue(connection.GetType().FullName, out List<FactoryInfo> factories))
            {
                return new IdGeneratorCollection(factories.Select(factory => factory.FactoryByConnection(connection)));
            }
            throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForConnection,
                nameof(IIdGeneratorFactory), connection.GetType().FullName));
        }

        /// <summary>
        /// Get all ID generators for specified connection type.
        /// </summary>
        /// <param name="connectionString">Connection string for database.</param>
        /// <param name="clientName">Name, which specifies database type.</param>
        /// <returns>Collection of ID generators.</returns>
        public static IIdGeneratorsForDatabaseInit GetGeneratorsForDatabaseInit(string connectionString, string clientName)
        {
            if (_byClientName.TryGetValue(clientName, out List<FactoryInfo> factories))
            {
                return new IdGeneratorCollection(factories.Select(factory => factory.FactoryByClientName(connectionString)));
            }
            throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForClient,
                nameof(IIdGeneratorFactory), clientName));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        [Obsolete("Method is deprecated. Use 'Register' method with 'dataType' argument.")]
        public static void Register<TConnection>(
            string adoClientName,
            Func<IDbConnection, IIdGeneratorFactory> factoryByConnection,
            Func<string, IIdGeneratorFactory> factoryByConnectionString)
            where TConnection : IDbConnection
            => Register<TConnection>(typeof(int), adoClientName, factoryByConnection, factoryByConnectionString);

        [Obsolete("Method is deprecated. Use 'GetFactory' method with 'dataType' argument.")]
        public static IIdGeneratorFactory GetFactory(DbConnection connection)
            => GetFactory(typeof(int), connection);

        [Obsolete("Method is deprecated. Use 'GetFactory' method with 'dataType' argument.")]
        public static IIdGeneratorFactory GetFactory(string connectionString, string adoClientName)
            => GetFactory(typeof(int), connectionString, adoClientName);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
