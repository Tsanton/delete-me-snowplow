using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;

namespace SnowplowTests;

public partial class TableTests
{
    [Fact]
    public async Task test_table_time()
    {
        /*Arrange*/
        var (dbAsset, schemaAsset) = await BootstrapTableAssets();
        var col1 = new Assets.Time("TIME_COLUMN")
        {
            Precision = 0,
            PrimaryKey = false,
            Nullable = false,
            Unique = false,
            ForeignKey = null,
            DefaultValue = null
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
            Assert.Equal("TIME", dbTable.Columns.First().ColumnType.Type);
            Assert.Equal(col1.Precision, dbTable.Columns.First().ColumnType.Precision!.Value);
            Assert.False(dbTable.Columns.First().PrimaryKey);
            Assert.False(dbTable.Columns.First().ColumnType.Nullable);
            Assert.False(dbTable.Columns.First().UniqueKey);
            Assert.Null(dbTable.Columns.First().Default);
            Assert.Null(dbTable.Columns.First().Expression);
            Assert.Null(dbTable.Columns.First().Check);
            Assert.Null(dbTable.Columns.First().PolicyName);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}