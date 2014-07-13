Notes:
1. Caching folder contains various caching abstractions: 
   - an ICache interface that hides the actual implementation of a cache
   - the simplest possible implementation of the ICache interface that uses internally a ConcurrentDictionary as a backing store (SimpleInmemoryCache)
   - a wrapper class for the core .net ObjectCache class that adapts it to the ICache interface
   - a cache decorator class that acts as a simple extensibility point for other developers
   - cache administrator functionality 
2. For more information:
   a. See Overview diagram: CachingOverview.cd
   b. See the tests/examples in Ram.Series5.Tests.Caching
   c. See the .net class documentation for ObjectCache and MemoryCache
