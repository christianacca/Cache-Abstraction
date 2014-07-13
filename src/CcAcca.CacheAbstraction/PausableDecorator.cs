// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Extends a <see cref="ICache"/> with the ability to be paused and resumed
    /// </summary>
    public class PausableDecorator : CacheDecorator, IPausableCache
    {
        #region Constructors

        public PausableDecorator(ICache cache) : base(cache) {}

        #endregion


        #region IStoppableCache Members

        public override void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            if (IsPaused) return;
            base.AddOrUpdate(key, value, cachePolicy);
        }


        public override bool Contains(string key)
        {
            if (IsPaused) return false;
            return base.Contains(key);
        }

        public override T GetData<T>(string key)
        {
            if (IsPaused) return default(T);
            return base.GetData<T>(key);
        }


        public bool IsPaused { get; set; }


        public override void Remove(string key)
        {
            if (IsPaused) return;
            base.Remove(key);
        }

        #endregion
    }
}