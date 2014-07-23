using CcAcca.CacheAbstraction.WebApi;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof($rootnamespace$.App_Start.CacheAbstraction), "ConfigureWebApi")]

namespace $rootnamespace$.App_Start
{
    public static class CacheAbstraction
    {
        public static void ConfigureWebApi()
        {
            CacheWebApiConfig.UrlPrefix = "api/caches";
        }
    }
}