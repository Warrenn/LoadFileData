using System.Collections.Generic;
using System.Configuration;
using System.IO;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentReaders;
using LoadFileData.DAL;
using LoadFileData.FileHandlers;
using LoadFileData.Web.Constants;
using Newtonsoft.Json;

namespace LoadFileData.Web
{
    public class FileHandlerFactory : IFileHandlerFactory
    {
        private readonly IStreamManager streamManager;
        private readonly IDataService service;
        private readonly JsonConverter readerConverter;
        private readonly JsonConverter handlerConverter;

        public FileHandlerFactory(
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

        protected virtual IDictionary<string, string> ReadJsonSettings()
        {
            var settingsFolder = ConfigurationManager.AppSettings[Folders.WatchDefinitons];
            if (string.IsNullOrEmpty(settingsFolder) || !Directory.Exists(settingsFolder))
            {
                throw new SettingsPropertyWrongTypeException(
                    string.Format("AppSetting {0} must be a reference to a folder that exists", Folders.WatchDefinitons));
            }

            var settings = new Dictionary<string, string>();
            foreach (var fileName in Directory.GetFiles(settingsFolder,"*.json"))
            {
                settings[fileName] = File.ReadAllText(fileName);
            }
            return settings;
        }


        #region Implementation of IFileHandlerFactory

        public virtual IDictionary<string, IFileHandler> CreateFileHandlers()
        {
            var copyToTemplate = ConfigurationManager.AppSettings[Folders.CopyTo];
            if (string.IsNullOrEmpty(copyToTemplate))
            {
                throw new SettingsPropertyWrongTypeException(
                    string.Format("AppSetting {0} must have a valid path template", Folders.CopyTo));
            }

            var returnValue = new Dictionary<string, IFileHandler>();

            foreach (var setting in ReadJsonSettings())
            {
                var name = Path.GetFileNameWithoutExtension(setting.Key);
                var contentReader = JsonConvert.DeserializeObject<IContentReader>(setting.Value, readerConverter);
                var contentHandler = JsonConvert.DeserializeObject<IContentHandler>(setting.Value, handlerConverter);
                var fileHandlerSettings = new FileHandlerSettings
                {
                    DestinationPathTemplate = copyToTemplate,
                    Service = service,
                    StreamManager = streamManager,
                    Name = name,
                    ContentHandler = contentHandler,
                    Reader = contentReader
                };
                var fileHandler = new FileHandler(fileHandlerSettings);
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                returnValue[name] = fileHandler;
            }

            return returnValue;
        }

        #endregion
    }
}