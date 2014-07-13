using System;
using System.Threading;

namespace CcAcca.CacheAbstraction.Statistics
{
    public class CacheItemAccessInfo
    {
        private int _readCount;

        public string Key { get; set; }
        public int ReadCount
        {
            get { return _readCount; }
            set { _readCount = value; }
        }

        public DateTimeOffset? LastWrite { get; set; }
        public DateTimeOffset? LastRead { get; set; }

        public void IncrementReadCount()
        {
            Interlocked.Increment(ref _readCount);
        }
    }
}