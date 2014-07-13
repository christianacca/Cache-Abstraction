namespace CcAcca.CacheAbstraction.Statistics
{
    public interface IStatisticsCache : ICache
    {
        CacheStatistics Statistics { get; }
    }
}