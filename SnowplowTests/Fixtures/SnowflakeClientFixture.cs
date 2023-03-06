using Snowflake.Client;
using Snowflake.Client.Model;
using Snowplow;

namespace SnowplowTests.Fixtures;

[CollectionDefinition("SnowflakeClientSetupCollection")]
public class DatabaseCollection : ICollectionFixture<SnowflakeClientFixture>
{
    // A class with no code, only used to define the collection
}

public class SnowflakeClientFixture: IDisposable
{
    public readonly SnowplowClient Plow;

    public SnowflakeClientFixture()
    {
        if ((Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "TEST") == "UNIT_TEST")
        {
            //TODO: Fix this stupid shit
            DotNetEnv.Env.Load("/Users/a5050nc/Code/c-sharp/Snowplow/.env");
        }
        Console.WriteLine("Setting up");
        var cli = new SnowflakeClient(
            authInfo: new AuthInfo
            {
                Account = Environment.GetEnvironmentVariable("SNOWFLAKE_ACCOUNT"),
                Password = Environment.GetEnvironmentVariable("SNOWFLAKE_PWD"),
                User = Environment.GetEnvironmentVariable("SNOWFLAKE_UID"),
                Region = Environment.GetEnvironmentVariable("SNOWFLAKE_REGION"),
            },
            urlInfo: new UrlInfo
            {
                Host = Environment.GetEnvironmentVariable("SNOWFLAKE_HOST"),
                Protocol = "https",
                Port = 443
            },
            sessionInfo: new SessionInfo
            {
                Role = Environment.GetEnvironmentVariable("SNOWFLAKE_ROLE"),
                Warehouse = Environment.GetEnvironmentVariable("SNOWFLAKE_WH")
            });

        Plow = new SnowplowClient(cli);
    }
    
    public void Dispose()
    {
        Console.WriteLine("Disposing");
    }
}