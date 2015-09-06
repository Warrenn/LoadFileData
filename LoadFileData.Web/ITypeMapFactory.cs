using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LoadFileData.Web
{
    public interface ITypeMapFactory
    {
        IDictionary<string, Type> CreateTypeMap(ModuleBuilder builder = null);
    }
}