// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Defines the extended behaviours that an <see cref="ICache"/> instance should receive
    /// </summary>
    public class CacheDecoratorOptions : ICloneable
    {
        private CacheStatistics _statistics;

        /// <summary>
        /// Extend the cache with <see cref="IMultiThreadProtectedCache"/> behaviour?
        /// </summary>
        public virtual bool? IsMultiThreadProtectionOn { get; set; }

        /// <summary>
        /// Extend the cache with <see cref="IPausableCache"/> behaviour?
        /// </summary>
        public virtual bool? IsPausableOn { get; set; }

        /// <summary>
        /// Extend the cache with <see cref="IStatisticsCache"/> behaviour?
        /// </summary>
        public virtual bool? IsStatisticsOn { get; set; }


        /// <summary>
        /// The statistics that a <see cref="IStatisticsCache"/> should record
        /// </summary>
        /// <remarks>
        /// By default if not set explicitly, it will default to <see cref="CacheStatistics.All"/> whenever
        /// <see cref="IsStatisticsOn"/> is <c>true</c>
        /// </remarks>
        public virtual CacheStatistics Statistics
        {
            get { return _statistics ?? ((IsStatisticsOn ?? false) ? CacheStatistics.All : CacheStatistics.Empty); }
            set { _statistics = value; }
        }

        public virtual CacheDecoratorOptions Clone()
        {
            return (CacheDecoratorOptions) MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }


        private static CacheDecoratorOptions _defaultInstance;

        static CacheDecoratorOptions()
        {
            _defaultInstance = new CacheDecoratorOptions
                {
                    IsMultiThreadProtectionOn = true,
                    IsPausableOn = true,
                    IsStatisticsOn = false
                };
        }

        public static CacheDecoratorOptions All
        {
            get
            {
                return new CacheDecoratorOptions
                    {
                        IsMultiThreadProtectionOn = true,
                        IsPausableOn = true,
                        IsStatisticsOn = true
                    };
            }
        }

        /// <summary>
        /// The default extensions with which to decorate an instance of a <see cref="ICache"/>
        /// </summary>
        public static CacheDecoratorOptions Default
        {
            get { return _defaultInstance.Clone(); }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _defaultInstance = value;
            }
        }

        public static CacheDecoratorOptions None
        {
            get { return new CacheDecoratorOptions(); }
        }
    }
}