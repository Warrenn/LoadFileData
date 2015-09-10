using System;
using System.Collections.Generic;
using LoadFileData.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class ConverterManagerUnitTest
    {
        private static bool testMethodCalled;
        private static bool testParamsMethodCalled;
        private static bool methodNameCalled;


        [Converter]
        public static Func<object, object> TestMethod()
        {
            testMethodCalled = true;
            return o => o;
        }

        [Converter(Name = "ConvertMe")]
        public static Func<object, string> Convert(int a, bool? b, string c, DateTime time, DateTime? time2)
        {
            return o => $"a:{a} b:{b} c:{c} time:{time} o:{o} time2:{time2}";
        }

        [Converter]
        public static Func<object, object> TestParamsMethod(DateTime date, string value)
        {
            if ((date == new DateTime(2005, 5, 5)) &&
                (value == "string value"))
            {
                testParamsMethodCalled = true;
            }
            return o => o;
        }

        [Converter(Name = "DifferentName")]
        public static Func<object, object> MethodName()
        {
            methodNameCalled = true;
            return o => o;
        }

        [TestMethod]
        public void DefaultTypesShouldBeFound()
        {
            //Arrange
            IDictionary<string, Type> expectedMap =
                new SortedDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {"byte", typeof (byte)},
                    {"char", typeof (char)},
                    {"int", typeof (int)},
                    {"long", typeof (long)},
                    {"decimal", typeof (decimal)},
                    {"float", typeof (float)},
                    {"bool", typeof (bool)},
                    {"DateTime", typeof (DateTime)},
                    {"Date", typeof (DateTime)},
                    {"Date?", typeof (DateTime?)},
                    {"string", typeof (string)},
                    {"byte?", typeof (byte?)},
                    {"char?", typeof (char?)},
                    {"int?", typeof (int?)},
                    {"long?", typeof (long?)},
                    {"decimal?", typeof (decimal?)},
                    {"float?", typeof (float?)},
                    {"bool?", typeof (bool?)},
                    {"DateTime?", typeof (DateTime?)}
                };

            foreach (var pair in expectedMap)
            {
                //Act
                var actual = ConverterManager.LookupType(pair.Key);

                //Assert
                Assert.AreEqual(pair.Value, actual);
            }
        }

        [TestMethod]
        public void CallMethodLoadedByReflection()
        {
            //Arrange
            testMethodCalled = false;

            //Act
            ConverterManager.GetConverter("TestMethod");

            //Assert
            Assert.IsTrue(testMethodCalled);
        }

        [TestMethod]
        public void ConverterMustMapParameters()
        {
            //Arrange
            testParamsMethodCalled = false;

            //Act
            ConverterManager.GetConverter("TestParamsMethod('2005-05-05','string value')");

            //Assert
            Assert.IsTrue(testParamsMethodCalled);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void IncorrectNumberOfParametersMustThrow()
        {
            //Arrange
            testParamsMethodCalled = false;

            //Act
            ConverterManager.GetConverter("TestParamsMethod('2005-05-05','string value','throw')");

            //Assert
            Assert.Fail();
        }

        [TestMethod]
        public void InvokeCorrectMethod()
        {
            //Arrange
            var expected = Guid.NewGuid();
            var converter = ConverterManager.GetConverter("TestMethod");

            //Act
            var actual = converter.Function(expected);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MethodNameCanBeSetFromAttribute()
        {
            //Arrange
            methodNameCalled = false;

            //Act
            var converter = ConverterManager.GetConverter("DifferentName");

            //Assert
            Assert.IsNotNull(converter);
            Assert.IsTrue(methodNameCalled);
        }

        [TestMethod]
        public void ReflectionMustInterpretReturnTypeAndParametersCorrectly()
        {
            //Arrange
            var converter = ConverterManager.GetConverter("ConvertMe(1,true,'yellow there','2012-12-31',\"asdfasdf\")");

            //Act
            var returnValue = converter.Function("the bid o");

            //Assert
            Assert.AreEqual(typeof(string), converter.ReturnType);
            Assert.AreEqual("a:1 b:True c:yellow there time:2012-12-31 12:00:00 AM o:the bid o time2:", returnValue);
        }

    }
}
