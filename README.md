# Kros.Utils [![Build status](https://ci.appveyor.com/api/projects/status/5ws8h5jp777wsalb/branch/master?svg=true)](https://ci.appveyor.com/project/Kros/kros-libs/branch/master)

__Kros.Utils__ is universal library of various tools to simplify the work of the programmer.
Library is:

* Independent of third-party libraries. You only need .NET Framework.
* Platform-independent. That means it's applicable to desktop applications and server services (e.g. It's independent of System.Windows.Forms).

Library is compiled for __.NET Standard 2.0__. .NET Framework 4.7 is supported.

## Documentation

For configuration, general information and examples [see the documentation](https://kros-sk.github.io/Kros.Libs.Documentation/).

## Download

Kros.Libs is available from __Nuget__ [__Kros.Utils__](https://www.nuget.org/packages/Kros.Utils/).

## Contributing Guide

To contribute with new topics/information or make changes, see [contributing](https://github.com/Kros-sk/Kros.Utils/blob/master/CONTRIBUTING.md) for instructions and guidelines.

## This topic contains following sections

__Kros.Utils__

* [Arguments Check Functions](#arguments-check-functions)
* [Standard Extensions](#standard-extensions)
* [File/Folder Path Helpers](#filefolder-path-helpers)
* [Database Schema](#database-schema)
* [Bulk Operations - Bulk Insert and Bulk Update](#bulk-operations---bulk-insert-and-bulk-update)
* [IdGenerator](#idgenerator)
* [IDiContainer Interface Describing Dependency Injection Container](#idicontainer-interface-describing-dependency-injection-container)
* [Caching](#caching)
* [Unit Testing Helpers](#unit-testing-helpers)

### Arguments Check Functions

The [Check](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Utils.Check.html "Check") class provides simple tools to check arguments of the functions. Standard usage:

```c#
private string _value1;
private int _value2;

public void MethodWithParameters(string arg1, int arg2)
{
    if (string.IsNullOrEmpty(arg1))
    {
        throw new ArgumentNullException(nameof(arg1));
    }
    if (arg2 <= 0)
    {
        throw new ArgumentException("Value of parameter arg2 must be greater than 0.", nameof(arg2));
    }

    _value1 = arg1;
    _value2 = arg2;

    // ...
}
```

With the [Check](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Utils.Check.html "Check") class, it's much more simplier. The individual checks return the input value, so it's possible to check the argument on one line, even to assign:

```c#
private string _value1;
private int _value2;

public void MethodWithParameters(string arg1, int arg2)
{
    _value1 = Check.NotNullOrEmpty(arg1, nameof(arg1));
    _value2 = Check.GreaterThan(arg2, 0, nameof(arg2));

    // ...
}
```

The [Check](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Utils.Check.html "Check") offers string check, type check, value check (equal, smaller, larger, ...), check if the value is in the list, check GUID values.

### Standard Extensions

General extensions for:

* Strings - [StringExtensions](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Extensions.StringExtensions.html "String Extensions")
* Dates - [DateTimeExtensions](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Extensions.DateTimeExtensions.html "DateTime Extensions")

### File/Folder Path Helpers

The [PathHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.IO.PathHelper.html "PathHelper") class provides functions to work with files/folder paths.

* [PathHelper.BuildPath](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.IO.PathHelper.html#Kros_IO_PathHelper_BuildPath_ "PathHelper BuildPath") server to link multiple string into one path, as well as standard function [Path.Combine](https://msdn.microsoft.com/en-us/library/dd781134 "Path.Combine") but with some changed details.
* [PathHelper.ReplaceInvalidPathChars](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.IO.PathHelper.html#Kros_IO_PathHelper_ReplaceInvalidPathChars_ "PathHelper ReplaceInvalidChars") in the input string will replace all characters that are not applicable to the file path.

The [PathFormatter](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.IO.PathFormatter.html "PathFormatter") class includes functions for formatting paths to output files so that the result path is valid. The class checks the maximum allowed length of the path so that the result path does not exceed it.

The class is not static. If necessary, you can inherit it and modify its behavior. For simple use, the [Default](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.IO.PathFormatter.html#Kros_IO_PathFormatter_Default "Default") instance is created.

* `PathFormatter.Default.FormatPath("C:\data\export", "exportFile.txt")` returns path `C:\data\export\exportFile.txt`. If the resulting path is too long, the file name is automatically truncated (the suffix is preserved) so the path is valid.
* `PathFormatter.Default.FormatNewPath("C:\data\export", "exportFile.txt")` returns path `C:\data\export\exportFile.txt`. However, if the resulting file already exists, it automatically adds a counter to the name so that the return path is to a non-existent file: `C:\data\export\exportFile (1).txt`. If the resulting path was too long, the file name is automatically truncated so that the path is valid. The suffix and counter are preserved.

The [PathFormatter](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.IO.PathFormatter.html "PathFormatter") class can also be used to create a list of paths with specified parameters. More information is provided in the documentation of each of its functions.

### Database Scheme

It is simple to get a database schema. The database schema includes [TableSchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.TableSchema.html "TableSchema") tables, their [ColumnSchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.ColumnSchema.html "ColumnSchema") columns, [IndexSchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.IndexSchema.html "IndexSchema") indexes, and [ForeignKeySchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.ForeignKeySchema.html "ForeignKeySchema") foreign keys (foreign keys are only supported for SQL Server).

```c#
SqlConnection cn = new SqlConnection("...");

DatabaseSchema schema = DatabaseSchemaLoader.Default.LoadSchema(cn);
```

Since getting a schema is a time-consuming operation, it is a good idea to use the [DatabaseSchemaCache](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.DatabaseSchemaCache.html "DatabaseSchemaCache") cache for the schemas you are reading. It holds the schema once it is retrieved and is returned from the memory upon the next schema request. The class can be used either by creating its instance or by using the static [DatabaseSchemaCache.Default](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.DatabaseSchemaCache.html#Kros_Data_Schema_DatabaseSchemaCache_Default "DatabaseSchemaCache Default") property for ease of use.

```c#
SqlConnection cn = new SqlConnection("...");

// Use to create your own instance.
var cache = new DatabaseSchemaCache();

DatabaseSchema schema = cache.GetSchema(cn);

// Using a static property.
schema = DatabaseSchemaCache.Default.GetSchema(cn);
```

### Bulk Operations - Bulk Insert and Bulk Update

Inserting (`INSERT`) and updating (`UPDATE`) large amounts of data in a database are time-consuming. Therefore, support for rapid mass insertion, `Bulk Insert` and a fast bulk update, `Bulk Update`. The [IBulkInsert](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.IBulkInsert.html "IBulkInsert") and [IBulkUpdate](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.IBulkUpdate.html "IBulkUpdate") interfaces are used. They are implemented for SQL Server in the [SqlServerBulkInsert](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.SqlServer.SqlServerBulkInsert.html "SqlServerBulkInsert") and [SqlServerBulkUpdate](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.SqlServer.SqlServerBulkUpdate.html "SqlServerBulkUpdate") classes. As a data source, it serves any [IDataReader](https://msdn.microsoft.com/en-us/library/sh674a6a "IDataReader") or [DataTable](https://msdn.microsoft.com/en-us/library/9186hy08 "DataTable") table.

Because `IDataReader` is an intricate interface, you just need to implement the simplier interface [IBulkActionDataReader](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.IBulkActionDataReader.html "IBulkActionDataReader"). If the source is a list (`IEnumerable`), it is sufficient to use the [`EnumerableDataReader<T>`](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.EnumerableDataReader-1.html "EnumerableDataReader<T>") class for its bulk insertion.

#### Bulk Insert

```c#
private class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public void InsertManyItems()
{
    IEnumerable<Item> data = GetData();
    using (var reader = new EnumerableDataReader<Item>(data, new string[] { "Id", "Name" }))
    {
        using (var bulkInsert = new SqlServerBulkInsert("connection string"))
        {
            bulkInsert.Insert(reader);
        }
    }
}

private class BulkUpdateItem
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

#### Bulk Update

```c#
public void UpdateManyItems()
    {
    IEnumerable<BulkUpdateItem> data = GetItems();

    using (var reader = new EnumerableDataReader<BulkUpdateItem>(data, new string[] { "Id", "Name" }))
    {
        using (var bulkUpdate = new SqlServerBulkUpdate("connection string"))
            {
            bulkUpdate.DestinationTableName = "TableName";
            bulkUpdate.PrimaryKeyColumn = "Id";
            bulkUpdate.Update(reader);
        }
    }
}
```

### IdGenerator

Generating unique (incrementally) values in databases (most common Id) is not easy. In the namespace `Kros.Data` there is the [IIdGenerator](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IIdGenerator.html "IIdGenerator") interface that describes exactly such a unique value generator. They are currently supported for `SqlServer` and `MsAccess`. We do not create their instances directly but through the [IIdGeneratorFactory](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IIdGeneratorFactory.html "IIdGeneratorFactory") factory class. We can get a Factory with [GetFactory(DbConnection)](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IdGeneratorFactories.html#Kros_Data_IdGeneratorFactories_GetFactory_System_Data_Common_DbConnection_ "GetFactory") in the [IdGeneratorFactories](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IdGeneratorFactories.html "IdGeneratorFactories") class.

```c#
public class PeopleService
{
    private IIdGeneratorFactory _idGeneratorFactory;

    public PeopleService(IIdGeneratorFactory idGeneratorFactory)
    {
        _idGeneratorFactory = Check.NotNull(idGeneratorFactory, nameof(idGeneratorFactory));
    }

    public void GenerateData()
    {
        using (var idGenerator = _idGeneratorFactory.GetGenerator("people", 1000))
        {
            for (int i = 0; i < 1800; i++)
            {
                var person = new Person()
                {
                    Id = idGenerator.GetNext()
                };
            };
        }
    }
}
```

An `IdStore` table is required in the database. For `SQL Server`, the stored procedure `spGetNewId` is required. In order to create everything necessary for the ID generator to work it is possible to use the methods of the individual generators [SqlServerIdGenerator.InitDatabaseForIdGenerator](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.SqlServer.SqlServerIdGenerator.html#Kros_Data_SqlServer_SqlServerIdGenerator_InitDatabaseForIdGenerator "SqlServerIdGenerator InitDatabaseForIdGenerator") for `SQL Server` and [MsAccessIdGenerator.InitDatabaseForIdGenerator](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.MsAccess.MsAccessIdGenerator.html#Kros_Data_MsAccess_MsAccessIdGenerator_InitDatabaseForIdGenerator "MsAccessIdGenerator InitDatabaseForIdGenerator") for `MS Access`.

If your existing implementations do not suit you (for example, you have another id store table) you can create a custom implementation of the [IIdGenerator](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IIdGenerator.html "IIdGenerator") and [IIdGeneratorFactory](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IIdGeneratorFactory.html "IIdGeneratorFactory") interfaces that you register using the [Register method in the IdGeneratorFactories class](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.IdGeneratorFactories.html#Kros_Data_IdGeneratorFactories_Register__1_System_String_System_Func_System_Data_Common_DbConnection_Kros_Data_IIdGeneratorFactory__System_Func_System_String_Kros_Data_IIdGeneratorFactory__ "Register").

### IDiContainer Interface Describing Dependency Injection Container

The [IDiContainer](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Utils.IDiContainer.html "IDiContainer") interface is not directly implemented in `Kros.Utils` because `Kros.Utils` does not have any external dependencies. However, the interface is implemented using the [Unity](https://www.nuget.org/packages/Unity/ "Unity") container in the `KrosUnityContainer` class in the `Kros.Utils.UnityContainer` library.

### Caching

The very simple [Cache<TKey, TValue>](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Caching.Cache-2.html "Cache") is implemented. The class holds values to the specified keys. When the value is retrieved the returned value is already in the cache.

### Unit Testing Helpers

Standard unit tests should be database-independent. But sometimes it is necessary to test the actual database because the test items are directly related to it. To test the actual database you can use the [SqlServerTestHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerTestHelper.html "SqlServerTestHelper") class. It creates a database for testing purposes on the server and runs tests over it. When tests are finished the database is deleted.

```c#
// In the connection string there is no specified database
// because it is automatically created with random name.
// At the end of the job, the database is automatically deleted.

private const string BaseConnectionString = "Data Source=SQLSERVER;Integrated Security=True;";

private const string CreateTestTableScript =
@"CREATE TABLE [dbo].[TestTable] (
    [Id] [int] NOT NULL,
    [Name] [nvarchar](255) NULL,

    CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
) ON [PRIMARY];";

[Fact]
public void DoSomeTestWithDatabase()
{
    using (var serverHelper = new SqlServerTestHelper(BaseConnectionString, "TestDatabase", CreateTestTableScript))
    {
        // Do tests with connection serverHelper.Connection.
    }
}
```

When testing MS Access database you can use [MsAccessTestHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.UnitTests.MsAccessTestHelper.html "MsAccessTestHelper") from [Kros.Utils.MsAccess](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Utils.MsAccess.html "MsAccess") library.

The superstructure over the [SqlServerTestHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerTestHelper.html "SqlServerTestHelper") class is the basic class for creating additional test classes [SqlServerDatabaseTestBase](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerDatabaseTestBase.html "SqlServerDatabaseTestBase"). In a child just rewrite the [BaseConnectionString](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerDatabaseTestBase.html#Kros_UnitTests_SqlServerDatabaseTestBase_BaseConnectionString "BaseConnectionString") property to connect to the database, and then just write the tests. The class in the constructor creates a temporary empty database in which it is possible to "play" and this database is deleted during `Dispose()`. Internally [SqlServerTestHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerTestHelper.html "SqlServerTestHelper") is used which is available in the [ServerHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerDatabaseTestBase.html#Kros_UnitTests_SqlServerDatabaseTestBase_ServerHelper "ServerHelper") property. There is also a link to the database itself.

If you need to initialize the test database (create some tables, fill dates, etc.) just rewrite the [DatabaseInitScripts](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.UnitTests.SqlServerDatabaseTestBase.html#Kros_UnitTests_SqlServerDatabaseTestBase_DatabaseInitScripts "DatabaseInitScripts") property and return the scripts that initialize the database.

There is no need to solve database initialization for individual tests because `xUnit` creates a new instance of the test class for each test. So each test has its own initialized test database.

The class is for `SQL Server`. There is no similar class for `MS Access` (so far).

```c#
public class SomeDatabaseTests
    : Kros.UnitTests.SqlServerDatabaseTestBase
{
    protected override string BaseConnectionString => "Data Source=TESTSQLSERVER;Integrated Security=True";

    [Fact]
    public void Test1()
    {
        using (var cmd = ServerHelper.Connection.CreateCommand())
        {
            // Use cmd to execute queries.
        }
    }

    [Fact]
    public void Test2()
    {

    }
}
```
