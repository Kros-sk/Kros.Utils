using FluentAssertions;
using Kros.Data.BulkActions;
using Kros.Data.BulkActions.SqlServer;
using Kros.UnitTests;
using Microsoft.Data.SqlClient;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class SqlServerBulkUpdateShould : DatabaseTestBase
    {
        #region Nested types

        private class BulkUpdateItem
        {
            public int Id { get; set; }
            public int? ColInt32 { get; set; }
            public double? ColDouble { get; set; }
            public DateTime? ColDate { get; set; }
            public Guid? ColGuid { get; set; }
            public bool? ColBool { get; set; }
            public string ColShortText { get; set; } = string.Empty;

            public BulkUpdateItem Clone() => (BulkUpdateItem)MemberwiseClone();
        }

        private class BulkUpdateItemIdentity
        {
            public int Id { get; set; }
            public string Value { get; set; } = string.Empty;
            public override bool Equals(object? obj)
            {
                if (obj is BulkUpdateItemIdentity item)
                {
                    return (Id == item.Id) && (Value == item.Value);
                }
                return base.Equals(obj);
            }
            public override int GetHashCode() => HashCode.Combine(Id, Value);
        }

        private class BulkUpdateItemComposite
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public string Value { get; set; } = string.Empty;
            public override bool Equals(object? obj)
            {
                if (obj is BulkUpdateItemComposite item)
                {
                    return (Id1 == item.Id1) && (Id2 == item.Id2) && (Value == item.Value);
                }
                return base.Equals(obj);
            }
            public override int GetHashCode() => HashCode.Combine(Id1, Id2, Value);
        }

        #endregion

        #region Constants

        private const double DoubleMinimum = -999999999999999999999.999999999999;
        private const double DoubleMaximum = 999999999999999999999.999999999999;
        private const string DatabaseName = "KrosUtilsTestBulkUpdate";
        private const string TableName = "BulkUpdateTest";
        private const string PrimaryKeyColumn = "Id";
        private const string ShortTextAction = "dolor sit amet";

        private readonly string CreateTable_BulkUpdateTest =
$@"CREATE TABLE [{TableName}] (
    [Id] [int] NOT NULL,
    [ColInt32] [int] NULL,
    [ColDouble] [float] NULL,
    [ColDate] [datetime2] (7) NULL,
    [ColGuid] [uniqueidentifier] NULL,
    [ColBool] [bit] NULL,
    [ColShortText] [nvarchar] (20) NULL

    CONSTRAINT [PK_{TableName}] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
) ON [PRIMARY];";

        private readonly string Insert_BulkUpdateTest =
$@"INSERT INTO {TableName} VALUES (1, 0, 0.0, '1900-01-01 00:00:00', '00000000-0000-0000-0000-000000000000', 0, '')
INSERT INTO {TableName} (Id) VALUES (2)
INSERT INTO {TableName} VALUES (3, 1, 9.9, '2018-01-30 10:26:36', 'abcdefab-1234-5678-9123-abcdefabcdef', 1, 'ipsum')";

        private const string Identity_TableName = "BulkUpdate_Identity";

        private static readonly string Identity_CreateTable =
$@"CREATE TABLE [{Identity_TableName}](
    [Id] [int] IDENTITY(1, 1) NOT NULL,
    [Value] [nvarchar](50) NULL,
    CONSTRAINT [PK_{Identity_TableName}] PRIMARY KEY CLUSTERED ([id] ASC)
)";

        private static readonly string Identity_InsertData =
$@"INSERT INTO [{Identity_TableName}] ([Value]) VALUES ('one')
INSERT INTO [{Identity_TableName}] ([Value]) VALUES ('two')
INSERT INTO [{Identity_TableName}] ([Value]) VALUES ('three')";

        private const string Composite_TableName = "BulkUpdate_Composite";

        private static readonly string Composite_CreateTable =
$@"CREATE TABLE [{Composite_TableName}](
    [Id1] [int] NOT NULL,
    [Id2] [int] NOT NULL,
    [Value] [nvarchar](50) NULL,
    CONSTRAINT [PK_{Composite_TableName}] PRIMARY KEY CLUSTERED ([Id1] ASC, [Id2] ASC)
)";

        private static readonly string Composite_InsertData =
