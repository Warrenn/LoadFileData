using System.Collections.Generic;

namespace Conventions
{
    public class SetPropertyContext
    {
        public PropertyPathDescriptor Descriptor { get; set; }
        public object BaseInstance { get; set; }
        public KeyValuePair<string, object> MatchingPair { get; set; }
        public IDictionary<string, object> AllValues { get; set; }
    }
}
