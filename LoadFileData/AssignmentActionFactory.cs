using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LoadFileData
{
    public static class AssignmentActionFactory
    {
        public static Func<object, T> ConvertFunction<T>()
        {
            var memberType = typeof(T);
            var nullableMethodInfo = GenericHelper.MethodInfo(
                "Nullable",
                typeof(TryParser),
                GenericHelper.StaticFlags,
                new[] { typeof(object) });

            MethodInfo convertMethod;
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericParameter = memberType.GenericTypeArguments[0];
                if (genericParameter == typeof(DateTime))
                {
                    return o => (T)((object)TryParser.DateTime(o));
                }

                convertMethod = nullableMethodInfo.MakeGenericMethod(genericParameter);
                return o => (T)convertMethod.Invoke(null, new[] { o });
            }
            if (!memberType.IsValueType)
            {
                return o => (T)o;
            }
            convertMethod = nullableMethodInfo.MakeGenericMethod(memberType);
            if (memberType == typeof(DateTime))
            {
                return o => (T)((object)TryParser.DateTime(o) ?? default(T));
            }
            return o => (T)(convertMethod.Invoke(null, new[] { o }) ?? default(T));
        }

        public static Action<TClass, object> Create<TClass, T>(
            Expression<Func<TClass, T>> memberAssignment)
            where TClass : class
        {
            var memberExpression = memberAssignment.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Expression body must be a MemberExpression");
            }
            var parameter = Expression.Parameter(typeof(TClass));
            var valueParameter = Expression.Parameter(typeof(T));
            var propertyInfo = typeof(TClass).GetProperty(memberExpression.Member.Name);
            var assignment = Expression.Assign(Expression.MakeMemberAccess(parameter, propertyInfo), valueParameter);
            var assign = Expression.Lambda<Action<TClass, T>>(assignment, parameter, valueParameter);
            var assignFunction = assign.Compile();
            var convertFunction = ConvertFunction<T>() ?? (o => (T)o);

            Action<TClass, object> complete = (instance, o) =>
                {
                    var v = convertFunction(o);
                    assignFunction(instance, v);
                };
            return complete;
        }
    }
}
