using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ContentReaders;
using LoadFileData.Converters;
using LoadFileData.DAL;
using LoadFileData.DAL.Models;
using LoadFileData.Tests.MockFactory;
using LoadFileData.Web;
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
                .Where(pi => pi.PropertyType == typeof(decimal))
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
            private readonly IService service;
            private readonly IService2 service2;

            public Controller(IService service, IService2 service2)
            {
                this.service = service;
                this.service2 = service2;
            }

            public virtual int TestMethod(int input)
            {
                return service.TestResult(input);
            }

            public virtual int Service3Test()
            {
                return service2.TestResult2();
            }
        }

        [TestMethod]
        public void Test2()
        {
            var ins = ClassBuilder.CreateInstance("ClassA", new Dictionary<string, Type>
            {
                {"name", typeof (string)},
                {"age", typeof (int)}
            });

            dynamic dyn = new DynamicProperties(ins);
            dyn.Name = "A new Name";

            var value = dyn.Name;
            Assert.AreEqual("A new Name", value);

        }

        [TestMethod]
        public void RegexMustNotAllowNumbersInfront()
        {
            var inputdata = "1a v_aria-ble ' name () == 123$123[";

            
            var input2 = "ava_riable   ggg  -pp";
            var replacement = Regex.Replace(input2, "[^\\d\\w]*", "", RegexOptions.Compiled);

            Assert.IsNotNull(replacement);
        }

        [Converter(Name = "ConvertMe")]
        public static Func<object, string> Convert(int a, bool? b, string c, DateTime time, DateTime? time2)
        {
            return o => string.Format("a:{0} b:{1} c:{2} time:{3} o:{4} time2:{5}", a, b, c, time, o, time2);
        }

        [TestMethod]
        public void ConverterManagerNeedsToGetReflectedFunctions()
        {
            var converter = ConverterManager.GetConverter("ConvertMe(1,true,'yellow there','2012-12-31',\"asdfasdf\")");
            Assert.AreEqual(typeof(string), converter.ReturnType);
            var returnValue = converter.Function("the bid o");
            Assert.AreEqual("a:1 b:True c:yellow there time:2012-12-31 12:00:00 AM o:the bid o time2:", returnValue);
        }

        [TestMethod]
        public void Test4()
        {
            var m = Regex.Match("?78", "^([a-zA-Z]+|\\?)([1-9][0-9]*|\\?)$");
            var d = 'A' - '@';
            Assert.IsTrue(m.Success);
        }

        [TestMethod]
        public void Test5()
        {
            var method = "DateCon(0,','' ',\"strinsg,'' )()v\"\"alue  \"\"\"\"    \"    ,true)     ".Trim().TrimEnd(')');
            var parts = Regex.Match(method, "^([^\\(]+)\\(?(.*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var paramst = DelimiteredReader.SplitLine(parts.Groups[2].Value);
            var y = TryParser.Default<bool>("asdfasdf");
            dynamic tryParser = new GenericInvoker(typeof(TryParser));
            var d = tryParser.Default(typeof(bool), paramst[3]);
            Assert.IsTrue(d);
        }

        [Converter]
        public static Func<object, DateTime> GetMethod(string arg)
        {
            return o => DateTime.Now;
        }

        [TestMethod]
        public void TestString()
        {
            var teststring = "-|inside-|";
            var news = teststring.Trim('-', '|');

        }

        [TestMethod]
        public void Testtr()
        {

        }

        [TestMethod]
        public void Test6()
        {
            var thistype = GetType();
            var mi = thistype.GetMethod("GetMethod");
            var o = mi.Invoke(this, new object[] { "helo" });
            //formi.GetParameters()
            var typ = o.GetType();
            var targs = typ.GenericTypeArguments;
            var intt = targs[1];
            var inkmi = o.GetType().GetMethod("Invoke");
            var newd = inkmi.Invoke(o, new object[] { 1 });
            Trace.Write(newd);
        }

        [TestMethod]
        public void TestAgain()
        {
            dynamic model = ClassBuilder
                .Build("ClassA")
                .Property<int>("Prop1")
                .Property<string>("prop2")
                .Property(typeof(decimal), "decprop")
                .ToDynamic();

            dynamic dataContext = ClassBuilder
                .Build("newDbClass", typeof(MyClass))
                .CreateSet((Type)model.Type, "PropA")
                .CreateSet<DataClass2>("Prop2")
                .ToDynamic();

            var migType = typeof(Migration<>).MakeGenericType(dataContext.Type);

            var initType = typeof(MigrateDatabaseToLatestVersion<,>).MakeGenericType(dataContext.Type, migType);
            var initInst = Activator.CreateInstance(initType);


            var mi = typeof(Database).GetMethod("SetInitializer");
            var gmi = mi.MakeGenericMethod(dataContext.Type);
            gmi.Invoke(null, new object[] { initInst });

            var myClass = (MyClass)dataContext;
            myClass.Database.Initialize(true);

            var dataSet = myClass.Set(model.Type);
            model.Amountd = 12.666666789M;
            model.Name = "name 2";
            model.Surname = "surname 2";
            model.Age = 44;
            dataSet.Add(model);
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

    public class Test3Class : DataEntry
    {
        public string DataEntry3 { get; set; }
    }
}
