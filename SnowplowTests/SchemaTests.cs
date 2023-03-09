using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;

namespace SnowplowTests;



[Collection("SnowflakeClientSetupCollection")]
public class SchemaTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public SchemaTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    [Fact]
    public async Task test_create_schema()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var schemaAsset = new Assets.Schema
        {
            Database = dbAsset,
            Name = "TEST_SNOWPLOW_SCHEMA",
            Comment = "Integration test schema from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(schemaAsset, _stack);
            var schemaEntity = await _cli.ShowOne<Entities.Schema>(new Describables.Schema
            {
                DatabaseName = dbAsset.Name,
                SchemaName = schemaAsset.Name
            });

            /*Assert*/
            Assert.NotNull(schemaEntity);
            Assert.Equal(schemaAsset.Name, schemaEntity!.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_describe_schema_in_non_existing_database()
    {
        /*Arrange & Act*/
        var schemaEntity = await _cli.ShowOne<Entities.Schema>(new Describables.Schema
        {
            DatabaseName = "DO_NOT_EXIST_DB",
            SchemaName = "DO_NOT_EXIST_SCHEMA"
        });


        /*Assert*/
        Assert.Null(schemaEntity);
    }


    
}