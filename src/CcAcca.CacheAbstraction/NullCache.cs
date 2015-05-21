// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Never actually caches items! Useful when you have a consumer expects to be handed an <see cref="ICache"/>
    /// instance but the context dictates that caching should not actually occur
    /// </summary>
    public class NullCache : CacheBase, INullCache
    {
        #region Constructors

        private NullCache() : base("NullCache") {}

        #endregion


        #region ICache Members

        public void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            //no-op
        }

        public bool Contains(string key)
        {
            return false;
        }


        public int Count
        {
            get { return 0; }
        }


        public void Flush()
        {
            //no-op
        }


        public CacheItem<T> GetCacheItem<T>(string key)
        {
            return null;
        }


        public void Remove(string key)
        {
            //no-op
        }

        #endregion


        #region Class Members

        static NullCache()
        {
            Instance = new NullCache();
        }


        public static ICache Instance { get; private set; }

        #endregion
    }
}