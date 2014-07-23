using System;
using System.Web.Http;

namespace CcAcca.CacheAbstraction.WebApi
{
    /// <summary>
    /// Assigns the <see cref="RoutePrefixAttribute.Prefix"/> to the api controllers in this assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class CacheRoutePrefixAttribute : RoutePrefixAttribute
    {
        public CacheRoutePrefixAttribute() : base(CacheWebApiConfig.UrlPrefix) { }
    }
}