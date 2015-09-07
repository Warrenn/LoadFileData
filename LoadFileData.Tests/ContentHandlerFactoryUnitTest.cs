using System;
using System.Collections.Generic;
using System.Globalization;
using LoadFileData.ContentHandlers;
using LoadFileData.Converters;
using LoadFileData.Tests.MockFactory;
using LoadFileData.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class ContentHandlerFactoryUnitTest
    {
        public class TestClass
        {
            public DateTime Column1 { get; set; }
            public string Column2 { get; set; }
            public int Column3 { get; set; }
        }

        [Converter(Name = "toDate")]
        public static Func<object, object> DateConversion(string format)
        {
            return o => DateTime.ParseExact(o as string, format, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void CreateMustCreateHandlerSucessfully()
        {
            //Arrange
            var helper = new MockHelper();
            var mockTypeMapFactory = helper.Mock<ITypeMapFactory>();
            mockTypeMapFactory
                .Setup(f => f.CreateTypeMap(null))
                .Returns(new Dictionary<string, Type>
                {
                    {"DateType3", typeof (TestClass)}
                });
            var sut = helper.Instance<ContentHandlerFactory>();

            //Act
            var handler = sut.Create("{" +
                       "    \"fixed\": {" +
                       "        \"RemoveWhiteSpace\": true," +
                       "        \"Widths\": [ 2, 4, 5 ]" +
                       "    }," +
                       "    \"indicies\": {" +
                       "        \"column1\": 1," +
                       "        \"column2\": 2," +
                       "        \"column3\": 3" +
                       "    }," +
                       "    \"DateType3\": {" +
                       "        \"column1\": \"toDate('yyy-mmm-dd')\"" +
                       "    }," +
                       "    \"Settings\": {" +
                       "        \"ContentLineNumber\": 2" +
                       "    }" +
                       "}");

            //Assert
            Assert.IsInstanceOfType(handler, typeof(FixedIndexContentHandler));
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void MinimumSettingsMustBeCreatedSucessfully()
        {
            //Arrange
            var helper = new MockHelper();
            var mockTypeMapFactory = helper.Mock<ITypeMapFactory>();
            mockTypeMapFactory
                .Setup(f => f.CreateTypeMap(null))
                .Returns(new Dictionary<string, Type>
                {
                    {"DateType3", typeof (TestClass)}
                });
            var sut = helper.Instance<ContentHandlerFactory>();

            //Act
            var handler = sut.Create("{" +
                       "    \"csv\": {}," +
                       "    \"regex\": {}," +
                       "    \"DateType3\": {}," +
                       "}");

            //Assert
            Assert.IsInstanceOfType(handler, typeof(RegexContentHandler));
            Assert.IsNotNull(handler);
        }
    }
}
