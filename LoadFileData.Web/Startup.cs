using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using LoadFileData.FileHandlers;
using LoadFileData.Web;
using LoadFileData.Web.Constants;
using Microsoft.Owin;
using Microsoft.Practices.Unity;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace LoadFileData.Web
{
    public class Startup
    {
        public static void StartUpMonitors(IFileHandlerSettingsFactory factory)
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
                var monitor = new FolderMonitor(watchFolder, fileHandler);
                HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)fileHandler.RecoverExistingFiles);
                HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)monitor.StartMonitoring);
            }
        }

        public void Configuration(IAppBuilder app)
        {

            IUnityContainer container = new UnityContainer();
            StartUpMonitors(container.Resolve<IFileHandlerSettingsFactory>());
        }
    }
}
