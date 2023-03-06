using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;

namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class RoleTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public RoleTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    [Fact]
    public async Task test_create_role()
    {
        /*Arrange*/
        var roleAsset = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(roleAsset, _stack);
            var dbRole= await _cli.ShowOne<Entities.Role>(new Describables.Role{ Name = roleAsset.Name });

            /*Assert*/
            Assert.NotNull(dbRole);
            Assert.Equal(roleAsset.Name, dbRole!.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_describe_non_exising_role()
    {
        /*Arrange & Act*/
        var dbRole = await _cli.ShowOne<Entities.Role>(new Describables.Role{ Name = "I_SURELY_DO_NOT_EXIST_ROLE" });

        /*Assert*/
        Assert.Null(dbRole);
    }
}