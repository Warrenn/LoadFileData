﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using LoadFileData.Converters;
using LoadFileData.DAL.Models;
using LoadFileData.Web.Constants;
using Newtonsoft.Json;

namespace LoadFileData.Web
{
    public class TypeMapFactory : ITypeMapFactory
    {
        private readonly Lazy<IDictionary<string, Type>> lazyMaping;

        public TypeMapFactory()
        {
            lazyMaping = new Lazy<IDictionary<string, Type>>(InternalMapping);
        }

        public static string ScrubVariableName(string variableName)
        {
            var value = new char[variableName.Length];
            for (var i = 0; i < variableName.Length; i++)
            {
                var c = variableName[i];
                if (!char.IsLetterOrDigit(c))
                {
                    value[i] = '_';
                }
                else
                {
                    value[i] = c;
                }
                if ((i == 0) && (char.IsDigit(c)))
                {
                    value[0] = '_';
                }
            }
            return new string(value);
        }

        protected virtual IDictionary<string, string> ReadJsonSettings()
        {
            var settingsFolder = ConfigurationManager.AppSettings[Folders.DataEntries];
            if (string.IsNullOrEmpty(settingsFolder) || !Directory.Exists(settingsFolder))
            {
                throw new SettingsPropertyWrongTypeException(
                    string.Format("AppSetting {0} must be a reference to a folder that exists", Folders.DataEntries));
            }

            var settings = new Dictionary<string, string>();
            foreach (var fileName in Directory.GetFiles(settingsFolder, "*.json"))
            {
                settings[fileName] = File.ReadAllText(fileName);
            }
            return settings;
        }

        protected virtual Type CreateTypeFromJson(string name, string jsonData)
        {
            var className = ScrubVariableName(name);
            var builder = ClassBuilder.Build(className, typeof(DataEntry));
            var properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            foreach (var property in properties)
            {
                var type = ConverterManager.LookupType(property.Value);
                if (type == null)
                {
                    continue;
                }
                var propertyName = ScrubVariableName(property.Key);
                builder = builder.Property(type, propertyName);
            }
            return builder.ToType();
        }

        protected virtual IDictionary<string, Type> InternalMapping()
        {
            var dataEntryTypes = AssemblyHelper
                .LoadableTypesOf<DataEntry>()
                .ToDictionary(loadableType => loadableType.Name);

            foreach (var setting in ReadJsonSettings())
            {
                var name = Path.GetFileNameWithoutExtension(setting.Key);
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var type = CreateTypeFromJson(setting.Key, setting.Value);
                dataEntryTypes[name] = type;
            }
            return dataEntryTypes;
        }

        public IDictionary<string, Type> CreateTypeMapping()
        {
            return lazyMaping.Value;
        }
    }
}