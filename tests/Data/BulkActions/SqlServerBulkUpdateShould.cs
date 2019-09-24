using FluentAssertions;
using Kros.Data.BulkActions;
using Kros.Data.BulkActions.SqlServer;
using Kros.UnitTests;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            public string ColShortText { get; set; }

            public BulkUpdateItem Clone() => (BulkUpdateItem)MemberwiseClone();
        }

        private class BulkUpdateItemWithIdentity
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public override bool Equals(object obj)
            {
                if (obj is BulkUpdateItemWithIdentity item)
                {
                    return (Id == item.Id) && (Value == item.Value);
                }
                return base.Equals(obj);
            }
            public override int GetHashCode() => HashCode.Combine(Id, Value);
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
$@"CREATE TABLE[dbo].[{TableName}] (
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
$@"CREATE TABLE [dbo].[{Identity_TableName}](
    [Id] [int] IDENTITY(1, 1) NOT NULL,
    [Value] [nvarchar](50) NULL,
    CONSTRAINT [PK_{Identity_TableName}] PRIMARY KEY CLUSTERED ([id] ASC)
)";

        private static readonly string Identity_InsertData =
$@"INSERT INTO [{Identity_TableName}] ([Value]) VALUES ('one')
INSERT INTO [{Identity_TableName}] ([Value]) VALUES ('two')
INSERT INTO [{Identity_TableName}] ([Value]) VALUES ('three')";

        #endregion

        #region DatabaseTestBase Overrides

        protected override string BaseDatabaseName => DatabaseName;

        protected override IEnumerable<string> DatabaseInitScripts =>
            new string[] { CreateTable_BulkUpdateTest, Insert_BulkUpdateTest };

        #endregion

        #region Tests

        [Fact]
        public void BulkUpdateDataFromIDataReaderSynchronouslyWithoutDeadLock()
        {
            AsyncContext.Run(() =>
            {
                DataTable expectedData = CreateExpectedData();
                DataTable actualData = null;

                using (IDataReader reader = expectedData.CreateDataReader())
                using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
                {
                    bulkUpdate.DestinationTableName = TableName;
                    bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                    bulkUpdate.Update(reader);
                }

                actualData = LoadData(ServerHelper.Connection);

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
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                bulkUpdate.TempTableAction = UpdateTempItems;
                bulkUpdate.Update(reader);
            }

            actualData = LoadData(ServerHelper.Connection);

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
        public async Task BulkUpdateDataInTableWithIdentityPrimaryColumn()
        {
            List<BulkUpdateItemWithIdentity> actualData = null;

            using (var helper = new SqlServerTestHelper(
                BaseConnectionString, DatabaseName, new[] { Identity_CreateTable, Identity_InsertData }))
            using (var bulkUpdate = new SqlServerBulkUpdate(helper.Connection))
            {
                var dataToUpdate = new EnumerableDataReader<BulkUpdateItemWithIdentity>(
                    new[] { new BulkUpdateItemWithIdentity() { Id = 2, Value = "lorem ipsum" } },
                    new[] { nameof(BulkUpdateItemWithIdentity.Id), nameof(BulkUpdateItemWithIdentity.Value) });

                bulkUpdate.DestinationTableName = Identity_TableName;
                bulkUpdate.PrimaryKeyColumn = nameof(BulkUpdateItemWithIdentity.Id);
                await bulkUpdate.UpdateAsync(dataToUpdate);

                helper.Connection.Open();
                actualData = LoadDataForTableWithIdentity(helper.Connection);
            }

            actualData.Should().Equal(new List<BulkUpdateItemWithIdentity>(new[]
            {
                new BulkUpdateItemWithIdentity() { Id = 1, Value = "one" },
                new BulkUpdateItemWithIdentity() { Id = 2, Value = "lorem ipsum" },
                new BulkUpdateItemWithIdentity() { Id = 3, Value = "three" }
            }));
        }

        #endregion

        #region Helpers

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

        private void UpdateTempItems(IDbConnection connection, IDbTransaction transaction, string tempTableName)
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

        private List<BulkUpdateItemWithIdentity> LoadDataForTableWithIdentity(SqlConnection cn)
        {
            var data = new List<BulkUpdateItemWithIdentity>();

            using (var cmd = new SqlCommand($"SELECT [id], [value] FROM [{Identity_TableName}]", cn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new BulkUpdateItemWithIdentity() { Id = reader.GetInt32(0), Value = reader.GetString(1) });
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

            table.PrimaryKey = new DataColumn[] { table.Columns[PrimaryKeyColumn] };

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
