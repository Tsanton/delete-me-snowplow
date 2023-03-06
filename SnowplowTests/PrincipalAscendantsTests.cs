using Snowplow;
using Assets = Snowplow.Models.Assets;
using Entities = Snowplow.Models.Entities;
using Describables = Snowplow.Models.Describables;
using SnowplowTests.Fixtures;


namespace SnowplowTests;

[Collection("SnowflakeClientSetupCollection")]
public class PrincipalAscendantsTests
{
    private readonly SnowplowClient _cli;
    private readonly Stack<Assets.ISnowflakeAsset> _stack;

    public PrincipalAscendantsTests(SnowflakeClientFixture fixture)
    {
        _cli = fixture.Plow;
        _stack = new Stack<Assets.ISnowflakeAsset>();
    }
    
    /// <summary>
    /// We know that ACCOUNTADMIN is the direct ascendant of SYSADMIN 
    /// </summary>
    [Fact]
    public async Task test_sysadmin_ascendants()
    {
        /*Arrange & Act*/
        var dbAscendants = await _cli.ShowOne<Entities.PrincipalAscendants>(new Describables.PrincipalAscendants
        {
            RolePrincipal = new Describables.Role{Name = "SYSADMIN"}
        });

        /*Assert*/
        Assert.NotNull(dbAscendants);
        Assert.NotNull(dbAscendants!.Ascendants);
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
        Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
    }
    
    // /// <summary>
    // /// We know that ACCOUNTADMIN is the direct ascendant of SECURITYADMIN
    // /// </summary>
    [Fact]
    public async Task test_security_admin_ascendants()
    {
        /*Arrange & Act*/
        var dbAscendants = await _cli.ShowOne<Entities.PrincipalAscendants>(new Describables.PrincipalAscendants
        {
            RolePrincipal = new Describables.Role{Name = "SECURITYADMIN"}
        });

        /*Assert*/
        Assert.NotNull(dbAscendants);
        Assert.NotNull(dbAscendants!.Ascendants);
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
        Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
    }
    
    
    // /// <summary>
    // /// We know that ACCOUNTADMIN is the direct ascendant of SECURITYADMIN whom is the direct ascendant of USERADMIN
    // /// </summary>
    [Fact]
    public async Task test_user_admin_ascendants()
    {
        /*Arrange & Act*/
        var dbAscendants = await _cli.ShowOne<Entities.PrincipalAscendants>(new Describables.PrincipalAscendants
        {
            RolePrincipal = new Describables.Role{Name = "USERADMIN"}
        });

        /*Assert*/
        Assert.NotNull(dbAscendants);
        Assert.NotNull(dbAscendants!.Ascendants);
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "SECURITYADMIN");
        Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "SECURITYADMIN").Select(y => y.DistanceFromSource).First());
        Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
        Assert.Equal(1, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
    }
    
    
    // /// <summary>
    // /// We know that ACCOUNTADMIN is the direct ascendant of SECURITYADMIN whom is the direct ascendant of USERADMIN
    // /// </summary>
    [Fact]
    public async Task test_ascendants_with_database_roles()
    {
        /*Arrange*/
        var db = new Assets.Database
        {
            Name = "TEST_SNOWPLOW_DB",
            Comment = "Integration test database from the Snowplow test suite",
            Owner = "SYSADMIN"
        };
        var dr1 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DB_SYS_ADMIN",
            DatabaseName = db.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var dr2 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DB_SCHEMA_RWC",
            DatabaseName = db.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        var dr3 = new Assets.DatabaseRole
        {
            Name = "TEST_SNOWPLOW_DB_SCHEMA_RW",
            DatabaseName = db.Name,
            Comment = "Integration test role from the Snowplow test suite",
            Owner = "USERADMIN"
        };
        
        var rel1 = new Assets.RoleInheritance
        {
            ChildRole = dr1,
            ParentPrincipal = new Assets.Role { Name = "SYSADMIN", Comment = "", Owner = "" }
        };
        var rel2 = new Assets.RoleInheritance
        {
            ChildRole = dr2,
            ParentPrincipal = dr1
        };
        var rel3 = new Assets.RoleInheritance
        {
            ChildRole = dr3,
            ParentPrincipal = dr2
        };
        try
        {
            /*Act*/
            await _cli.RegisterAsset(db, _stack);
            await _cli.RegisterAsset(dr1, _stack);
            await _cli.RegisterAsset(dr2, _stack);
            await _cli.RegisterAsset(dr3, _stack);
            await _cli.RegisterAsset(rel1, _stack);
            await _cli.RegisterAsset(rel2, _stack);
            await _cli.RegisterAsset(rel3, _stack);
            
            var dbAscendants = await _cli.ShowOne<Entities.PrincipalAscendants>(new Describables.PrincipalAscendants
            {
                RolePrincipal = new Describables.DatabaseRole{Name = dr3.Name, DatabaseName = dr3.DatabaseName}
            });
            
            /*Assert*/
            Assert.NotNull(dbAscendants);
            Assert.NotNull(dbAscendants!.Ascendants);
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == dr2.GetIdentifier());
            Assert.Equal(0, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == dr2.GetIdentifier()).Select(y => y.DistanceFromSource).First());
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == dr1.GetIdentifier());
            Assert.Equal(1, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == dr1.GetIdentifier()).Select(y => y.DistanceFromSource).First());
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "SYSADMIN");
            Assert.Equal(2, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "SYSADMIN").Select(y => y.DistanceFromSource).First());
            Assert.Contains(dbAscendants.Ascendants, x => x.GranteeIdentifier == "ACCOUNTADMIN");
            Assert.Equal(3, dbAscendants.Ascendants.Where(x => x.GranteeIdentifier == "ACCOUNTADMIN").Select(y => y.DistanceFromSource).First());
        }
        finally
        {
            await _cli.DeleteAssets(_stack);
        }
    }
}


