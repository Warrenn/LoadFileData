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

        private static readonly IDictionary<string, Converter> DefaultFunctions =
            new SortedDictionary<string, Converter>(StringComparer.InvariantCultureIgnoreCase)
            {
                {
                    "int", new Converter
                    {
                        ReturnType = typeof (int),
                        Function = o => TryParser.Default<int>(o)
                    }
                },
                {
                    "long", new Converter
                    {
                        ReturnType = typeof (long),
                        Function = o => TryParser.Default<long>(o)
                    }
                },
                {
                    "decimal", new Converter
                    {
                        ReturnType = typeof (decimal),
                        Function = o => TryParser.Default<int>(o)
                    }
                },
                {
                    "float", new Converter
                    {
                        ReturnType = typeof (float),
                        Function = o => TryParser.Default<float>(o)
                    }
                },
                {
                    "bool", new Converter
                    {
                        ReturnType = typeof (bool),
                        Function = o => TryParser.Default<bool>(o)
                    }
                },
                {
                    "DateTime", new Converter
                    {
                        ReturnType = typeof (DateTime),
                        Function = o => TryParser.DateTime(o) ?? default(DateTime)
                    }
                },
                {
                    "string", new Converter
                    {
                        ReturnType = typeof (string),
                        Function = o => o as string
                    }
                },
                {
                    "int?", new Converter
                    {
                        ReturnType = typeof (int?),
                        Function = o => TryParser.Nullable<int>(o)
                    }
                },
                {
                    "long?", new Converter
                    {
                        ReturnType = typeof (long?),
                        Function = o => TryParser.Nullable<long>(o)
                    }
                },
                {
                    "decimal?", new Converter
                    {
                        ReturnType = typeof (decimal?),
                        Function = o => TryParser.Nullable<int>(o)
                    }
                },
                {
                    "float?", new Converter
                    {
                        ReturnType = typeof (float?),
                        Function = o => TryParser.Nullable<float>(o)
                    }
                },
                {
                    "bool?", new Converter
                    {
                        ReturnType = typeof (bool?),
                        Function = o => TryParser.Nullable<bool>(o)
                    }
                },
                {
                    "DateTime?", new Converter
                    {
                        ReturnType = typeof (DateTime?),
                        Function = o => TryParser.DateTime(o)
                    }
                }
            };

        private static readonly IDictionary<string, MethodInfo> ReflectedFunctions = GetReflectedFunctions();

        private static IDictionary<string, MethodInfo> GetReflectedFunctions()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in AssemblyHelper.GetLoadableTypes(assembly)
                    from info in type.GetMethods()
                    where
                        info.IsStatic &&
                        typeof(Delegate).IsAssignableFrom(info.ReturnType) &&
                        info.ReturnType.IsGenericType &&
                        info.ReturnType.GenericTypeArguments.Count() == 2 &&
                        info.ReturnType.GenericTypeArguments[0] == typeof(object)
                    let attr = info
                        .GetCustomAttributes(typeof(FunctionAttribute), false)
                        .FirstOrDefault() as FunctionAttribute
                    where attr != null
                    select new { attr, info }).ToDictionary(
                    a => string.IsNullOrEmpty(a.attr.SpecificName) ? a.info.Name : a.attr.SpecificName,
                    a => a.info);
        }

        public static Converter GetConverter(string convertString)
        {
            convertString = convertString.Trim().TrimEnd(')');

            var match = Regex.Match(convertString, "^([^\\(]+)\\(?(.*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success || match.Groups.Count != 2 || match.Groups.Count != 3)
            {
                return null;
            }

            var methodName = match.Groups[1].Value;

            if (DefaultFunctions.ContainsKey(methodName))
            {
                return DefaultFunctions[methodName];
            }

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
                ReturnType = returnFunctionType.GenericTypeArguments[0]
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
            dynamic tryParser = new GenericInvoker(typeof(TryParser));

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameterInfos[i].ParameterType;
                if (parameterType.IsValueType)
                {
                    returnValue[i] = tryParser.Default(parameterType, parameters[i]);
                }
                else
                {
                    returnValue[i] = Convert.ChangeType(parameters[i], parameterType);
                }
            }

            return returnValue;
        }
    }
}
