namespace CcAcca.CacheAbstraction.WebApi
{
    public static class CacheWebApiConfig
    {
        static CacheWebApiConfig()
        {
            UrlPrefix = "api/caches";
        }

        /// <summary>
        /// The base url to access cache representations defaulting to 'api/caches'
        /// </summary>
        public static string UrlPrefix { get; set; }
    }
}