using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace CcAcca.CacheAbstraction.WebApi
{
    /// <summary>
    /// Implements the http endpoint to administer caches registered with <see cref="CacheAdministator"/>
    /// </summary>
    /// <remarks>
    /// The controller by default will administer caches that have been registered with 
    /// <see cref="CacheAdministator.DefaultInstance"/> of the <see cref="CacheAdministator"/>
    /// </remarks>
    [CacheRoutePrefix]
    public class CacheController : ApiController
    {
        private readonly CacheAdministator _cacheAdministator;

        public CacheController() : this(CacheAdministator.DefaultInstance) {}

        public CacheController(CacheAdministator cacheAdministator)
        {
            _cacheAdministator = cacheAdministator;
        }

        [Route("{id}/{itemKey}")]
        public IHttpActionResult DeleteItem(string id, string itemKey)
        {
            try
            {
                var isSuccess = _cacheAdministator.RemoveItem(id, itemKey);
                if (isSuccess)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (ArgumentException e)
            {
                ModelState.AddModelError(e.ParamName, e.Message);
                return BadRequest(ModelState);
            }

        }

        [Route("")]
        public IEnumerable<CacheInfo> Get(string expand = null)
        {
            var infos = _cacheAdministator.AllDefaultCacheInfos.ApplyExpand(expand).ToList();
            return infos;
        }

        [Route("{id}")]
        public IHttpActionResult GetOne(string id, string expand = null)
        {
            var match = 
                _cacheAdministator.AllDefaultCacheInfos.Where(c => c.CacheId == id).ApplyExpand(expand).FirstOrDefault();
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        [Route("{id}")]
        public IHttpActionResult Patch(string id, [FromBody] dynamic cacheUpdate)
        {
            foreach (JProperty prop in cacheUpdate)
            {
                if (String.Equals(prop.Name, "IsPaused", StringComparison.OrdinalIgnoreCase))
                {
                    return StartOrPauseCache(id, prop.Value.ToObject<bool>());
                }
            }
            
            ModelState.AddModelError("", "No updateable cache properties supplied");
            return BadRequest(ModelState);
        }

        private IHttpActionResult StartOrPauseCache(CacheIdentity cacheId, bool isPaused)
        {
            Func<CacheIdentity, bool> action = isPaused
                                                   ? (Func<CacheIdentity, bool>) _cacheAdministator.PauseCache
                                                   : _cacheAdministator.StartCache;
            bool isSuccess = action(cacheId);
            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}