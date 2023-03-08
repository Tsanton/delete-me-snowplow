using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;

namespace SnowplowTests;

public partial class TableTests
{
    #region integer
    [Fact]
    public async Task test_table_integer()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Assets.Number("INTEGER_COLUMN")
        {
            Precision = 38,
            Scale = 0,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Identity = null
        };
        var tableAsset = new Assets.Table
        {
            DatabaseName = dbAsset.Name,
            SchemaName = schemaAsset.Name,
            TableName = "TEST_TABLE",
            Columns = new Queue<Assets.ISnowflakeColumn>(),
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        tableAsset.Columns.Enqueue(col1);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Entities.Table>(new Describables.Table
            {
                DatabaseName = dbAsset.Name,
                SchemaName = schemaAsset.Name,
                TableName = tableAsset.TableName
            });

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Single(dbTable!.Columns);
            Assert.Equal(col1.Name, dbTable.Columns.First().Name);
            Assert.Equal("FIXED", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Scale!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().AutoIncrement);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_table_integer_auto_increment()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Assets.Number("INTEGER_COLUMN")
        {
            Precision = 38,
            Scale = 0,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Identity = new Assets.Identity
            {
                StartNumber = 0,
                IncrementNumber = 1
            }
        };
        var tableAsset = new Assets.Table
        {
            DatabaseName = dbAsset.Name,
            SchemaName = schemaAsset.Name,
            TableName = "TEST_TABLE",
            Columns = new Queue<Assets.ISnowflakeColumn>(),
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        tableAsset.Columns.Enqueue(col1);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Entities.Table>(new Describables.Table
            {
                DatabaseName = dbAsset.Name,
                SchemaName = schemaAsset.Name,
                TableName = tableAsset.TableName
            });

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Single(dbTable!.Columns);
            Assert.Equal(col1.Name, dbTable.Columns.First().Name);
            Assert.Equal("FIXED", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Scale!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Equal(col1.Identity.ToString(), dbTable.Columns.First().Default);
            Assert.Equal(col1.Identity.ToString(), dbTable.Columns.First().AutoIncrement);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion
    
    #region decimal
    [Fact]
    public async Task test_table_decimal()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Assets.Number("NUMBER_COLUMN")
        {
            Precision = 38,
            Scale = 37,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null,
            Identity = null
        };
        var tableAsset = new Assets.Table
        {
            DatabaseName = dbAsset.Name,
            SchemaName = schemaAsset.Name,
            TableName = "TEST_TABLE",
            Columns = new Queue<Assets.ISnowflakeColumn>(),
            DataRetentionTimeInDays = 0,
            Comment = "TEST_TABLE"
        };
        tableAsset.Columns.Enqueue(col1);
        try
        {
            /*Act*/
            await _cli.RegisterAsset(tableAsset, _stack);
            var dbTable = await _cli.ShowOne<Entities.Table>(new Describables.Table
            {
                DatabaseName = dbAsset.Name,
                SchemaName = schemaAsset.Name,
                TableName = tableAsset.TableName
            });

            /*Assert*/
            Assert.NotNull(dbTable);
            Assert.Single(dbTable!.Columns);
            Assert.Equal(col1.Name, dbTable.Columns.First().Name);
            Assert.Equal("FIXED", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.Equal(col1.Scale, dbTable.Columns.First().ColumnType.Scale!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().AutoIncrement);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion
}