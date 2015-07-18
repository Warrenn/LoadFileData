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
        private static readonly ModuleBuilder BaseBuilder = CreateBaseBuilder();

        public static TypeBuilder BuildDynamicTypeWithProperties(string typeName, Type baseType = null)
        {
            return BaseBuilder.DefineType(typeName, TypeAttributes.Public, baseType);
        }

        public static object CreateInstance(string typeName, IDictionary<string, Type> properties, Type baseType = null)
        {
            var type = CreateType(typeName, properties, baseType);
            return Activator.CreateInstance(type);
        }

        public static Type CreateType(string typeName, IDictionary<string, Type> properties, Type baseType = null)
        {
            var builder = BuildDynamicTypeWithProperties(typeName, baseType);
            foreach (var property in properties)
            {
                AddProperty(builder, property.Value, property.Key);
            }
            return builder.CreateType();
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

            var customerNameBldr = builder.DefineField(fieldName, propertyType, FieldAttributes.Private);

            var custNamePropBldr = builder.DefineProperty(propertyName, PropertyAttributes.HasDefault,
                propertyType, null);

            const MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                MethodAttributes.Virtual;

            var custNameGetPropMthdBldr = builder.DefineMethod("get_" + propertyName, getSetAttr, propertyType,
                Type.EmptyTypes);

            var custNameGetIl = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIl.Emit(OpCodes.Ldarg_0);
            custNameGetIl.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIl.Emit(OpCodes.Ret);

            var custNameSetPropMthdBldr = builder.DefineMethod("set_" + propertyName, getSetAttr, null,
                new[] { propertyType });


            var custNameSetIl = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIl.Emit(OpCodes.Ldarg_0);
            custNameSetIl.Emit(OpCodes.Ldarg_1);
            custNameSetIl.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIl.Emit(OpCodes.Ret);

            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
        }

        private static ModuleBuilder CreateBaseBuilder()
        {
            var myDomain = Thread.GetDomain();
            var myAsmName = Assembly.GetExecutingAssembly().GetName();

            var myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);
            return myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
        }

        public static FluentPropertyDescription Build(string className, Type baseType = null)
        {
            return new FluentPropertyDescription(className, new Dictionary<string, Type>(), baseType);
        }

        public static FluentPropertyDescription Build<T>(string className)
        {
            return Build(className, typeof(T));
        }

        public class FluentPropertyDescription
        {
            private readonly IDictionary<string, Type> properties;
            private readonly string className;
            private readonly Type baseType;

            internal FluentPropertyDescription(string className, IDictionary<string, Type> properties, Type baseType)
            {
                this.className = className;
                this.baseType = baseType;
                this.properties = properties;
            }

            public FluentPropertyDescription Property(Type type, string name)
            {
                if (properties.ContainsKey(name))
                {
                    throw new DuplicateNameException(name);
                }
                properties.Add(name, type);
                return new FluentPropertyDescription(className, properties, baseType);
            }

            public FluentPropertyDescription Property<T>(string name)
            {
                return Property(typeof(T), name);
            }

            public FluentPropertyDescription Property(Type type)
            {
                return Property(type, type.Name);
            }

            public FluentPropertyDescription Property<T>()
            {
                return Property(typeof (T));
            }

            public Type ToType()
            {
                return CreateType(className, properties, baseType);
            }

            public object ToInstance()
            {
                var type = CreateType(className, properties, baseType);
                return Activator.CreateInstance(type);
            }

            public dynamic ToDynamic()
            {
                var type = CreateType(className, properties, baseType);
                return new DynamicProperties(type);
            }
        }
    }
}
