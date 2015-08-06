using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using LoadFileData.FileHandlers;
using LoadFileData.Web.Constants;

namespace LoadFileData.Web
{
    public class StreamManager : IStreamManager
    {
        #region Implementation of IStreamManager

        public void CopyFile(string source, string destination)
        {
            File.Copy(source, destination, true);
        }

        public Stream OpenRead(string fullPath)
        {
            return File.OpenRead(fullPath);
        }

        #endregion

        public static IDictionary<string, string> AppSettingsFiles(string name)
        {
            var settingsFolder = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(settingsFolder) || !Directory.Exists(settingsFolder))
            {
                throw new SettingsPropertyWrongTypeException(
                    string.Format("AppSetting {0} must be a reference to a folder that exists", name));
            }

            var settings = new Dictionary<string, string>();
            foreach (var fileName in Directory.GetFiles(settingsFolder, "*.json"))
            {
                settings[fileName] = File.ReadAllText(fileName);
            }
            return settings;
        }

    }
}
