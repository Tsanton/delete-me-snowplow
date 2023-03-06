using Snowplow.Models.Assets;
using Snowplow.Models.Describables;
using Snowplow.Models.Entities;
using Snowplow.Models.Mergeables;

namespace Snowplow;

public interface ISnowplowClient
{
    public Task<T> ExecuteScalar<T>(string query);

    public Task CreateAsset(ISnowflakeAsset asset);
    public Task DeleteAsset(ISnowflakeAsset asset);
    public Task RegisterAsset(ISnowflakeAsset asset, Stack<ISnowflakeAsset> queue);
    public Task DeleteAssets(Stack<ISnowflakeAsset> queue);
    public Task<T?> ShowOne<T>(ISnowflakeDescribable describable) where T: ISnowflakeEntity;
    public Task MergeInto(ISnowflakeMergeable mergeable);
    public Task<T> GetMergeable<T>(T mergeable) where T : ISnowflakeMergeable;

}