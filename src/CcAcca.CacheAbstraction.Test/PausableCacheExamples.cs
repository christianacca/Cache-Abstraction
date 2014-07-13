// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Generic;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class PausableCacheExamples : CacheExamplesBase
    {
        #region Test helpers

        public new IPausableCache Cache
        {
            get { return (IPausableCache) base.Cache; }
        }


        protected override ICache CreateCache()
        {
            return new PausableDecorator(new ObjectCacheWrapper());
        }

        #endregion


        [Test]
        public void As_ShouldReturnDecoratorInstance()
        {
            var instance = Cache.As<IPausableCache>();
            Assert.That(instance, Is.Not.Null, "instance");
            Assert.IsInstanceOf<PausableDecorator>(instance, "wrong implementation");
        }

        [Test]
        public void As_ShouldReturnDecoratorInstance_DecoratorsChained()
        {
            var cache = new StatisticsDecorator(new List<CacheStatistic>(), Cache);
            var instance = cache.As<IPausableCache>();
            Assert.That(instance, Is.Not.Null, "instance");
            Assert.IsInstanceOf<PausableDecorator>(instance, "wrong implementation");
        }

        [Test]
        public void WhenDisabled_ExistingCachedItems_ShouldNotBeFlushed()
        {
            //given
            Cache.GetOrAdd("someKey", _ => new object());

            //when
            Cache.IsPaused = false;

            //then
            Assert.That(Cache.Count, Is.EqualTo(1));
        }


        [Test]
        public void WhenPaused_Contains_ShouldAlwaysReturnFalse()
        {
            //given
            Cache.GetOrAdd("someKey", _ => new object());

            //when
            Cache.IsPaused = true;

            //then
            Assert.That(Cache.Contains("someKey"), Is.False);
        }


        [Test]
        public void WhenPaused_GetOrAdd_ShouldNotReturnPreviouslyCachedItems()
        {
            //given
            object cachedItem = Cache.GetOrAdd("someKey", _ => new object());
            Cache.IsPaused = true;

            //when
            object actual = Cache.GetOrAdd("someKey", _ => new object());

            //then
            Assert.That(actual, Is.Not.SameAs(cachedItem));
        }


        [Test]
        public void WhenPaused_GetOrAdd_WillExecuteConstructorDelegate_EvenWhenItemExistsInCache()
        {
            //given
            Cache.GetOrAdd("someKey", _ => new object());
            Cache.IsPaused = true;

            //when
            bool constructorExecuted = false;
            Func<string, object> constructor = delegate {
                constructorExecuted = true;
                return new object();
            };
            Cache.GetOrAdd("someKey", constructor);

            //then
            Assert.That(constructorExecuted, Is.True);
        }


        [Test]
        public void WhenPaused_AddOrUpdateShouldDoNothing()
        {
            //given
            Cache.IsPaused = true;

            //when
            Cache.AddOrUpdate("someKey", new object());

            //then
            Assert.That(Cache.Contains("someKey"), Is.False);
        }


        [Test]
        public void WhenDisabled_GetData_ShouldNotReturnCachedItems()
        {
            //given
            Cache.GetOrAdd("someKey", _ => new object());
            Cache.IsPaused = false;

            //when, then
            var item = Cache.GetData<object>("somekey");
            Assert.That(item, Is.Null);
        }


        [Test]
        public void WhenRestarted_ExistingCachedItems_ShouldBeReturned()
        {
            //given
            object cachedItem = Cache.GetOrAdd("someKey", _ => new object());
            Cache.IsPaused = true;

            //when
            Cache.IsPaused = false;

            //then
            object itemFromCache = Cache.GetOrAdd("someKey", _ => new object());
            Assert.That(itemFromCache, Is.SameAs(cachedItem));
        }
    }
}