using Snowplow;
using Snowplow.Models.Enums;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;

namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class DatabaseRoleFutureGrantTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public DatabaseRoleFutureGrantTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    [Fact]
    public async Task test_describe_future_grant_for_non_existing_database_role()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        /*Act*/
        try
        {
            
            await _cli.RegisterAsset(dbAsset, _stack);
            
            /*Act*/
            var roleGrants = await _cli.ShowMany<Entities.FutureGrant>(new Describables.FutureGrant
            {
                Target = new Describables.DatabaseRole
                {
                    Name = "I_DONT_EXIST_DATABASE_ROLE",
                    DatabaseName = dbAsset.Name
                } 
            });
            
            /*Assert*/
            Assert.Null(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_describe_future_grant_for_database_role_in_non_existing_database()
    {
        /*Arrange & Act*/
        try
        {
            var roleGrants = await _cli.ShowMany<Entities.FutureGrant>(new Describables.FutureGrant
            {
                Target = new Describables.DatabaseRole
                {
                    Name = "I_DONT_EXIST_DATABASE_ROLE",
                    DatabaseName = "I_DONT_EXIST_EITHER_DATABASE"
                } 
            });
            
            /*Assert*/
            Assert.Null(roleGrants);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_database_role_future_database_object_grant()
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
        var grant = new Assets.Grants.DatabaseObjectFutureGrant<Assets.DatabaseRole>
        {
            Grantable = roleAsset, 
            DatabaseName = dbAsset.Name,
            GrantTarget = SnowflakeObject.Table,
        };
        var grantAction = new Assets.GrantAction
        {
            Target = grant,
            Privileges = new List<Privilege> { Privilege.Select, Privilege.References}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grantAction, _stack);
            var roleGrants = await _cli.ShowMany<Entities.FutureGrant>(new Describables.FutureGrant
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
    
    [Fact]
    public async Task test_database_role_schema_object_future_grant()
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
            Target = new Assets.Grants.SchemaObjectFutureGrant<Assets.DatabaseRole>
            {
                Grantable = roleAsset, 
                DatabaseName = dbAsset.Name, 
                SchemaName = schemaAsset.Name,
                GrantTarget = SnowflakeObject.Table,
            },
            Privileges = new List<Privilege> { Privilege.Select, Privilege.References}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(schemaAsset, _stack);
            await _cli.RegisterAsset(roleAsset, _stack);
            await _cli.RegisterAsset(grant, _stack);
            var roleGrants = await _cli.ShowMany<Entities.FutureGrant>(new Describables.FutureGrant
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