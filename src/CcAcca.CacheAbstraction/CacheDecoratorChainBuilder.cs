// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// The default cache decorator chain builder
    /// </summary>
    public class CacheDecoratorChainBuilder
    {
        /// <summary>
        /// Decorate <paramref name="decorated"/> instance with the extended behaviours specified in <paramref name="options"/>
        /// </summary>
        public virtual ICache AddDecorators<T>(ICache decorated, T options) where T : CacheDecoratorOptions
        {
            // order of chain should be IMultiThreadProtectedCache -> 0..* -> IPausableCache -> 0..* -> IStatisticsCache
            // rationale:
            // IMultiThreadProtectedCache must be before IPausableCache otherwise pausing cache will have no effect
            // IStatisticsCache must be after IPausableCache otherwise stats will be recorded even if the cache is paused

            if (options.IsStatisticsOn && !decorated.IsDecoratedWith<IStatisticsCache>())
            {
                decorated = InsertDecoratorAtStartOfChain(decorated,
                                                          cache => new StatisticsDecorator(options.Statistics, cache));
            }
            if (options.IsPausableOn && !decorated.IsDecoratedWith<IPausableCache>())
            {
                decorated = InsertDecoratorBetween<IMultiThreadProtectedCache, IStatisticsCache>(
                    decorated,
                    cache => new PausableDecorator(cache));
            }
            if (options.IsMultiThreadProtectionOn && !decorated.IsDecoratedWith<IMultiThreadProtectedCache>())
            {
                decorated = new MultiThreadProtectedDecorator(decorated);
            }

            return decorated;
        }


        /// <summary>
        /// Inserts a new cache decorator at the start of the chain of decorators terminating at the
        /// <paramref name="decorated"/> instance supplied
        /// </summary>
        /// <param name="decorated">The existing cache to insert a new cache decorator into it's chain</param>
        /// <param name="createDecoration">
        /// A delegate that takes the cache at the head of the chain and creates the decorator to be inserted
        /// </param>
        public virtual ICache InsertDecoratorAtStartOfChain(ICache decorated,
                                                            Func<ICache, CacheDecorator> createDecoration)
        {
            var cacheDecorator = (CacheDecorator) decorated.GetDecoratorChain().LastOrDefault();
            if (cacheDecorator == null)
            {
                return createDecoration(decorated);
            }

            // we need to insert statistics decorator as the first decorator in the chain
            // this means redefining the existing chain...
            // the reason why we need to do this is a decorator at the end of the chain can be by-passed using the 
            // ICache.As method.
            // By adding the statistics cache at the start of the chain we ensure that it will always be called

            // find the last decorator in the chain
            CacheDecorator insert = createDecoration(cacheDecorator.DecoratedCache);
            cacheDecorator.DecoratedCache = insert;
            return decorated;
        }

        public virtual ICache InsertDecoratorBetween<TStart, TEnd>(ICache decorated,
                                                                   Func<ICache, CacheDecorator> createDecoration)
            where TEnd : class, ICache where TStart : class, ICache
        {
            List<CacheDecorator> chain = decorated.GetDecoratorChain().OfType<CacheDecorator>().ToList();

            var end = chain.CacheOfType<TEnd>().LastOrDefault() as CacheDecorator;
            var start = chain.CacheOfType<TStart>().LastOrDefault() as CacheDecorator;

            if (!ReferenceEquals(end, null))
            {
                int endPosition = chain.IndexOf(end);
                if (endPosition == 0)
                {
                    // create a new head for the existing chain
                    return createDecoration(end);
                }
                else
                {
                    // insert new decorator into exising chain
                    CacheDecorator insert = createDecoration(end);
                    CacheDecorator beforeEnd = chain[endPosition - 1];
                    beforeEnd.DecoratedCache = insert;
                    return decorated;
                }
            }
            else if (!ReferenceEquals(start, null))
            {
                CacheDecorator insert = createDecoration(start.DecoratedCache);
                start.DecoratedCache = insert;
                return decorated;
            }
            else
            {
                // create a chain with our new decorator
                return createDecoration(decorated);
            }
        }
    }
}