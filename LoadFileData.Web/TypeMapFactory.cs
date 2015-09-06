using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using LoadFileData.Converters;
using LoadFileData.DAL.Models;
using LoadFileData.Web.Constants;
using Newtonsoft.Json;

namespace LoadFileData.Web
{
    public class TypeMapFactory : ITypeMapFactory
    {
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

        private static IDictionary<string, string> ReadJsonSettings()
        {
            return StreamManager.AppSettingsFiles(Folders.DataEntries);
        }

        private static Type CreateTypeFromJson(string name, string jsonData, ModuleBuilder builder)
        {
            var className = ScrubVariableName(name);
            var classBuilder = ClassBuilder.Build<DataEntry>(className);
            var properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            foreach (var property in properties)
            {
                var type = ConverterManager.LookupType(property.Value);
                if (type == null)
                {
                    continue;
                }
                var propertyName = ScrubVariableName(property.Key);
                classBuilder = classBuilder.Property(type, propertyName);
            }
            return classBuilder.ToType(builder);
        }

        private static IDictionary<string, Type> CreateTypeMapInternal(ModuleBuilder builder)
        {
            var dataEntryTypes = AssemblyHelper
                .LoadableTypesOf<DataEntry>()
                .ToDictionary(loadableType => loadableType.Name, StringComparer.InvariantCultureIgnoreCase);

            foreach (var setting in ReadJsonSettings())
            {
                var name = Path.GetFileNameWithoutExtension(setting.Key);
                if (string.IsNullOrEmpty(name) ||
                    (dataEntryTypes.ContainsKey(name)))
                {
                    continue;
                }
                var type = CreateTypeFromJson(name, setting.Value, builder);
                dataEntryTypes[name] = type;
            }
            return dataEntryTypes;
        }

        public IDictionary<string, Type> CreateTypeMap(ModuleBuilder builder = null)
        {
            return CreateTypeMapInternal(builder);
        }
    }
}
