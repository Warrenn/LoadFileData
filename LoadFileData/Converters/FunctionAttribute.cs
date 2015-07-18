using System;

namespace LoadFileData.Converters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {
        public string SpecificName { get; set; }
    }
}
