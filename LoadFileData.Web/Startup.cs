using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LoadFileData.Web.Startup))]

namespace LoadFileData.Web
{
    public class RegObject : IRegisteredObject
    {
        public void Stop(bool immediate)
        {
            
        }
    }
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var host = HostingEnvironment.ApplicationHost;
            var manager = ApplicationManager.GetApplicationManager();
            manager.CreateObject(host, typeof (RegObject));

            ConfigureAuth(app);
        }
    }
}
