// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    /// <summary>
    /// Defines and asserts the detailed behaviour / contract that a <see cref="ICache"/> implementation is
    /// expected to implement
    /// </summary>
    /// <remarks>
    /// When writing a new <see cref="ICache"/> implementation, you should create a test class that inherits from 
    /// this class. Doing so ensures that your <see cref="ICache"/> implementation is checked for compliance to 
    /// the <see cref="ICache"/> contract
    /// </remarks>
    public abstract class CacheExamplesBase
    {
        #region Member Variables

        private ICache _cache;

        #endregion


        #region Properties

        public ICache Cache
        {
            get { return _cache; }
        }

        #endregion


        #region Setup / Teardown

        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCache();
        }

        [TearDown]
        public void TestCleanup()
        {
            _cache = null;
        }

        #endregion


        [Test]
        public void AddOrUpdate_Should_Add()
        {
            Cache.AddOrUpdate("key", 5);

            Assert.That(Cache.Contains("key"), Is.True);
        }


        [Test]
        public void AddOrUpdate_ShouldUpdateExistingItem()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate("key", 6);

            Assert.That(Cache.GetData<int>("key"), Is.EqualTo(6));
        }


        [Test]
        public void AddOrUpdate_ShouldUpdateExistingItem_NullValueSupplied()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate<object>("key", null);

            Assert.That(Cache.GetData<object>("key"), Is.Null);
        }


        [Test]
        public void AddOrUpdate_ShouldNotIgnoreAttemptsToAddNullItem()
        {
            Cache.AddOrUpdate<object>("whatever", null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }


        protected abstract ICache CreateCache();


        [Test]
        public void GetData_ShouldReturnDefaultValueIfItemMissing()
        {
            Assert.That(Cache.GetData<object>("missingkey"), Is.Null);
        }

        [Test]
        public void GetOrAdd_ShouldNotIgnoreAttemptsToAddNullItem()
        {
            Cache.GetOrAdd<object>("whatever", _ => null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }


        [Test]
        public void GetOrAdd_ShouldReturnExistingCachedItem()
        {
            //given
            object cachedItem = Cache.GetOrAdd("someKey", _ => new object());

            //when
            object itemFromCache = Cache.GetOrAdd("someKey", _ => new object());

            //then
            Assert.That(cachedItem, Is.SameAs(itemFromCache), "previously cached item not returned");
            Assert.That(Cache.Count, Is.EqualTo(1), "cache should not have changed");
        }


        [Test]
        public void GetOrAdd_WhereItemAlreadyExistsInCache_ShouldNotExecuteConstructorDelegate()
        {
            //given
            Cache.GetOrAdd("someKey", _ => new object());

            //when
            bool constructorExecuted = false;
            Func<string, object> constructor = delegate {
                constructorExecuted = true;
                return new object();
            };
            Cache.GetOrAdd("someKey", constructor);

            //then
            Assert.That(constructorExecuted, Is.False);
        }


        [Test]
        public void GetOrAdd_WhereItemNotAlreadyCached_ShouldAddItemToCache()
        {
            Func<string, object> constructor = _ => new object();
            object cachedItem = Cache.GetOrAdd("someKey", constructor);

            //then
            Assert.That(Cache.Count, Is.EqualTo(1));
            Assert.That(cachedItem, Is.Not.Null);
        }


        [Test]
        public void GetOrAdd_WhereItemNotAlreadyCached_UnderRaceCondition_ShouldReturnFirstValueAdded()
        {
            Func<string, Task<int>> slowCtor = async _ => {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                return 1;
            };
            Func<string, Task<int>> fastCtor = async _ => {
                await Task.Delay(TimeSpan.FromMilliseconds(5));
                return 2;
            };
            Task<int> t1 = Task.Run(() => Cache.GetOrAdd("Key", slowCtor));
            Task<int> t2 = Task.Run(() => Cache.GetOrAdd("Key", fastCtor));
            Task<int[]> ts = Task.WhenAll(t1, t2);
            ts.ContinueWith(task => Assert.That(task.Result, Is.EqualTo(new[] { 2, 2})));
        }


        [Test]
        public void Remove_ShouldSilentlyIgnoreMissingItem()
        {
            Assert.DoesNotThrow(() => Cache.Remove("missingkey"));
        }


        [Test]
        public void WhenItemExpiresFromCache_GetOrAdd_WillAddNewItemToCache()
        {
            //given
            object expiredItem = Cache.GetOrAdd("someKey", _ => new object());
            Cache.Flush(); //simulate item expiring from the cache

            //when, then
            object newItem = Cache.GetOrAdd("someKey", _ => new object());
            Assert.That(newItem, Is.Not.SameAs(expiredItem), "expired item still in cache");

            object newItemInCache = Cache.GetOrAdd("someKey", _ => new object());
            Assert.That(newItemInCache, Is.SameAs(newItem), "new item was not added to cache");
        }
    }
}