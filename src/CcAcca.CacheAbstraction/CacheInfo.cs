// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Generic;
using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction
{
    public class CacheInfo
    {
        #region Constructors

        public CacheInfo()
        {
            ItemAccessStatistics = new List<CacheItemAccessInfo>();
        }

        #endregion


        #region Properties

        public string CacheId { get; set; }
        public string CacheName { get; set; }
        public string InstanceName { get; set; }
        public bool IsPaused { get; set; }
        public bool IsPausable { get; set; }
        public int ItemCount { get; set; }
        public decimal? CacheHitRatio { get; set; }
        public DateTimeOffset? LastFlush { get; set; }
        public DateTimeOffset? LastRead { get; set; }
        public DateTimeOffset? LastUse { get; set; }
        public DateTimeOffset? LastWrite { get; set; }
        public ICollection<CacheItemAccessInfo> ItemAccessStatistics { get; set; }

        #endregion
    }
}