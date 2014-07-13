namespace CcAcca.CacheAbstraction.Statistics
{
    /// <summary>
    /// Interface used internally to record when methods on <see cref="ICache"/> have been called
    /// </summary>
    internal interface ICacheActivityRecorder
    {
        void ContainsCalled();
        void FlushCalled();
        void ItemHit(string key);
        void ItemMiss(string key);
        void ItemRetrieved(string key);
        void ItemAddOrUpdated(string key);
        void ItemRemoved(string key);
    }
}