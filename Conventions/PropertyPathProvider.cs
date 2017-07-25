using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Conventions
{
    public class PropertyPathProvider
    {
        private static readonly MethodInfo SetGenMethodInfo =
                typeof(PropertyPathProvider)
                .GetMethod("CreateSetPropertyFunctionGeneric", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo GetGenMethodInfo =
                typeof(PropertyPathProvider)
                .GetMethod("CreateGetPropertyFunctionGeneric", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo ElementAtIndexMethodInfo =
                typeof(PropertyPathProvider)
                .GetMethod("ElementAtIndex", BindingFlags.Static | BindingFlags.NonPublic);

        private static IList<Expression> CreateMemberExpression<TClass>(string path, Expression parameter)
        {
            var expressions = new List<Expression>();

            var valueExpression = parameter;
            var nullBaseExpression = Expression.Constant(null, typeof(TClass));
            var newExpression = Expression.New(typeof(TClass));
            var conditionExpression = Expression.Condition(Expression.Equal(nullBaseExpression, parameter),
                newExpression, valueExpression);
            expressions.Add(Expression.Assign(valueExpression, conditionExpression));

            var parts = path.Split('.');

            for (var i = 0; i < parts.Length - 1; i++)
            {
                valueExpression = Expression.PropertyOrField(valueExpression, parts[i]);
                newExpression = Expression.New(valueExpression.Type);
                nullBaseExpression = Expression.Constant(null, valueExpression.Type);
                conditionExpression = Expression.Condition(Expression.Equal(nullBaseExpression, valueExpression),
                    newExpression, valueExpression);

                expressions.Add(Expression.Assign(valueExpression, conditionExpression));
            }

            expressions.Add(Expression.PropertyOrField(valueExpression, parts[parts.Length - 1]));

            return expressions;
        }


        private static Action<object, object, SetPropertyContext> CreateSetPropertyFunctionGeneric<T>(string path)
        {
            var propFunc = CreateSetPropertyFunction<T>(path);
            return (i, v, c) => propFunc((T)i, v, c);
        }

        private static Func<object, object> CreateGetPropertyFunctionGeneric<T, TProp>(string path)
        {
            var propFunc = CreateGetPropertyFunction<T, TProp>(path);
            return o => propFunc((T)o);
        }

        private static Func<TClass, TProp> CreateGetPropertyFunction<TClass, TProp>(string path)
        {
            var parameter = Expression.Parameter(typeof(TClass), "classparam");
            var labelTarget = Expression.Label(typeof(TProp));
            var expressions = CreateMemberExpression<TClass>(path, parameter);
            var valueExpression = expressions.Last();

            expressions.RemoveAt(expressions.Count - 1);

            expressions.Add(Expression.Return(labelTarget, valueExpression));
            expressions.Add(Expression.Label(labelTarget, Expression.Constant(default(TProp), typeof(TProp))));

            var body = Expression.Block(expressions);
            var expressionTree = Expression.Lambda<Func<TClass, TProp>>(body, parameter);
            return expressionTree.Compile();
        }

        private static Action<TClass, object, SetPropertyContext> CreateSetPropertyFunction<TClass>(string path)
        {
            var createConvertFunctionExpressionMethodInfo = typeof(TryParser)
                .GetMethod("CreateConvertFunctionExpression");
            var parameters = new[]
            {
                Expression.Parameter(typeof(TClass), "classparam"),
                Expression.Parameter(typeof(object), "propertyparam"),
                Expression.Parameter(typeof(SetPropertyContext), "context")
            };
            var expressions = CreateMemberExpression<TClass>(path, parameters[0]);
            var valueExpression = expressions.Last();

            expressions.RemoveAt(expressions.Count - 1);

            var genericConvertFunctionMethodInfo = createConvertFunctionExpressionMethodInfo.MakeGenericMethod(valueExpression.Type);
            var convertFunction = (Expression)genericConvertFunctionMethodInfo.Invoke(null, null);
            var invokeConvertExpression = Expression.Invoke(convertFunction, parameters[1]);

            expressions.Add(Expression.Assign(valueExpression, invokeConvertExpression));

            var body = Expression.Block(expressions);
            var expressionTree = Expression.Lambda<Action<TClass, object, SetPropertyContext>>(body, parameters);
            return expressionTree.Compile();
        }

        public static IDictionary<PropertyPath, PropertyPathDescriptor> CreatePropertyPathDictionary<T>() => CreatePropertyPathDictionary(typeof(T));

        public static IDictionary<PropertyPath, PropertyPathDescriptor> CreatePropertyPathDictionary(Type coreType, Type type = null, PropertyPath path = null, PropertyPath visited = null)
        {
            var dictionary = new Dictionary<PropertyPath, PropertyPathDescriptor>();
            path = path ?? PropertyPath.Empty;
            visited = visited ?? PropertyPath.Empty;

            if (type == null)
            {
                type = coreType;
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                if (visited != PropertyPath.Empty && visited.Any(i => i == propertyInfo)) return dictionary;

                var relativepath = new PropertyPath(path) { propertyInfo };
                var newvisitedpath = new PropertyPath(visited) { propertyInfo };

                var propType = propertyInfo.PropertyType;
                var propName = $"{relativepath}";
                var setMethodInfo = SetGenMethodInfo.MakeGenericMethod(coreType);
                var getMethodInfo = GetGenMethodInfo.MakeGenericMethod(coreType, propType);

                dictionary.Add(relativepath, new PropertyPathDescriptor
                {
                    CoreType = coreType,
                    PropertyType = propType,
                    CanProcessPredicate = (p, propPath) => string.Equals(p, propName, StringComparison.InvariantCultureIgnoreCase),
                    CreateKeyValueDelegate =
                        (instance, scope, propertyPathPair) =>
                            new KeyValuePair<string, object>($"{scope}{propertyPathPair.Key}",
                                propertyPathPair.Value.GetPropertyDelegate(instance)),
                    GetValueDelegate = (dict, pair) => pair.Value,
                    SetPropertyDelegate =
                        (Action<object, object, SetPropertyContext>)setMethodInfo
                            .Invoke(null, new object[] { propName }),
                    GetPropertyDelegate =
                        (Func<object, object>)getMethodInfo
                            .Invoke(null, new object[] { propName })
                });

                var nextCoreType = coreType;

                if (propType.IsEnum ||
                    propType.IsValueType ||
                    typeof(string).IsAssignableFrom(propType) ||
                    (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    continue;

                if (typeof(IEnumerable).IsAssignableFrom(propType))
                {
                    nextCoreType = propType.IsArray
                        ? propType.GetElementType()
                        : propType.GenericTypeArguments[0];
                    relativepath = PropertyPath.Empty;
                    propType = nextCoreType;
                }


                foreach (var typeMapping in CreatePropertyPathDictionary(nextCoreType, propType, relativepath, newvisitedpath))
                {
                    dictionary[typeMapping.Key] = typeMapping.Value;
                }
            }

            return dictionary;
        }

        public static T HydrateInstance<T>(T instance, IDictionary<string, object> values, IDictionary<PropertyPath, PropertyPathDescriptor> propertyPathDictionary)
        {
            var splitRegex = new Regex(@"\.(\d+)\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (var keyvalue in values)
            {
                var parts = splitRegex.Split(keyvalue.Key);
                var partCount = parts.Length / 2;
                object subobject = instance;

                for (var pathIndex = 0; pathIndex < partCount; pathIndex++)
                {
                    var propPart = (pathIndex * 2);
                    var indexPart = (pathIndex * 2) + 1;
                    var part = parts[propPart];
                    var subDescriptor = propertyPathDictionary
                        .FirstOrDefault(desc => desc.Value.CanProcessPredicate(part, desc.Key))
                        .Value;

                    if (subDescriptor == null) continue;
                    var getProperty = subDescriptor.GetPropertyDelegate;
                    var setProperty = subDescriptor.SetPropertyDelegate;
                    var propType = subDescriptor.PropertyType;

                    var indexString = parts[indexPart];
                    int arrayIndex;
                    if (!int.TryParse(indexString, out arrayIndex)) continue;
                    arrayIndex--;

                    var collection = getProperty(subobject);

                    var arrayType = propType.IsArray
                        ? propType.GetElementType()
                        : propType.GenericTypeArguments[0];


                    var genElementAtIndexMethodInfo = ElementAtIndexMethodInfo.MakeGenericMethod(arrayType);
                    var arguments = new[] { collection, arrayIndex };
                    var element = genElementAtIndexMethodInfo.Invoke(null, arguments);
                    var context = new SetPropertyContext
                    {
                        AllValues = values,
                        Descriptor = subDescriptor,
                        BaseInstance = instance,
                        MatchingPair = keyvalue
                    };

                    collection = arguments[0];
                    setProperty(subobject, collection, context);

                    subobject = element;
                }

                var lastpart = parts[parts.Length - 1];
                var descriptor = propertyPathDictionary.FirstOrDefault(desc => desc.Value.CanProcessPredicate(lastpart,desc.Key)).Value;
                if (descriptor == null) continue;

                var setPropertyFunc = descriptor.SetPropertyDelegate;
                var getValue = descriptor.GetValueDelegate;
                var value = getValue(values, keyvalue);
                var lastpartContext = new SetPropertyContext
                {
                    AllValues = values,
                    Descriptor = descriptor,
                    BaseInstance = instance,
                    MatchingPair = keyvalue
                };

                setPropertyFunc(subobject, value, lastpartContext);
            }

            return instance;
        }

        private static T ElementAtIndex<T>(ref object instance, int index)
        {
            var num = 0;
            var collection = instance as ICollection<T> ?? new List<T>();

            using (var enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (num == index) return enumerator.Current;
                    checked { ++num; }
                }
            }
            var type = typeof(T);
            var createDefault = (type.GetConstructor(Type.EmptyTypes) == null)
                ? (Func<T>)(() => default(T))
                : () => (T)Activator.CreateInstance(type);
            var newItem = createDefault();
            while (num <= index)
            {
                newItem = createDefault();
                collection.Add(newItem);
                checked { ++num; }
            }
            instance = collection;
            return newItem;
        }
    }
}
