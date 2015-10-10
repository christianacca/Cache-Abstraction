// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Generic;
using System.Linq;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class CacheDecoratorChainBuilderExamples
    {
        private CacheDecoratorChainBuilder _builder;
        private ICache _cacheImpl;

        [SetUp]
        public void SetUp()
        {
            _builder = new CacheDecoratorChainBuilder();
            _cacheImpl = new SimpleInmemoryCache();
        }


        [Test]
        public void CanBuildWithMultiThreadProtectedDecorator()
        {
            ICache cache = _builder.AddDecorators(_cacheImpl,
                                                  new CacheDecoratorOptions {IsMultiThreadProtectionOn = true});
            Assert.That(cache.As<IMultiThreadProtectedCache>(), Is.Not.Null);
        }

        [Test]
        public void CanBuildWithPausableDecorator()
        {
            ICache cache = _builder.AddDecorators(_cacheImpl, new CacheDecoratorOptions {IsPausableOn = true});
            Assert.That(cache.As<IPausableCache>(), Is.Not.Null);
        }

        [Test]
        public void CanBuildWithStatisticsDecorator()
        {
            ICache cache = _builder.AddDecorators(_cacheImpl, new CacheDecoratorOptions {IsStatisticsOn = true});
            Assert.That(cache.As<IStatisticsCache>(), Is.Not.Null);
        }


        [Test]
        public void CanBuildWithAllDecorators()
        {
            ICache cache = _builder.AddDecorators(_cacheImpl, CacheDecoratorOptions.All);
            Assert.That(cache.As<IPausableCache>(), Is.Not.Null);
            Assert.That(cache.As<IStatisticsCache>(), Is.Not.Null);
            Assert.That(cache.As<IMultiThreadProtectedCache>(), Is.Not.Null);
        }

        [Test]
        public void CanBuildWithDefaulDecorators()
        {
            ICache cache = _builder.AddDecorators(_cacheImpl, CacheDecoratorOptions.Default);
            Assert.That(cache.As<IPausableCache>(), Is.Not.Null);
            Assert.That(cache.As<IStatisticsCache>(), Is.Null);
            Assert.That(cache.As<IMultiThreadProtectedCache>(), Is.Null);
        }

        [Test]
        public void ShouldNotAddDuplicateDecorator()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl, CacheDecoratorOptions.All);

            // when
            cache = _builder.AddDecorators(cache, CacheDecoratorOptions.All);

            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            Assert.That(chain.Count(), Is.EqualTo(3));
            Assert.That(chain.CacheOfType<IPausableCache>().Count(), Is.EqualTo(1));
            Assert.That(chain.CacheOfType<IStatisticsCache>().Count(), Is.EqualTo(1));
            Assert.That(chain.CacheOfType<IMultiThreadProtectedCache>().Count(), Is.EqualTo(1));
        }


        [Test]
        public void OrderOfDecoratorsImportant()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl, CacheDecoratorOptions.All);

            // when
            cache = _builder.AddDecorators(cache, CacheDecoratorOptions.All);

            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            // order should be IMultiThreadProtectedCache -> 0..* -> IPausableCache -> 0..* -> IStatisticsCache
            // rationale:
            // IMultiThreadProtectedCache must be before IPausableCache otherwise pausing cache will have no effect
            // IStatisticsCache must be after IPausableCache otherwise stats will be recorded even if the cache is paused
            Assert.That(chain.ElementAt(0), Is.InstanceOf<IMultiThreadProtectedCache>());
            Assert.That(chain.ElementAt(1), Is.InstanceOf<IPausableCache>());
            Assert.That(chain.ElementAt(2), Is.InstanceOf<IStatisticsCache>());
        }


        [Test]
        public void ShouldInsertStatisticsDecoratorAtStartOfExistingChain()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl,
                                                  new CacheDecoratorOptions
                                                      {
                                                          IsPausableOn = true,
                                                          IsMultiThreadProtectionOn = true
                                                      });

            // when
            cache = _builder.AddDecorators(cache, new CacheDecoratorOptions {IsStatisticsOn = true});

            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            Assert.That(chain.ElementAt(0), Is.InstanceOf<IMultiThreadProtectedCache>());
            Assert.That(chain.ElementAt(1), Is.InstanceOf<IPausableCache>());
            Assert.That(chain.ElementAt(2), Is.InstanceOf<IStatisticsCache>());
        }


        [Test]
        public void ShouldInsertPausableDecoratorImmediatelyAfterStatisticsDecorator()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl,
                                                  new CacheDecoratorOptions
                                                      {
                                                          IsStatisticsOn = true,
                                                          IsMultiThreadProtectionOn = true
                                                      });

            // when
            cache = _builder.AddDecorators(cache, new CacheDecoratorOptions {IsPausableOn = true});

            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            Assert.That(chain.ElementAt(0), Is.InstanceOf<IMultiThreadProtectedCache>());
            Assert.That(chain.ElementAt(1), Is.InstanceOf<IPausableCache>());
            Assert.That(chain.ElementAt(2), Is.InstanceOf<IStatisticsCache>());
        }


        [Test]
        public void ShouldInsertPausableDecoratorImmediatelyAfterMutliThreadProtectionDecorator()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl,
                                                  new CacheDecoratorOptions
                                                      {
                                                          IsMultiThreadProtectionOn = true
                                                      });

            // when
            cache = _builder.AddDecorators(cache, new CacheDecoratorOptions {IsPausableOn = true});

            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            Assert.That(chain.ElementAt(0), Is.InstanceOf<IMultiThreadProtectedCache>());
            Assert.That(chain.ElementAt(1), Is.InstanceOf<IPausableCache>());
        }

        [Test]
        public void RemoveDecorators_ShouldReturnCacheMinusDecorators()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl,
                                                  new CacheDecoratorOptions
                                                      {
                                                          IsMultiThreadProtectionOn = true,
                                                          IsStatisticsOn = true,
                                                          IsPausableOn = true
                                                      });

            // when
            cache = _builder.RemoveDecorators(cache,
                                              new CacheDecoratorOptions {IsStatisticsOn = false, IsPausableOn = false});

            // then
            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            Assert.That(chain.Count(), Is.EqualTo(1), "Decorators");
            Assert.That(chain.ElementAt(0), Is.InstanceOf<IMultiThreadProtectedCache>(), "MutliThreadPotected");
        }

        [Test]
        public void RemoveDecorators_ShouldSafelyIgnoreMissingDecorators()
        {
            // given
            ICache cache = _builder.AddDecorators(_cacheImpl, new CacheDecoratorOptions { IsMultiThreadProtectionOn = true });

            // when
            cache = _builder.RemoveDecorators(cache, new CacheDecoratorOptions { IsStatisticsOn = false });

            // then
            IEnumerable<ICache> chain = cache.GetDecoratorChain().ToList();

            Assert.That(chain.Count(), Is.EqualTo(1), "Decorators");
            Assert.That(chain.ElementAt(0), Is.InstanceOf<IMultiThreadProtectedCache>(), "MutliThreadPotected");
        }
    }
}