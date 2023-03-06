using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;


namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class PrincipalDescendantsTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public PrincipalDescendantsTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    /// <summary>
    /// We know that SYSADMIN and SECURITYADMIN are direct descendants of ACCOUNTADMIN
    /// </summary>
    [Fact]
    public async Task test_account_admin_descendants()
    {
        /*Arrange & Act*/
        var dbDescendants = await _cli.ShowOne<Entities.PrincipalDescendants>(new Describables.PrincipalDescendants
        {
            Principal = new Describables.Role{Name = "ACCOUNTADMIN"}
        });

        /*Assert*/
        Assert.NotNull(dbDescendants);
        Assert.NotNull(dbDescendants!.DescendantRoles);
        Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == "SYSADMIN");
        Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == "SECURITYADMIN");
    }

    [Fact]
    public async Task test_role_to_role_descendants()
    {
        /*Arrange*/
        var childRole = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_CHILD_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var parentRole = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_PARENT_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var rel = new Assets.RoleInheritance
        {
            ChildRole = childRole,
            ParentPrincipal = parentRole
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(childRole, _stack);
            await _cli.RegisterAsset(parentRole, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbDescendants = await _cli.ShowOne<Entities.PrincipalDescendants>(new Describables.PrincipalDescendants
            {
                Principal = new Describables.Role{Name = parentRole.Name}
            });

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Single(dbDescendants!.DescendantRoles);
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childRole.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_role_to_roles_descendants()
    {
        /*Arrange*/
        var childRole1 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_CHILD_ROLE_1",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var childRole2 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_CHILD_ROLE_2",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var parentRole = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_PARENT_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var rel1 = new Assets.RoleInheritance
        {
            ChildRole = childRole1,
            ParentPrincipal = parentRole
        };
        var rel2 = new Assets.RoleInheritance
        {
            ChildRole = childRole2,
            ParentPrincipal = parentRole
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(childRole1, _stack);
            await _cli.RegisterAsset(childRole2, _stack);
            await _cli.RegisterAsset(parentRole, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            var dbDescendants = await _cli.ShowOne<Entities.PrincipalDescendants>(new Describables.PrincipalDescendants
            {
                Principal = new Describables.Role{Name = parentRole.Name}
            });

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Equal(2, dbDescendants!.DescendantRoles.Count);
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childRole1.Name);
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childRole2.Name);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    
    [Fact]
    public async Task test_role_to_role_and_database_role_descendants()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        var childRole = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_CHILD_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var childDatabaseRole = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_CHILD_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var parentRole = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_PARENT_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var rel1 = new Assets.RoleInheritance
        {
            ChildRole = childRole,
            ParentPrincipal = parentRole
        };
        var rel2 = new Assets.RoleInheritance
        {
            ChildRole = childDatabaseRole,
            ParentPrincipal = parentRole
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(childRole, _stack);
            await _cli.RegisterAsset(childDatabaseRole, _stack);
            await _cli.RegisterAsset(parentRole, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            var dbDescendants = await _cli.ShowOne<Entities.PrincipalDescendants>(new Describables.PrincipalDescendants
            {
                Principal = new Describables.Role{Name = parentRole.Name}
            });

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Equal(2, dbDescendants!.DescendantRoles.Count);
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childRole.Name);
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childDatabaseRole.GetIdentifier());
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_database_role_to_database_roles_descendants()
    {
        /*Arrange*/
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        var childDatabaseRole1 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_CHILD_DATABASE_ROLE1",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var childDatabaseRole2 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_CHILD_DATABASE_ROLE2",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var parentDatabaseRole = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_PARENT_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var rel1 = new Assets.RoleInheritance
        {
            ChildRole = childDatabaseRole1,
            ParentPrincipal = parentDatabaseRole
        };
        var rel2 = new Assets.RoleInheritance
        {
            ChildRole = childDatabaseRole2,
            ParentPrincipal = parentDatabaseRole
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(parentDatabaseRole, _stack);
            await _cli.RegisterAsset(childDatabaseRole1, _stack);
            await _cli.RegisterAsset(childDatabaseRole2, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            var dbDescendants = await _cli.ShowOne<Entities.PrincipalDescendants>(new Describables.PrincipalDescendants
            {
                Principal = new Describables.DatabaseRole{Name = parentDatabaseRole.Name, DatabaseName = parentDatabaseRole.DatabaseName}
            });

            /*Assert*/
            Assert.NotNull(dbDescendants);
            Assert.Equal(2, dbDescendants!.DescendantRoles.Count);
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childDatabaseRole1.GetIdentifier());
            Assert.Contains(dbDescendants.DescendantRoles, x => x.GrantedIdentifier == childDatabaseRole2.GetIdentifier());
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}