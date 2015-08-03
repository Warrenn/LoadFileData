using System;

namespace LoadFileData.Converters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConverterAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
