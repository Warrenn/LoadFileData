using System;
using System.Collections.Generic;

namespace Conventions
{
    public class PropertyPathDescriptor
    {
        public Func<string, PropertyPath, bool> CanProcessPredicate { get; set; }
        public Func<IDictionary<string, object>, KeyValuePair<string, object>, object> GetValueDelegate { get; set; }
        public Action<object, object, SetPropertyContext> SetPropertyDelegate { get; set; }

        public Func<object, string, KeyValuePair<PropertyPath, PropertyPathDescriptor>, KeyValuePair<string, object>>
            CreateKeyValueDelegate
        { get; set; }

        public Func<object, object> GetPropertyDelegate { get; set; }
        public Type PropertyType { get; set; }
        public Type CoreType { get; set; }
    }
}