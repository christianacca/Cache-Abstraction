// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE
namespace CcAcca.CacheAbstraction
{
    public class CacheItem<T>
    {
        public CacheItem(T value)
        {
            Value = value;
        }

        public T Value { get; private set; } 
    }
}