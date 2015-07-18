using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LoadFileData.ContentReaders;

namespace LoadFileData.Converters
{
    public static class ConverterManager
    {
        public class Converter
        {
            public Type ReturnType { get; set; }
            public Func<object, object> Function { get; set; }
        }

        private static readonly IDictionary<string, Type> TypeMap =
            new SortedDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"byte", typeof (byte)},
                {"char", typeof (char)},
                {"int", typeof (int)},
                {"long", typeof (long)},
                {"decimal", typeof (decimal)},
                {"float", typeof (float)},
                {"bool", typeof (bool)},
                {"DateTime", typeof (DateTime)},
                {"string", typeof (string)},
                {"byte?", typeof (byte?)},
                {"char?", typeof (char?)},
                {"int?", typeof (int?)},
                {"long?", typeof (long?)},
                {"decimal?", typeof (decimal?)},
                {"float?", typeof (float?)},
                {"bool?", typeof (bool?)},
                {"DateTime?", typeof (DateTime?)}
            };

        private static readonly IDictionary<string, MethodInfo> ReflectedFunctions = GetReflectedFunctions();

        private static IDictionary<string, MethodInfo> GetReflectedFunctions()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in AssemblyHelper.GetLoadableTypes(assembly)
                from info in type.GetMethods()
                where
                    info.IsPublic &&
                    info.IsStatic &&
                    typeof (Delegate).IsAssignableFrom(info.ReturnType) &&
                    info.ReturnType.IsGenericType &&
                    info.ReturnType.GenericTypeArguments.Count() == 2 &&
                    info.ReturnType.GenericTypeArguments[0] == typeof (object)
                let attr = info
                    .GetCustomAttributes(typeof (ConverterAttribute), false)
                    .FirstOrDefault() as ConverterAttribute
                where attr != null
                select new {attr, info}).ToDictionary(
                    a => string.IsNullOrEmpty(a.attr.SpecificName) ? a.info.Name : a.attr.SpecificName,
                    a => a.info);
        }

        public static Type LookupType(string typeString)
        {
            if (TypeMap.ContainsKey(typeString))
            {
                return TypeMap[typeString];
            }
            var returnType = Type.GetType(typeString);
            if (returnType != null)
            {
                return returnType;
            }
            var systemAssembly = Assembly.GetAssembly(typeof(short));
            returnType = systemAssembly.GetType(typeString);
            if (returnType != null)
            {
                return returnType;
            }

            return
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in AssemblyHelper.GetLoadableTypes(assembly)
                 where string.Equals(type.Name, typeString, StringComparison.InvariantCultureIgnoreCase)
                 select type).FirstOrDefault();
        }

        public static Converter GetConverter(string convertString)
        {
            convertString = convertString.Trim().TrimEnd(')');

            var match = Regex.Match(convertString, "^([^\\(]+)\\(?(.*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success || (match.Groups.Count != 2 && match.Groups.Count != 3))
            {
                return null;
            }

            var methodName = match.Groups[1].Value;

            if (!ReflectedFunctions.ContainsKey(methodName))
            {
                return null;
            }

            var methodInfo = ReflectedFunctions[methodName];

            var parameters = match.Groups.Count == 3
                ? CreateParameters(match.Groups[2].Value, methodInfo)
                : null;

            var returnFunction = methodInfo.Invoke(null, parameters);
            var returnFunctionType = returnFunction.GetType();

            var invokeMethod = returnFunctionType.GetMethod("Invoke");
            Func<object, object> convertFunc = o => invokeMethod.Invoke(returnFunction, new[] { o });
            return new Converter
            {
                Function = convertFunc,
                ReturnType = returnFunctionType.GenericTypeArguments[1]
            };
        }

        private static object[] CreateParameters(string parameterString, MethodInfo methodInfo)
        {
            var parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length == 0)
            {
                return null;
            }

            var parameters = DelimiteredReader.SplitLine(parameterString);
            if (parameters.Length != parameterInfos.Length)
            {
                throw new InvalidOperationException("parameters don't match method signature");
            }

            var returnValue = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameterInfos[i].ParameterType;
                returnValue[i] = TryParser.ChangeType(parameters[i], parameterType);
            }

            return returnValue;
        }
    }
}
