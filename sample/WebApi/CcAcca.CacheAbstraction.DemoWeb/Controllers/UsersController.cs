using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CcAcca.CacheAbstraction.DemoWeb.Models;
using CcAcca.CacheAbstraction.DemoWeb.Repositories;

namespace CcAcca.CacheAbstraction.DemoWeb.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly UserRepository _repository;
        private readonly ICache _cache;

        public UsersController() : this(new UserRepository(), GlobalCacheProvider.DefaultInstance)
        {
        }

        public UsersController(UserRepository repository, ICacheProvider cacheProvider)
        {
            _repository = repository;
            _cache = cacheProvider.Get<ICache>("UsersCache");
        }

        [Route("")]
        [ResponseType(typeof(ICollection<UserBrief>))]
        public async Task<IHttpActionResult> Get()
        {
            ICollection<UserBrief> users = await _cache.GetOrAdd("GetAllAsync", k => _repository.GetAllAsync());
            return Ok(users);
        }

        [Route("{name}")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetOneByName(string name)
        {
            string cacheKey = string.Format("GetOneByNameAsync-{0}", name);
            User user = await _cache.GetOrAdd(cacheKey, k => _repository.GetOneByNameAsync(name));
            return Ok(user);
        }
    }
}