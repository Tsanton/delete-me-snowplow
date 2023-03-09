using System.Data;
using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;

namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class RoleInheritanceTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public RoleInheritanceTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }

    #region RoleToRole
    [Fact]
    public async Task test_show_non_existing_role_to_role_inheritance()
    {
        /*Arrange*/
        var r1 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE_1",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var r2 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE_2",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(r1, _stack);
            await _cli.RegisterAsset(r2, _stack);
            var dbInheritance = await _cli.ShowOne<Entities.RoleInheritance>(new Describables.RoleInheritance
            {
                InheritedRole = new Describables.Role{Name = r1.Name},
                ParentPrincipal = new Describables.Role{Name = r2.Name},
            });

            /*Assert*/
            Assert.Null(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    [Fact]
    public async Task test_show_role_to_role_inheritance()
    {
        /*Arrange*/
        var r1 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE_1",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var r2 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE_2",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var rel = new Assets.RoleInheritance
        {
            ChildRole = r1,
            ParentPrincipal = r2
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(r1, _stack);
            await _cli.RegisterAsset(r2, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbInheritance = await _cli.ShowOne<Entities.RoleInheritance>(new Describables.RoleInheritance
            {
                InheritedRole = new Describables.Role{Name = r1.Name},
                ParentPrincipal = new Describables.Role{Name = r2.Name},
            });

            /*Assert*/
            Assert.NotNull(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion

    #region RoleToDatabaseRole
    /// <summary>
    /// Account roles can not be granted to database roles
    /// </summary>
    [Fact]
    public async Task test_show_role_to_database_role_inheritance_in_non_existing_database()
    {
        /*Arrange*/
        var r1 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE_1",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(r1, _stack);
            var dbInheritance = await _cli.ShowOne<Entities.RoleInheritance>(new Describables.RoleInheritance
            {
                ParentPrincipal = new Describables.Role{Name = r1.Name},
                InheritedRole = new Describables.DatabaseRole()
                {
                    Name = "I_DONT_EXIST_ROLE",
                    DatabaseName = "I_DONT_EXIST_EITHER_DATABASE"
                },
            });

            /*Assert*/
            Assert.Null(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    
    /// <summary>
    /// Account roles can not be granted to database roles
    /// </summary>
    [Fact]
    public async Task test_show_role_to_database_role_inheritance()
    {
        /* Arrange */
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var r1 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var dr1 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.DatabaseRole{Name = "USERADMIN"}
        };
        var rel = new Assets.RoleInheritance
        {
            ChildRole = r1,
            ParentPrincipal = dr1
        };
        
        /* Act and Assert */
        await Assert.ThrowsAsync<ConstraintException>(() => _cli.RegisterAsset(rel, _stack));
    }
    #endregion
    
    
    #region DatabaseRoleToRole
    [Fact]
    public async Task test_show_database_role_to_role_inheritance()
    {
        /* Arrange */
        var dbAsset = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var dr1 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE",
            DatabaseName = dbAsset.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var r1 = new Assets.Role
        {
            Name = "TEST_SNOWPLOW_ROLE",
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var rel = new Assets.RoleInheritance
        {
            ChildRole = dr1,
            ParentPrincipal = r1
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(dbAsset, _stack);
            await _cli.RegisterAsset(dr1, _stack);
            await _cli.RegisterAsset(r1, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbInheritance = await _cli.ShowOne<Entities.RoleInheritance>(new Describables.RoleInheritance
            {
                InheritedRole = new Describables.DatabaseRole{Name = dr1.Name, DatabaseName = dr1.DatabaseName},
                ParentPrincipal = new Describables.Role{Name = r1.Name}
            });

            /*Assert*/
            Assert.NotNull(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion
    
    #region DatabaseRoleToDatabaseRole
    [Fact]
    public async Task test_show_database_role_to_database_role_cross_database_inheritance()
    {
        /* Arrange */
        var db1 = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB1",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var db2 = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB2",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var dr1 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE1",
            DatabaseName = db1.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.DatabaseRole{Name = "USERADMIN"}
        };
        var dr2 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE2",
            DatabaseName = db2.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.DatabaseRole{Name = "USERADMIN"}
        };
        var rel = new Assets.RoleInheritance
        {
            ChildRole = dr1,
            ParentPrincipal = dr2
        };
        
        /* Act and Assert */
        await Assert.ThrowsAsync<ConstraintException>(() => _cli.RegisterAsset(rel, _stack));
    }
    
    [Fact]
    public async Task test_show_database_role_to_database_role_same_database_inheritance()
    {
        /* Arrange */
        var db = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB1",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = new Assets.Role{Name = "SYSADMIN"}
        };
        var dr1 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE1",
            DatabaseName = db.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var dr2 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DATABASE_ROLE2",
            DatabaseName = db.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = new Assets.Role{Name = "USERADMIN"}
        };
        var rel = new Assets.RoleInheritance
        {
            ChildRole = dr1,
            ParentPrincipal = dr2
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(db, _stack);
            await _cli.RegisterAsset(dr1, _stack);
            await _cli.RegisterAsset(dr2, _stack);
            await _cli.RegisterAsset(rel, _stack);
            var dbInheritance = await _cli.ShowOne<Entities.RoleInheritance>(new Describables.RoleInheritance
            {
                InheritedRole = new Describables.DatabaseRole{Name = dr1.Name, DatabaseName = dr1.DatabaseName},
                ParentPrincipal = new Describables.DatabaseRole{Name = dr2.Name, DatabaseName = dr2.DatabaseName},
            });

            /*Assert*/
            Assert.NotNull(dbInheritance);
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
    #endregion
}

