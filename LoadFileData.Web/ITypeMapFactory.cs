using System;
using System.Collections.Generic;

namespace LoadFileData.Web
{
    public interface ITypeMapFactory
    {
        IDictionary<string, Type> CreateTypeMapping();
    }
}