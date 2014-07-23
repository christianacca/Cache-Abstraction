using System;
using System.Collections.Generic;
using System.Linq;
using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction.WebApi
{
    public static class CacheInfoExts
    {
        /// <summary>
        /// Determines which navigation property(s) of <see cref="CacheInfo"/> should be returned to the client
        /// expanded with the associated representations
        /// </summary>
        /// <param name="source">The items whose navigation properties are to be expanded</param>
        /// <param name="expandClause">A comma seperated string of the navigation properties to expand</param>
        public static IEnumerable<CacheInfo> ApplyExpand(this IEnumerable<CacheInfo> source, string expandClause)
        {
            source = source ?? Enumerable.Empty<CacheInfo>();
            bool stripItems = String.IsNullOrWhiteSpace(expandClause) || 
                !expandClause.ToLower().Contains("itemaccessstatistics");
            foreach (CacheInfo cacheInfo in source)
            {
                if (stripItems)
                {
                    cacheInfo.ItemAccessStatistics = new List<CacheItemAccessInfo>();
                }
                yield return cacheInfo;
            }
        }
    }
}