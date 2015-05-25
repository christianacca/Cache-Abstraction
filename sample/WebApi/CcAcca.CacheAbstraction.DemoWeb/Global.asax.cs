using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using CcAcca.CacheAbstraction.DemoWeb.Repositories;

namespace CcAcca.CacheAbstraction.DemoWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            UserRepository.TestUserJsonFile = Server.MapPath("~/App_Data/TestUsers.txt");
        }
    }
}
