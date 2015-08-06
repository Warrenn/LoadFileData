using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentReaders;
using LoadFileData.DAL;
using LoadFileData.FileHandlers;
using LoadFileData.Web.Constants;
using Newtonsoft.Json;

namespace LoadFileData.Web
{
    public class FileHandlerSettingsFactory : IFileHandlerSettingsFactory
    {
        private readonly IStreamManager streamManager;
        private readonly IDataService service;
        private readonly JsonConverter readerConverter;
        private readonly JsonConverter handlerConverter;

        public FileHandlerSettingsFactory(
            IStreamManager streamManager,
            IDataService service,
            JsonConverter readerConverter,
            JsonConverter handlerConverter)
        {
            this.streamManager = streamManager;
            this.service = service;
            this.readerConverter = readerConverter;
            this.handlerConverter = handlerConverter;
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
                let contentReader = JsonConvert.DeserializeObject<IContentReader>(setting.Value, readerConverter)
                let contentHandler = JsonConvert.DeserializeObject<IContentHandler>(setting.Value, handlerConverter)
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