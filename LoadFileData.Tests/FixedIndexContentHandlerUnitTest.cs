using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentHandlers.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class FixedIndexContentHandlerUnitTest
    {
        public class ContentData
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
        }

        [TestMethod]
        public void FieldValuesAreFoundByIndex()
        {
            //Arrange
            var settings = new FixedIndexSettings(typeof (ContentData))
            {
                FieldIndices = new Dictionary<int, string>
                {
                    {3, "Index"},
                    {1, "Name"},
                    {2, "Value"}
                }
            };
            var expectedValues = new[]
            {
                new ContentData {Value = 12.3, Index = 1, Name = "name"},
                new ContentData {Value = 19.7, Index = 14, Name = "name value"},
                new ContentData {Value = 1.278, Index = 7, Name = "value"},
            };
            var handler = new FixedIndexContentHandler(settings);
            var context = new ContentHandlerContext
            {
                FileName = "test",
                Content = new[]
                {
                    new object[] {"name", 12.3, 1},
                    new object[] {"name value", 19.7, 14},
                    new object[] {"value", 1.278, 7},
                }
            };

            //Act
            var values = handler.HandleContent(context).Cast<ContentData>().ToArray();

            //Assert
            Assert.AreEqual(3, values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(expectedValues[i].Index, values[i].Index);
                Assert.AreEqual(expectedValues[i].Name, values[i].Name);
                Assert.AreEqual(expectedValues[i].Value, values[i].Value);
            }
        }
    }
}