$@"INSERT INTO [{Composite_TableName}] ([Id1], [Id2], [Value]) VALUES (1, 1, '1 - 1')
INSERT INTO [{Composite_TableName}] ([Id1], [Id2], [Value]) VALUES (1, 2, '1 - 2')
INSERT INTO [{Composite_TableName}] ([Id1], [Id2], [Value]) VALUES (2, 1, '2 - 1')
INSERT INTO [{Composite_TableName}] ([Id1], [Id2], [Value]) VALUES (2, 2, '2 - 2')
INSERT INTO [{Composite_TableName}] ([Id1], [Id2], [Value]) VALUES (3, 1, '3 - 1')
INSERT INTO [{Composite_TableName}] ([Id1], [Id2], [Value]) VALUES (3, 2, '3 - 2')";

        private const string CompositeWithIdentity_TableName = "BulkUpdate_CompositeWithIdentity";

        private static readonly string CompositeWithIdentity_CreateTable =
$@"CREATE TABLE [{CompositeWithIdentity_TableName}] (
    [Id1] [int] NOT NULL,
    [Id2] [int] IDENTITY(1,1) NOT NULL,
    [Value] [nvarchar](50) NULL,
    CONSTRAINT [PK_{CompositeWithIdentity_TableName}] PRIMARY KEY CLUSTERED ([Id1] ASC, [Id2] ASC)
)";

        private static readonly string CompositeWithIdentity_InsertData =
