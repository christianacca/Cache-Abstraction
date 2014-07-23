Notes:
1. CcAcca.CacheAbstraction package contains various caching abstractions: 
   - an ICache interface that hides the actual implementation of a cache
   - the simplest possible implementation of the ICache interface that uses internally a ConcurrentDictionary as a backing store (SimpleInmemoryCache)
   - a wrapper class for the core .net ObjectCache class that adapts it to the ICache interface
   - a cache decorator class that acts as a simple extensibility point for other developers
   - cache administrator functionality 
2. For more information:
   a. See project home page: https://github.com/christianacca/Cache-Abstraction
   b. See the tests/examples in src/CcAcca.CacheAbstraction.Test
	  (https://github.com/christianacca/Cache-Abstraction/tree/master/src/CcAcca.CacheAbstraction.Test)
   c. See the .net class documentation for ObjectCache and MemoryCache
