using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web.Hosting;
using LoadFileData.Monitors;
using LoadFileData.Web;
using LoadFileData.Web.Constants;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace LoadFileData.Web
{
    public class Startup
    {
        private static IList<FolderMonitor> monitors = new List<FolderMonitor>();

        public void Configuration(IAppBuilder app)
        {
            var handlerFactory = new FileHandlerFactory(new StreamManager());
            var watchBase = ConfigurationManager.AppSettings[Folders.WatchBase];
            foreach (var handlerPair in handlerFactory.CreateFileHandlers())
            {
                var watchFolder = string.Format(watchBase, handlerPair.Key);
                var monitor = new FolderMonitor(watchFolder, handlerPair.Value);
                HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)monitor.StartMonitoring);
                monitors.Add(monitor);
            }

        }
    }
}
