using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;


namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class DatabaseTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public DatabaseTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    [Fact]
    public async Task test_create_database()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            var dbEntity = await _cli.ShowOne<Entities.Database>(new Describables.Database { Name = dbAsset.Name });
            /*Assert*/
            Assert.NotNull(dbEntity);
            Assert.Equal(dbAsset.Name, dbEntity!.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_describe_non_existing_database()
    {
        /*Arrange & Act*/
        var dbEntity = await _cli.ShowOne<Entities.Database>(new Describables.Database { Name = "DO_NOT_EXIST_DB" });

        /*Assert*/
        Assert.Null(dbEntity);
    }
}