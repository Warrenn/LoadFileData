using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using LoadFileData.ContentReader;
using LoadFileData.Tests.MockFactory;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace LoadFileData.Tests.Controllers
{
    [TestClass]
    public class UnitTest1
    {
        public interface IService2
        {
            int TestResult2();
        }

        public interface IService3
        {
            int TestResult3();
        }

        public interface IService
        {
            int TestResult(int input);
        }

        public class ServiceBase
        {
            private readonly IService service;
            private readonly IService2 service2;

            protected ServiceBase(IService service, IService2 service2)
            {
                this.service = service;
                this.service2 = service2;
            }

            public virtual int NewResult(int input)
            {
                return service.TestResult(input);
            }

            public virtual int JustDoit()
            {
                return service2.TestResult2();
            }
        }

        public class Controller
        {
            private readonly ServiceBase service;
            private readonly IService3 service3;

            public int A;
            public string B;

            public Controller(ServiceBase service, IService3 service3)
            {
                this.service = service;
                this.service3 = service3;
            }

            public virtual int TestMethod(int input)
            {
                return service.NewResult(input);
            }

            public virtual int Service3Test()
            {
                return service3.TestResult3();
            }
        }

        public static void AddProperty<T>(TypeBuilder builder, string propertyName)
        {
            var propertyType = typeof (T);
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

        public static TypeBuilder BuildDynamicTypeWithProperties(string typeName, Type baseTypes = null)
        {
            var myDomain = Thread.GetDomain();
            var myAsmName = Assembly.GetExecutingAssembly().GetName();

            var myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);
            var myModBuilder = myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");

            return  myModBuilder.DefineType(typeName, TypeAttributes.Public, baseTypes);
        }

        [TestMethod]
        public void Test2()
        {
            var builder = BuildDynamicTypeWithProperties("ClassA");
            AddProperty<string>(builder, "name");
            AddProperty<int>(builder, "age");
            var type = builder.CreateType();

            var ins = Activator.CreateInstance(type);

            var i = type.GetProperty("Name");
            i.SetValue(ins, "A new Name");

            var value = i.GetValue(ins);

            Assert.AreEqual("A new Name", value);
        }


        [TestMethod]
        public void Test3()
        {
            var fields = DelimiteredReader.Split("fun:'{a|b|c}' | data | fds:'j|kkl|y'");
            
            Assert.IsTrue(fields.Length > 0);
        }
        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            var helper = new MockHelper();
            var serviceMock = helper.Mock<IService>();
            serviceMock
                .Setup(service => service.TestResult(It.Is<int>(v => v == 8)))
                .Returns(234)
                .Verifiable();

            var sut = helper.Instance<Controller>();

            //Act
            var result = sut.TestMethod(8);

            //Assert
            serviceMock.VerifyAll();
            Assert.AreEqual(234, result);
        }
    }
}
