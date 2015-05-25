// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class NullCacheExamples
    {
        [Test]
        public void As_ShouldReturnReferenceToINullCache()
        {
            var cache = NullCache.Instance;
            Assert.That(cache.As<INullCache>(), Is.SameAs(cache));
        }
    }
}