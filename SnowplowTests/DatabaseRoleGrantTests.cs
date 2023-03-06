using Snowplow;
using Snowplow.Models.Enums;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;

namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class DatabaseRoleGrantTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public DatabaseRoleGrantTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    [Fact]
    public async Task test_database_role_database_grant()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        var roleAsset = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var grant = new Assets.GrantAction
        {
            Target = new Assets.Grants.DatabaseGrant<Assets.DatabaseRole> { Grantable = roleAsset, DatabaseName = dbAsset.Name},
            Privileges = new List<Privilege> { Privilege.CreateDatabaseRole, Privilege.CreateSchema}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<Entities.Grant>(new Describables.Grant
            {
                Target = new Describables.DatabaseRole { Name = roleAsset.Name, DatabaseName = dbAsset.Name }
            });

            /*Assert*/
            Assert.NotNull(roleGrants);
            Assert.NotEmpty(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_database_role_schema_grant()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        var schemaAsset = new Assets.Schema
        {
            Database = dbAsset,
            Name = "TEST_SNOWPLOW_SCHEMA",
            Comment = "Integration test schema from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        var roleAsset = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var grant = new Assets.GrantAction
        {
            Target = new Assets.Grants.SchemaGrant<Assets.DatabaseRole> { Grantable = roleAsset, DatabaseName = dbAsset.Name, SchemaName = schemaAsset.Name},
            Privileges = new List<Privilege> { Privilege.Monitor, Privilege.Usage}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(schemaAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<Entities.Grant>(new Describables.Grant
            {
                Target = new Describables.DatabaseRole { Name = roleAsset.Name, DatabaseName = dbAsset.Name} 
            });

            /*Assert*/
            Assert.NotNull(roleGrants);
            Assert.NotEmpty(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}
