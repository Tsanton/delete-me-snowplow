using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;

namespace SnowplowTests;


[Collection("SnowflakeClientSetupCollection")]
public class DatabaseRoleTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public DatabaseRoleTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }

    [Fact]
    public async Task test_create_database_role()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var roleAsset = new Assets.DatabaseRole()
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            var dbRole = await _cli.ShowOne<Entities.DatabaseRole>(new Describables.DatabaseRole
            {
                Name = roleAsset.Name,
                DatabaseName = dbAsset.Name
            });

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
    public async Task test_describe_non_exising_database_role()
    {
        /*Arrange */ 
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
            
            var dbRole = await _cli.ShowOne<Entities.DatabaseRole>(new Describables.DatabaseRole
            {
                Name = "I_SURELY_DO_NOT_EXIST_DATABASE_ROLE",
                DatabaseName = dbAsset.Name
            });

            /*Assert*/
            Assert.Null(dbRole);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_describe_database_role_in_non_exising_database()
    {
        /*Arrange & Act*/
        var dbRole = await _cli.ShowOne<Entities.DatabaseRole>(new Describables.DatabaseRole
        {
            Name = "I_SURELY_DO_NOT_EXIST_DATABASE_ROLE",
            DatabaseName = "I_DONT_EXIST_DATABASE"
        });

        /*Assert*/
        Assert.Null(dbRole);
    }
    
    [Fact]
    public async Task test_database_role_with_database_role_owner()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var databaseRoleOwner = new Assets.DatabaseRole()
        {
            Name = "TEST_SNOWPLOW_DATABASE_OWNER_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var databaseRoleOwned = new Assets.DatabaseRole()
        {
            Name = "TEST_SNOWPLOW_DATABASE_OWNED_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = databaseRoleOwner
        };
        var rel = new Assets.RoleInheritance //Must grant owner role to USERADMIN in order to be able to delete owned role
        {
            ChildRole = databaseRoleOwner,
            ParentPrincipal = new Assets.Role{Name = "USERADMIN"}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(databaseRoleOwner, _stack);
            await _cli.RegisterAsset(rel, _stack);
            await _cli.RegisterAsset(databaseRoleOwned, _stack);
            var dbRole = await _cli.ShowOne<Entities.DatabaseRole>(new Describables.DatabaseRole
            {
                Name = databaseRoleOwned.Name,
                DatabaseName = dbAsset.Name
            });

            /*Assert*/
            Assert.NotNull(dbRole);
            Assert.Equal(databaseRoleOwned.Name, dbRole!.Name);
            Assert.Equal(databaseRoleOwner.Name, databaseRoleOwner.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}