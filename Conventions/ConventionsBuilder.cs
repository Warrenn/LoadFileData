using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Conventions
{
    public static class ConventionsBuilder
    {
        private static readonly ConcurrentDictionary<Type, Lazy<IDictionary<PropertyPath, PropertyPathDescriptor>>>
            PropertyPathsLookup =
                new ConcurrentDictionary<Type, Lazy<IDictionary<PropertyPath, PropertyPathDescriptor>>>();

        public static IDictionary<PropertyPath, PropertyPathDescriptor> GetPropertyPathDictionary<T>() => GetPropertyPathDictionary(typeof(T));

        public static IDictionary<PropertyPath, PropertyPathDescriptor> GetPropertyPathDictionary(Type type)
        {
            var lazy = PropertyPathsLookup.GetOrAdd(type,
                new Lazy<IDictionary<PropertyPath, PropertyPathDescriptor>>(() => PropertyPathProvider.CreatePropertyPathDictionary(type)));

            return lazy.Value;
        }

        public static ConventionPropertyDescriptor<T> When<T>(
            this ConventionPropertyDescriptor<T> descriptor,
            Func<string, PropertyPath, bool> canProcessPredicate)
        {
            descriptor.CanProcessPredicate = canProcessPredicate;
            return descriptor;
        }

        public static ConventionPropertyDescriptor<T> GetValue<T>(
            this ConventionPropertyDescriptor<T> descriptor,
            Func<IDictionary<string, object>, KeyValuePair<string, object>, object> getValueDelegate)
        {
            descriptor.GetValueDelegate = getValueDelegate;
            return descriptor;
        }

        public static ConventionPropertyDescriptor<T> SetProperty<T>(
            this ConventionPropertyDescriptor<T> descriptor,
            Action<object, object, SetPropertyContext> setPropertyDelegate)
        {
            descriptor.SetPropertyDelegate = setPropertyDelegate;
            return descriptor;
        }

        public static ConventionPropertyDescriptor<T> ReturnKey<T>(
            this ConventionPropertyDescriptor<T> descriptor,
            Func<object, string, KeyValuePair<PropertyPath, PropertyPathDescriptor>, KeyValuePair<string, object>> createKeyValueDelegate)
        {
            descriptor.CreateKeyValueDelegate = createKeyValueDelegate;
            return descriptor;
        }

        public static ConventionPropertyDescriptor<T> GetProperty<T>(
            this ConventionPropertyDescriptor<T> descriptor,
            Func<object, object> getPropertyDelegate)
        {
            descriptor.GetPropertyDelegate = getPropertyDelegate;
            return descriptor;
        }

        public static ConventionModelDescriptor<T> ForModel<T>() => new ConventionModelDescriptor<T>();

        public static void Build<T>(this ConventionPropertyDescriptor<T> descriptor)
        {
            var dictionary = GetPropertyPathDictionary<T>();
            var modelDescriptor = descriptor.ModelDescriptor;

            if (modelDescriptor.FilterExpression == null && descriptor.PropertyPath != null && dictionary.ContainsKey(descriptor.PropertyPath))
            {
                Build(dictionary[descriptor.PropertyPath], descriptor);
            }

            if (modelDescriptor.FilterExpression == null) return;

            foreach (var keyvalue in dictionary.Where(kv => modelDescriptor.FilterExpression(kv.Key)))
            {
                Build(keyvalue.Value, modelDescriptor.PropertyDescriptor);
            }

        }

        private static void Build<T>(PropertyPathDescriptor pathDescriptor,
            ConventionPropertyDescriptor<T> convention)
        {
            if (convention.CanProcessPredicate != null)
            {
                pathDescriptor.CanProcessPredicate = convention.CanProcessPredicate;
            }

            if (convention.SetPropertyDelegate != null)
            {
                pathDescriptor.SetPropertyDelegate = convention.SetPropertyDelegate;
            }

            if (convention.GetValueDelegate != null)
            {
                pathDescriptor.GetValueDelegate = convention.GetValueDelegate;
            }

            if (convention.CreateKeyValueDelegate != null)
            {
                pathDescriptor.CreateKeyValueDelegate = convention.CreateKeyValueDelegate;
            }

            if (convention.GetPropertyDelegate != null)
            {
                pathDescriptor.GetPropertyDelegate = convention.GetPropertyDelegate;
            }
        }


        public static ConventionPropertyDescriptor<T> ForProperty<T>(
            this ConventionModelDescriptor<T> modelDescriptor,
            Expression<Func<T, object>> memberExpression)
        {
            var visitor = new MemberExpressionVisitor();
            visitor.Visit(memberExpression);

            var descriptor = new ConventionPropertyDescriptor<T>
            {
                PropertyPath = visitor.PropertyPath,
                ModelDescriptor = modelDescriptor
            };

            modelDescriptor.PropertyDescriptor = descriptor;

            return descriptor;
        }

        public static ConventionPropertyDescriptor<T> WhereProperties<T>(
            this ConventionModelDescriptor<T> modelDescriptor,
            Func<PropertyPath, bool> filterExpression)
        {
            var descriptor = new ConventionPropertyDescriptor<T> { ModelDescriptor = modelDescriptor };

            modelDescriptor.PropertyDescriptor = descriptor;
            modelDescriptor.FilterExpression = filterExpression;

            return descriptor;
        }


        public class ConventionModelDescriptor<T>
        {
            internal ConventionModelDescriptor()
            {
                FilterExpression = null;
                PropertyDescriptor = new ConventionPropertyDescriptor<T>();
            }

            internal Func<PropertyPath, bool> FilterExpression { get; set; }

            internal ConventionPropertyDescriptor<T> PropertyDescriptor { get; set; }
        }

        public class ConventionPropertyDescriptor<T>
        {
            internal ConventionPropertyDescriptor()
            {
                ModelDescriptor = null;
                PropertyPath = null;
                CanProcessPredicate = null;
                GetValueDelegate = null;
                SetPropertyDelegate = null;
                CreateKeyValueDelegate = null;
                GetPropertyDelegate = null;
            }

            internal ConventionModelDescriptor<T> ModelDescriptor { get; set; }
            internal PropertyPath PropertyPath { get; set; }
            internal Func<string, PropertyPath, bool> CanProcessPredicate { get; set; }
            internal Func<IDictionary<string, object>, KeyValuePair<string, object>, object> GetValueDelegate { get; set; }
            internal Action<object, object, SetPropertyContext> SetPropertyDelegate { get; set; }

            internal Func<object, string, KeyValuePair<PropertyPath, PropertyPathDescriptor>, KeyValuePair<string, object>>
                CreateKeyValueDelegate
            { get; set; }

            internal Func<object, object> GetPropertyDelegate { get; set; }
        }

        private class MemberExpressionVisitor : ExpressionVisitor
        {
            private bool reachedTerminalNode;
            private readonly Stack<PropertyInfo> members = new Stack<PropertyInfo>();

            public PropertyPath PropertyPath => new PropertyPath(members);

            protected override Expression VisitMember(MemberExpression node)
            {
                var info = (PropertyInfo)node.Member;
                reachedTerminalNode =
                    reachedTerminalNode ||
                    (!typeof(string).IsAssignableFrom(info.PropertyType) &&
                    members.Any() &&
                     typeof(IEnumerable).IsAssignableFrom(info.PropertyType));

                if (!reachedTerminalNode)
                {
                    members.Push(info);
                }

                return base.VisitMember(node);
            }
        }
    }
}