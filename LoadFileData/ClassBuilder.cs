using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using PropertyAttributes = System.Reflection.PropertyAttributes;

namespace LoadFileData
{
    public static class ClassBuilder
    {
        public static TypeBuilder BuildDynamicTypeWithProperties(string typeName, Type baseType = null, ModuleBuilder builder = null)
        {
            if (builder == null)
            {
                builder = CreateBaseBuilder();
            }
            return builder.DefineType(typeName, TypeAttributes.Public, baseType);
        }

        public static object CreateInstance(string typeName, IDictionary<string, Type> properties, Type baseType = null,
            ModuleBuilder builder = null)
        {
            var type = CreateType(typeName, properties, baseType, builder);
            return Activator.CreateInstance(type);
        }

        public static Type CreateType(string typeName, IDictionary<string, Type> properties, Type baseType = null,
            ModuleBuilder builder = null)
        {
            var typeBuilder = BuildDynamicTypeWithProperties(typeName, baseType, builder);
            foreach (var property in properties)
            {
                AddProperty(typeBuilder, property.Value, property.Key);
            }
            return typeBuilder.CreateType();
        }

        public static void AddProperty<T>(TypeBuilder builder, string propertyName)
        {
            var propertyType = typeof(T);
            AddProperty(builder, propertyType, propertyName);
        }

        public static void AddProperty(TypeBuilder builder, Type propertyType, string propertyName)
        {
            propertyName = char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1);
            var fieldName = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

            var customFldBldr = builder.DefineField(fieldName, propertyType, FieldAttributes.Private);

            var custNamePropBldr = builder.DefineProperty(propertyName, PropertyAttributes.HasDefault,
                propertyType, null);

            const MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                MethodAttributes.Virtual;

            var custNameGetPropMthdBldr = builder.DefineMethod("get_" + propertyName, getSetAttr, propertyType,
                Type.EmptyTypes);

            var custNameGetIl = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIl.Emit(OpCodes.Ldarg_0);
            custNameGetIl.Emit(OpCodes.Ldfld, customFldBldr);
            custNameGetIl.Emit(OpCodes.Ret);

            var custNameSetPropMthdBldr = builder.DefineMethod("set_" + propertyName, getSetAttr, null,
                new[] { propertyType });

            var custNameSetIl = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIl.Emit(OpCodes.Ldarg_0);
            custNameSetIl.Emit(OpCodes.Ldarg_1);
            custNameSetIl.Emit(OpCodes.Stfld, customFldBldr);
            custNameSetIl.Emit(OpCodes.Ret);

            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
        }

        public static ModuleBuilder CreateBaseBuilder()
        {
            var myDomain = Thread.GetDomain();
            var myAsmName = Assembly.GetExecutingAssembly().GetName();

            var myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);
            return myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
        }

        public static FluentClassBuilder Build(string className, Type baseType = null)
        {
            return new FluentClassBuilder(className, new Dictionary<string, Type>(), baseType);
        }

        public static FluentClassBuilder Build<T>(string className)
        {
            return Build(className, typeof(T));
        }

        public class FluentClassBuilder
        {
            private readonly IDictionary<string, Type> properties;
            private readonly string className;
            private readonly Type baseType;

            internal FluentClassBuilder(string className, IDictionary<string, Type> properties, Type baseType)
            {
                this.className = className;
                this.baseType = baseType;
                this.properties = properties;
            }

            public FluentClassBuilder Property(Type type, string name)
            {
                if (properties.ContainsKey(name))
                {
                    throw new DuplicateNameException(name);
                }
                properties.Add(name, type);
                return new FluentClassBuilder(className, properties, baseType);
            }

            public FluentClassBuilder Property<T>(string name)
            {
                return Property(typeof(T), name);
            }

            public FluentClassBuilder Property(Type type)
            {
                return Property(type, type.Name);
            }

            public FluentClassBuilder Property<T>()
            {
                return Property(typeof (T));
            }

            public Type ToType(ModuleBuilder builder =  null)
            {
                return CreateType(className, properties, baseType, builder);
            }

            public object ToInstance(ModuleBuilder builder = null)
            {
                var type = CreateType(className, properties, baseType, builder);
                return Activator.CreateInstance(type);
            }

            public dynamic ToDynamic(ModuleBuilder builder = null)
            {
                var type = CreateType(className, properties, baseType, builder);
                return new DynamicProperties(type);
            }
        }
    }
}
