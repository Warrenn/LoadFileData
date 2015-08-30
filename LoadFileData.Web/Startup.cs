using System;
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
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif
            app.UseWelcomePage("/");

            var container = UnityConfig.Container;
            var factory = container.Resolve<IFileHandlerSettingsFactory>();
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            var watchFolderTemplate = ConfigurationManager.AppSettings[Folders.WatchBase];
            foreach (var handlerSetting in factory.CreateFileHandlers())
            {
                var fileHandler = new FileHandler(handlerSetting);
                var watchFolder = string.Format(watchFolderTemplate, handlerSetting.Name);
                if (!Directory.Exists(watchFolder))
                {
                    Directory.CreateDirectory(watchFolder);
                }
                var monitor = new FolderMonitor(watchFolder, fileHandler);
                if (HostingEnvironment.IsHosted)
                {
                    HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)fileHandler.RecoverExistingFiles);
                    HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)monitor.StartMonitoring);
                    continue;
                }
                Task.Run(() => fileHandler.RecoverExistingFiles(token), token);
                Task.Run(() => monitor.StartMonitoring(token), token);
            }
        }
    }
}
