using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using LoadFileData.FileHandlers;
using LoadFileData.Web;
using LoadFileData.Web.Constants;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Microsoft.Practices.Unity;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LoadFileData.Web
{
    public class Startup
    {
        private static IEnumerable<HandlerMonitors> handlers;

        public struct HandlerMonitors
        {
            public FolderMonitor Monitor { get; set; }
            public IRecoverWorkflow Handler { get; set; }
        }

        public static IEnumerable<HandlerMonitors> StartUpMonitors(IFileHandlerSettingsFactory factory)
        {
            var watchFolderTemplate = ConfigurationManager.AppSettings[Folders.WatchBase];
            foreach (var handlerSetting in factory.CreateFileHandlers())
            {

                var fileHandler = new FileHandler(handlerSetting);
                var watchFolder = string.Format(watchFolderTemplate, handlerSetting.Name);
                if (!Directory.Exists(watchFolder))
                {
                    Directory.CreateDirectory(watchFolder);
                }
                yield return new HandlerMonitors
                {
                    Handler = fileHandler,
                    Monitor = new FolderMonitor(watchFolder, fileHandler)
                };
            }
        }

        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfig.Container;
            handlers = StartUpMonitors(container.Resolve<IFileHandlerSettingsFactory>());
            if (HostingEnvironment.IsHosted)
            {
                foreach (var handler in handlers)
                {
                    HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>) handler.Handler.RecoverExistingFiles);
                    HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>) handler.Monitor.StartMonitoring);
                }
                return;
            }
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            foreach (var handler in handlers)
            {
                var handlerMonitors = handler;
                Task.Run(() => handlerMonitors.Handler.RecoverExistingFiles(token), token);
                Task.Run(() => handlerMonitors.Monitor.StartMonitoring(token), token);
            }
        }
    }
}
