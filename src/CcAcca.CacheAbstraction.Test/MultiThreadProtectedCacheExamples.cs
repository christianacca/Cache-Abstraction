// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class MultiThreadProtectedCacheExamples : CacheExamplesBase
    {
        protected override ICache CreateCache()
        {
            return new MultiThreadProtectedDecorator(new SimpleInmemoryCache());
        }

        [Test]
        public void As_ShouldReturnDecoratorInstance()
        {
            var instance = Cache.As<IMultiThreadProtectedCache>();
            Assert.That(instance, Is.Not.Null, "instance");
            Assert.IsInstanceOf<MultiThreadProtectedDecorator>(instance, "wrong implementation");
        }

        [Test]
        public void As_ShouldReturnDecoratorInstance_DecoratorsChained()
        {
            var cache = new StatisticsDecorator(new List<CacheStatistic>(), Cache);
            var instance = cache.As<IMultiThreadProtectedCache>();
            Assert.That(instance, Is.Not.Null, "instance");
            Assert.IsInstanceOf<MultiThreadProtectedDecorator>(instance, "wrong implementation");
        }

        [Test]
        public void Demo_SimpleInmemoryCache_within_single_thread_constructor_called_only_once()
        {
            var cache = new SimpleInmemoryCache();
            int ctorCallCount = 0;
            Func<string, string> ctor = _ => {
                ctorCallCount++;
                return string.Format("Hello World {0}", ctorCallCount);
            };
            cache.GetOrAdd("Key", ctor);
            cache.GetOrAdd("Key", ctor);

            Assert.That(ctorCallCount, Is.EqualTo(1));
        }

        [Test]
        public void Demo_SimpleInmemoryCache_two_threads_receiving_cache_misses_will_cause_constructor_to_be_run_twice()
        {
            var cache = new SimpleInmemoryCache();
            int ctorCallCount = 0;
            Func<string, string> ctor = _ => {
                Interlocked.Increment(ref ctorCallCount);
                return string.Format("Hello World {0}", ctorCallCount);
            };
            Func<Task<string>> fn = async () => {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return cache.GetOrAdd("Key", ctor);
            };
            Task<string> t1 = Task.Run(fn);
            Task<string> t2 = Task.Run(fn);
            Task<string[]> ts = Task.WhenAll(new[] {t1, t2});
            ts.ContinueWith(task => Assert.That(ctorCallCount, Is.EqualTo(2)));
        }

        [Test]
        public void Demo_ConcurrentDictionary_multi_threaded_cache_miss_also_a_problem()
        {
            var cache = new ConcurrentDictionary<string, string>();
            int ctorCallCount = 0;
            Func<string, string> ctor = key => {
                Interlocked.Increment(ref ctorCallCount);
                return string.Format("Hello World {0}", ctorCallCount);
            };
            Func<Task<string>> fn = async () => {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return cache.GetOrAdd("Key", ctor);
            };
            Task<string> t1 = Task.Run(fn);
            Task<string> t2 = Task.Run(fn);
            Task<string[]> ts = Task.WhenAll(new[] {t1, t2});
            ts.ContinueWith(task => Assert.That(ctorCallCount, Is.EqualTo(2)));
        }

        [Test]
        public void GetOrAdd_protected_from_running_twice_in_multi_threaded_code()
        {
            var cache = new MultiThreadProtectedDecorator(new SimpleInmemoryCache());
            int ctorCallCount = 0;
            Func<string, string> ctor = _ => {
                Interlocked.Increment(ref ctorCallCount);
                return string.Format("Hello World {0}", ctorCallCount);
            };
            Func<Task<string>> fn = async () => {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return cache.GetOrAdd("Key", ctor);
            };
            Task<string> t1 = Task.Run(fn);
            Task<string> t2 = Task.Run(fn);
            Task<string[]> ts = Task.WhenAll(new[] {t1, t2});
            ts.ContinueWith(task => Assert.That(ctorCallCount, Is.EqualTo(1)));
        }


        [Test]
        public void GetOrAdd_ExtentionMethod_protected_from_running_twice_in_multi_threaded_code()
        {
            var cache = new MultiThreadProtectedDecorator(new SimpleInmemoryCache());
            int ctorCallCount = 0;
            Func<string, string> ctor = _ => {
                Interlocked.Increment(ref ctorCallCount);
                return string.Format("Hello World {0}", ctorCallCount);
            };
            Func<Task<string>> fn = async () => {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return CacheExtensions.GetOrAdd(cache, "Key", ctor);
            };
            Task<string> t1 = Task.Run(fn);
            Task<string> t2 = Task.Run(fn);
            Task<string[]> ts = Task.WhenAll(new[] {t1, t2});
            ts.ContinueWith(task => Assert.That(ctorCallCount, Is.EqualTo(1)));
        }
    }
}