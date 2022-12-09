using System;
using System.Data;
using System.Data.Common;

namespace Kros.Utils.UnitTests.Data
{
    public class CustomConnection : DbConnection
    {
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

        public override string Database => throw new NotImplementedException();

        public override string DataSource => throw new NotImplementedException();

        public override string ServerVersion => throw new NotImplementedException();

        public override ConnectionState State => throw new NotImplementedException();

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }
    }
}
