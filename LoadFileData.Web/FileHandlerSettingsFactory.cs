using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using LoadFileData.DAL;
using LoadFileData.FileHandlers;
using LoadFileData.Web.Constants;

namespace LoadFileData.Web
{
    public class FileHandlerSettingsFactory : IFileHandlerSettingsFactory
    {
        private readonly IStreamManager streamManager;
        private readonly IDataService service;
        private readonly IContentReaderFactory readerFactory;
        private readonly IContentHandlerFactory handlerFactory;

        public FileHandlerSettingsFactory(
            IStreamManager streamManager,
            IServiceFactory serviceFactory,
            IContentReaderFactory readerFactory,
            IContentHandlerFactory handlerFactory)
        {
            this.streamManager = streamManager;
            service = serviceFactory.Create();
            this.readerFactory = readerFactory;
            this.handlerFactory = handlerFactory;
        }


        #region Implementation of IFileHandlerFactory

        public virtual IEnumerable<FileHandlerSettings> CreateFileHandlers()
        {
            var copyToTemplate = ConfigurationManager.AppSettings[Folders.CopyTo];
            if (string.IsNullOrEmpty(copyToTemplate))
            {
                throw new SettingsPropertyWrongTypeException(
                    string.Format("AppSetting {0} must have a valid path template", Folders.CopyTo));
            }


            return from setting in ReadJsonSettings()
                   let name = Path.GetFileNameWithoutExtension(setting.Key)
                   let contentReader = readerFactory.Create(setting.Value)
                   let contentHandler = handlerFactory.Create(setting.Value)
                   let fileHandlerSettings = new FileHandlerSettings
                   {
                       DestinationPathTemplate = copyToTemplate,
                       Service = service,
                       StreamManager = streamManager,
                       Name = name,
                       ContentHandler = contentHandler,
                       Reader = contentReader
                   }
                   where !string.IsNullOrEmpty(name)
                   select fileHandlerSettings;
        }

        protected virtual IDictionary<string, string> ReadJsonSettings()
        {
            return StreamManager.AppSettingsFiles(Folders.WatchDefinitons);
        }

        #endregion
    }
}