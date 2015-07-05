using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using LoadFileData.ContentReader;
using LoadFileData.Tests.MockFactory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LoadFileData.Tests.Controllers
{

    public class Migration<T> : DbMigrationsConfiguration<T> where T : DbContext
    {
        public Migration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(T context)
        { }

    }

    public class DataClass
    {
        public int Id { get; set; }
        public int Hash { get; set; }
    }

    public class DataClass2
    {
        public int Id { get; set; }
        public decimal Hash { get; set; }
    }

    public class MyClass : DbContext
    {
        public MyClass()
            : base("dbcontext")
        {
            
        }

        public DbSet<DataClass2> Class2 { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Properties()
                .Where(pi => pi.PropertyType == typeof (decimal))
                .Configure(c => c.HasPrecision(10, 3));
            modelBuilder
                .Entity<DataClass2>()
                .Property(dc => dc.Hash)
                .HasPrecision(5, 3);
            base.OnModelCreating(modelBuilder);
        }
    }

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

        public static ModuleBuilder BaseBuilder()
        {
            var myDomain = Thread.GetDomain();
            var myAsmName = Assembly.GetExecutingAssembly().GetName();

            var myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);
            return myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");

        }

        public static TypeBuilder BuildDynamicTypeWithProperties(ModuleBuilder baseBuilder, string typeName, Type baseTypes = null)
        {
            return baseBuilder.DefineType(typeName, TypeAttributes.Public, baseTypes);
        }

        [TestMethod]
        public void Test2()
        {
            var baseb = BaseBuilder();
            var builder = BuildDynamicTypeWithProperties(baseb, "ClassA", typeof(DataClass));
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
        public void TestAgain()
        {
            var baseb = BaseBuilder();
            var builder = BuildDynamicTypeWithProperties(baseb, "ClassA", typeof(DataClass));
            AddProperty<string>(builder, "name");
            AddProperty<string>(builder, "surname");
            AddProperty<int>(builder, "age");
            AddProperty<decimal>(builder, "amountd");
            var type = builder.CreateType();

            var builderMyClass = BuildDynamicTypeWithProperties(baseb, "newDbClass", typeof(MyClass));
            var genPropType = typeof(DbSet<>);
            var propType = genPropType.MakeGenericType(type);

            AddProperty(builderMyClass, propType, "Propa");

            var myClassType = builderMyClass.CreateType();
            var migType = typeof(Migration<>).MakeGenericType(myClassType);
            //var migInst = Activator.CreateInstance(migType);

            //var migrator = new DbMigrator((DbMigrationsConfiguration)migInst);
            //migrator.Update();;
            var initType = typeof(MigrateDatabaseToLatestVersion<,>).MakeGenericType(myClassType, migType);
            var initInst = Activator.CreateInstance(initType);

            var initMethod = typeof(Database).GetMethod("SetInitializer").MakeGenericMethod(myClassType);
            initMethod.Invoke(null, BindingFlags.Default, null, new[] { initInst }, CultureInfo.InvariantCulture);

            var ctx = Activator.CreateInstance(myClassType);
            var myClass =(MyClass)ctx;
            myClass.Database.Initialize(true);
            var prop = myClassType.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance, null,
                new[] {typeof (Type)}, null);

            var values = myClass.Set(type);
            var tinst = Activator.CreateInstance(type);
            type.GetProperty("Amountd").SetMethod.Invoke(tinst, new object[] {12.666666789M});
            type.GetProperty("Name").SetMethod.Invoke(tinst, new object[] {"name 2"});
            type.GetProperty("Surname").SetMethod.Invoke(tinst, new object[] {"surname 2"});
            type.GetProperty("Age").SetMethod.Invoke(tinst, new object[] {44});
            values.Add(tinst);
            var c = myClass.Class2.Create();
            c.Hash = 12.66666666M;

            myClass.Class2.Add(c);
            myClass.SaveChanges();


        }


        [TestMethod]
        public void Test3()
        {
            var fields = DelimiteredReader.Split("fun:'{a|b|c}' | data | fds:'j|kkl|y'");

            Assert.IsTrue(fields.Length > 0);
        }

        [TestMethod]
        public void Testttt()
        {
            dynamic proxy = new GenericInvoker(typeof(TryParser));
            var datetime = proxy.Default(new[] { typeof(DateTime) }, "201O-08-14");
            Assert.AreEqual(new DateTime(2010, 8, 14), datetime);
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