$@"INSERT INTO [{CompositeWithIdentity_TableName}] ([Id1], [Value]) VALUES (1, '1 - 1')
INSERT INTO [{CompositeWithIdentity_TableName}] ([Id1], [Value]) VALUES (1, '1 - 2')
INSERT INTO [{CompositeWithIdentity_TableName}] ([Id1], [Value]) VALUES (2, '2 - 3')
INSERT INTO [{CompositeWithIdentity_TableName}] ([Id1], [Value]) VALUES (2, '2 - 4')
INSERT INTO [{CompositeWithIdentity_TableName}] ([Id1], [Value]) VALUES (3, '3 - 5')
INSERT INTO [{CompositeWithIdentity_TableName}] ([Id1], [Value]) VALUES (3, '3 - 6')";

        #endregion

        #region DatabaseTestBase Overrides

        protected override string BaseDatabaseName => DatabaseName;

        protected override IEnumerable<string> DatabaseInitScripts =>
            new string[] { CreateTable_BulkUpdateTest, Insert_BulkUpdateTest };

        #endregion

        #region Tests

        [Fact]
        public void ThrowExceptionWhenInputDataIsNull()
        {
            Action action = () =>
            {
                using (var helper = CreateHelper(null))
                using (var bulkUpdate = new SqlServerBulkUpdate(helper.Connection))
                {
                    bulkUpdate.DestinationTableName = TableName;
                    bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                    bulkUpdate.Update((IDataReader)null!);
                }
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowExceptionWhenPrimaryKeyIsNotSet()
        {
            Action action = () =>
            {
                using (var helper = CreateHelper(null))
                using (var bulkUpdate = new SqlServerBulkUpdate(helper.Connection))
                {
                    bulkUpdate.DestinationTableName = TableName;
                    bulkUpdate.Update(new DataTable());
                }
            };

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BulkUpdateDataFromIDataReaderSynchronouslyWithoutDeadLock()
        {
            AsyncContext.Run(() =>
            {
                DataTable expectedData = CreateExpectedData();

                using (IDataReader reader = expectedData.CreateDataReader())
                using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
                {
                    bulkUpdate.DestinationTableName = TableName;
                    bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                    bulkUpdate.Update(reader);
                }

                DataTable actualData = LoadData(ServerHelper.Connection);

                SqlServerBulkHelper.CompareTables(actualData, expectedData);
            });
        }

        [Fact]
        public async Task BulkUpdateDataFromIDataReaderAsynchronously()
        {
            DataTable expectedData = CreateExpectedData();

            using (IDataReader reader = expectedData.CreateDataReader())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                await bulkUpdate.UpdateAsync(reader);
            }

            DataTable actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkUpdateDataFromIBulkInsertDataReader()
        {
            DataTable expectedData = CreateExpectedData();

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                bulkUpdate.Update(reader);
            }

            DataTable actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public async Task BulkUpdateDataFromIBulkInsertDataReaderAsynchronously()
        {
            DataTable expectedData = CreateExpectedData();

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                await bulkUpdate.UpdateAsync(reader);
            }

            DataTable actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkUpdateDataFromIDataReaderWithAction()
        {
            DataTable expectedData = CreateExpectedDataWithAction();

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                bulkUpdate.TempTableAction = UpdateTempItems;
                bulkUpdate.Update(reader);
            }

            DataTable actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkUpdateDataFromDataTable()
        {
            DataTable expectedData = CreateExpectedData();

            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                bulkUpdate.Update(expectedData);
            }

            DataTable actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public async Task BulkUpdateDataFromDataTableAsynchronously()
        {
            DataTable expectedData = CreateExpectedData();

            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                await bulkUpdate.UpdateAsync(expectedData);
            }

            DataTable actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public async Task BulkUpdateDataInTableWithIdentityPrimaryKey()
        {
            List<BulkUpdateItemIdentity> actualData;

            using (var helper = CreateHelper(new[] { Identity_CreateTable, Identity_InsertData }))
            using (var bulkUpdate = new SqlServerBulkUpdate(helper.Connection))
            {
                var dataToUpdate = new EnumerableDataReader<BulkUpdateItemIdentity>(
                    new[] { new BulkUpdateItemIdentity() { Id = 2, Value = "lorem ipsum" } },
                    new[] { nameof(BulkUpdateItemIdentity.Id), nameof(BulkUpdateItemIdentity.Value) });

                bulkUpdate.DestinationTableName = Identity_TableName;
                bulkUpdate.PrimaryKeyColumn = nameof(BulkUpdateItemIdentity.Id);
                await bulkUpdate.UpdateAsync(dataToUpdate);

                helper.Connection.Open();
                actualData = LoadDataForTableWithIdentity(helper.Connection);
            }

            actualData.Should().Equal(new List<BulkUpdateItemIdentity>(new[]
            {
                new BulkUpdateItemIdentity() { Id = 1, Value = "one" },
                new BulkUpdateItemIdentity() { Id = 2, Value = "lorem ipsum" },
                new BulkUpdateItemIdentity() { Id = 3, Value = "three" }
            }));
        }

        [Fact]
        public async Task BulkUpdateDataInTableWithCompositePrimaryKey()
        {
            List<BulkUpdateItemComposite> actualData;

            using (var helper = CreateHelper(new[] { Composite_CreateTable, Composite_InsertData }))
            using (var bulkUpdate = new SqlServerBulkUpdate(helper.Connection))
            {
                var dataToUpdate = new EnumerableDataReader<BulkUpdateItemComposite>(
                    new[] {
                        new BulkUpdateItemComposite() { Id1 = 1, Id2 = 2, Value = "lorem ipsum 1" },
                        new BulkUpdateItemComposite() { Id1 = 2, Id2 = 2, Value = "lorem ipsum 2" },
                        new BulkUpdateItemComposite() { Id1 = 3, Id2 = 2, Value = "lorem ipsum 3" }
                    },
                    new[] { nameof(BulkUpdateItemComposite.Id1), nameof(BulkUpdateItemComposite.Id2), nameof(BulkUpdateItemIdentity.Value) });

                bulkUpdate.DestinationTableName = Composite_TableName;
                bulkUpdate.PrimaryKeyColumn = nameof(BulkUpdateItemComposite.Id1) + ", " + nameof(BulkUpdateItemComposite.Id2);
                await bulkUpdate.UpdateAsync(dataToUpdate);

                helper.Connection.Open();
                actualData = LoadDataForTableWithCompositePk(helper.Connection, Composite_TableName);
            }

            actualData.Should().Equal(new List<BulkUpdateItemComposite>(new[]
            {
                new BulkUpdateItemComposite() { Id1 = 1, Id2 = 1, Value = "1 - 1" },
                new BulkUpdateItemComposite() { Id1 = 1, Id2 = 2, Value = "lorem ipsum 1" },
                new BulkUpdateItemComposite() { Id1 = 2, Id2 = 1, Value = "2 - 1" },
                new BulkUpdateItemComposite() { Id1 = 2, Id2 = 2, Value = "lorem ipsum 2" },
                new BulkUpdateItemComposite() { Id1 = 3, Id2 = 1, Value = "3 - 1" },
                new BulkUpdateItemComposite() { Id1 = 3, Id2 = 2, Value = "lorem ipsum 3" },
            }));
        }

        [Fact]
        public async Task BulkUpdateDataInTableWithCompositeAndIdentityPrimaryKey()
        {
            List<BulkUpdateItemComposite> actualData;

            using (var helper = CreateHelper(new[] { CompositeWithIdentity_CreateTable, CompositeWithIdentity_InsertData }))
            using (var bulkUpdate = new SqlServerBulkUpdate(helper.Connection))
            {
                var dataToUpdate = new EnumerableDataReader<BulkUpdateItemComposite>(
                    new[] {
                        new BulkUpdateItemComposite() { Id1 = 1, Id2 = 2, Value = "lorem ipsum 2" },
                        new BulkUpdateItemComposite() { Id1 = 2, Id2 = 3, Value = "lorem ipsum 3" },
                        new BulkUpdateItemComposite() { Id1 = 3, Id2 = 6, Value = "lorem ipsum 6" }
                    },
                    new[] { nameof(BulkUpdateItemComposite.Id1), nameof(BulkUpdateItemComposite.Id2), nameof(BulkUpdateItemIdentity.Value) });

                bulkUpdate.DestinationTableName = CompositeWithIdentity_TableName;
                bulkUpdate.PrimaryKeyColumn = nameof(BulkUpdateItemComposite.Id1) + ", " + nameof(BulkUpdateItemComposite.Id2);
                await bulkUpdate.UpdateAsync(dataToUpdate);

                helper.Connection.Open();
                actualData = LoadDataForTableWithCompositePk(helper.Connection, CompositeWithIdentity_TableName);
            }

            actualData.Should().Equal(new List<BulkUpdateItemComposite>(new[]
            {
                new BulkUpdateItemComposite() { Id1 = 1, Id2 = 1, Value = "1 - 1" },
                new BulkUpdateItemComposite() { Id1 = 1, Id2 = 2, Value = "lorem ipsum 2" },
                new BulkUpdateItemComposite() { Id1 = 2, Id2 = 3, Value = "lorem ipsum 3" },
                new BulkUpdateItemComposite() { Id1 = 2, Id2 = 4, Value = "2 - 4" },
                new BulkUpdateItemComposite() { Id1 = 3, Id2 = 5, Value = "3 - 5" },
                new BulkUpdateItemComposite() { Id1 = 3, Id2 = 6, Value = "lorem ipsum 6" },
            }));
        }

        #endregion

        #region Helpers

        private SqlServerTestHelper CreateHelper(IEnumerable<string>? initDatabaseScripts)
            => new SqlServerTestHelper(BaseConnectionString, DatabaseName, initDatabaseScripts);

        private static readonly List<BulkUpdateItem> _rawData = new List<BulkUpdateItem>
        {
            new BulkUpdateItem()
            {
                Id = 1,
                ColInt32 = 123,
                ColDouble = 123456.654321,
                ColDate =new DateTime(1978, 12, 10, 7, 30, 59),
                ColGuid =new Guid("abcdef00-1234-5678-9000-abcdefabcdef"),
                ColBool = true,
                ColShortText = "lorem ipsum",
            },
            new BulkUpdateItem()
            {
                Id = 2,
                ColInt32 = int.MinValue,
                ColDouble = DoubleMinimum,
                ColDate =new DateTime(1900, 1, 1),
                ColGuid =new Guid("abcdef00-1234-5678-9000-abcdefabcdef"),
                ColBool = false,
                ColShortText = "dolor sit amet",
            },
            new BulkUpdateItem()
            {
                Id = 3,
                ColInt32 = int.MaxValue,
                ColDouble = DoubleMaximum,
                ColDate = DateTime.MaxValue,
                ColGuid = Guid.Empty,
                ColBool = true,
                ColShortText = string.Empty,
            }
        };

        private void UpdateTempItems(IDbConnection connection, IDbTransaction? transaction, string tempTableName)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = $"UPDATE [{tempTableName}] SET ColGuid = '{Guid.Empty}', ColShortText = '{ShortTextAction}'";
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable LoadData(SqlConnection cn)
        {
            DataTable data = new DataTable(TableName);

            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {TableName}", cn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.FillSchema(data, SchemaType.Source);
                adapter.Fill(data);
            }

            return data;
        }

        private List<BulkUpdateItemIdentity> LoadDataForTableWithIdentity(SqlConnection cn)
        {
            var data = new List<BulkUpdateItemIdentity>();

            using (var cmd = new SqlCommand($"SELECT [Id], [Value] FROM [{Identity_TableName}] ORDER BY [Id]", cn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new BulkUpdateItemIdentity() { Id = reader.GetInt32(0), Value = reader.GetString(1) });
                }
            }
            return data;
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private List<BulkUpdateItemComposite> LoadDataForTableWithCompositePk(SqlConnection cn, string tableName)
        {
            var data = new List<BulkUpdateItemComposite>();

            using (var cmd = new SqlCommand($"SELECT [Id1], [Id2], [Value] FROM [{tableName}] ORDER BY [Id1], [Id2]", cn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new BulkUpdateItemComposite() { Id1 = reader.GetInt32(0), Id2 = reader.GetInt32(1), Value = reader.GetString(2) });
                }
            }
            return data;
        }

        private DataTable CreateExpectedData()
        {
            DataTable table = CreateBulkUpdateDataTable();
            FillBulkUpdateDataTable(table, _rawData);

            return table;
        }

        private DataTable CreateExpectedDataWithAction()
        {
            var table = CreateBulkUpdateDataTable();

            FillBulkUpdateDataTable(table, GetActionRawData());

            return table;
        }

        private List<BulkUpdateItem> GetActionRawData()
        {
            var actionRawData = new List<BulkUpdateItem>();

            foreach (var item in _rawData)
            {
                BulkUpdateItem cloneItem = item.Clone();
                cloneItem.ColGuid = Guid.Empty;
                cloneItem.ColShortText = ShortTextAction;
                actionRawData.Add(cloneItem);
            }

            return actionRawData;
        }

        private DataTable CreateBulkUpdateDataTable()
        {
            DataTable table = new DataTable("data");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("ColInt32", typeof(int));
            table.Columns.Add("ColDouble", typeof(double));
            table.Columns.Add("ColDate", typeof(DateTime));
            table.Columns.Add("ColGuid", typeof(Guid));
            table.Columns.Add("ColBool", typeof(bool));
            table.Columns.Add("ColShortText", typeof(string));

            table.PrimaryKey = new DataColumn[] { table.Columns[PrimaryKeyColumn]! };

            return table;
        }

        private void FillBulkUpdateDataTable(DataTable table, List<BulkUpdateItem> rawData)
        {
            foreach (var rawItem in rawData)
            {
                AddBulkUpdateDataRow(table, rawItem);
            }
        }

        private void AddBulkUpdateDataRow(DataTable table, BulkUpdateItem item)
        {
            DataRow row = table.NewRow();

            foreach (var itemProperty in typeof(BulkUpdateItem).GetProperties())
            {
                row[itemProperty.Name] = itemProperty.GetValue(item);
            }

            table.Rows.Add(row);
        }

        private IBulkActionDataReader CreateDataReaderForUpdate()
        {
            var columnNames = typeof(BulkUpdateItem).GetProperties().Select(p => p.Name);

            return new EnumerableDataReader<BulkUpdateItem>(_rawData, columnNames);
        }

        #endregion
    }
}
