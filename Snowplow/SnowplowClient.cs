using System.Text.Json;
using Snowflake.Client;
using Snowplow.Models.Assets;
using Snowplow.Models.Describables;
using Snowplow.Models.Entities;
using Snowplow.Models.Mergeables;

namespace Snowplow;
public class SnowplowClient: ISnowplowClient
{
    private readonly ISnowflakeClient _cli;

    public SnowplowClient(ISnowflakeClient cli)
    {
        _cli = cli;
    }
    
    public async Task<T> ExecuteScalar<T>(string query)
    {
        return await ((SnowflakeClient)_cli).ExecuteScalarAsync<T>(query);
    }

    public async Task CreateAsset(ISnowflakeAsset asset)
    {
        await _CreateAsset(asset);
    }

    public async Task DeleteAsset(ISnowflakeAsset asset)
    {
        await _DeleteAsset(asset);
    }

    public async Task RegisterAsset(ISnowflakeAsset asset, Stack<ISnowflakeAsset> queue)
    {
        await _CreateAsset(asset);
        queue.Push(asset);
    }

    public async Task DeleteAssets(Stack<ISnowflakeAsset> queue)
    {
        while (queue.TryPop(out var asset))
        {
            await _DeleteAsset(asset);
        }
    }

    public async Task<T?> ShowOne<T>(ISnowflakeDescribable describable) where T : ISnowflakeEntity
    {
        try
        {
            var query = describable.GetDescribeStatement();
            //TODO: Fix this horrible hack
            if (query.ToUpper().Contains("AS PROCEDURE"))
            {
                var res = await ((SnowflakeClient)_cli).ExecuteScalarAsync<T>(query);
                return res != null ? res : default;
            }
            else
            {
                var res = await ((SnowflakeClient)_cli).QueryAsync<T>(query);
                return res != null && res.Any() ? res.First() : default;
            }
        }
        catch (Snowflake.Client.Model.SnowflakeException e)
        {
            //If you query for objects that does not exist the client throws a SnowflakeException.
            //For non existing data objects: Object 'XXXX' does not exist, or operation cannot be performed.
            //For non existing databases: Database 'XXXX' does not exist or not authorized.
            //For non existing role inheritance: Role relationship does not exist or not authorized
            if (e.Message.Contains("does not exist"))
            {
                return default;
            }
            throw;
        }
    }
    
    public async Task<List<T>?> ShowMany<T>(ISnowflakeDescribable describable) where T : ISnowflakeEntity
    {
        try
        {
            var query = describable.GetDescribeStatement();
            var results = await ((SnowflakeClient)_cli).ExecuteScalarAsync<List<T>>(query);
            return results.Count == 0 ? null : results;
        }
        catch (Snowflake.Client.Model.SnowflakeException e)
        {
            //If you query for objects that does not exist the client throws a SnowflakeException.
            //For non existing data objects: Object 'XXXX' does not exist, or operation cannot be performed.
            //For non existing databases: Database 'XXXX' does not exist or not authorized.
            if (e.Message.Contains("does not exist"))
            {
                return null;
            }
            throw;
        }
    }
    
    public Task MergeInto(ISnowflakeMergeable mergeable)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetMergeable<T>(T mergeable) where T : ISnowflakeMergeable
    {
        throw new NotImplementedException();
    }
    
    private async Task _CreateAsset(ISnowflakeAsset asset)
    {
        foreach (var query in asset.GetCreateStatement().Trim().Split(";"))
        {
            //TODO: Fix this ugly /n issue
            if (query.Length > 5) await _cli.ExecuteAsync(query);
        }
    }
    
    private async Task _DeleteAsset(ISnowflakeAsset asset)
    {
        foreach (var query in asset.GetDeleteStatement().Trim().Split(";")) 
        {
            //TODO: Fix this ugly /n issue
            if (query.Length > 5) await _cli.ExecuteAsync(query);
        }
    }
}
