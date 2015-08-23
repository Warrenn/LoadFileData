using System;
using System.IO;
using System.Linq;
using LoadFileData.ContentReaders;
using LoadFileData.ContentReaders.Settings;
using LoadFileData.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class XlsReaderUnitTest
    {
        [TestMethod]
        public void ReaderNeedsToReadBinaryXlsFiles()
        {
            //Arrange
            var settings = new ExcelSettings();
            var reader = new XlsContentReader(settings);
            var memoryStream = new MemoryStream(Resources.Test_xls);
            var expected = new[]
            {
                new object[] {"Field1", "Field2", "Field3"},
                new object[] {38477D, "1.1234", "asdfasdf"},
                new object[] {38874D, "9.0001", "Hello"},
                new object[] {39702.0D, "12.43", "text value"}
            };

            //Act
            var actual = reader.ReadContent(memoryStream).ToArray();

            //Assert
            Assert.AreEqual(expected.Length, actual.Length);
            for (var i = 0; i < actual.Length; i++)
            {
                var subArray = actual[i].ToArray();
                var expectedSub = expected[i];
                for (var ii = 0; ii < subArray.Length; ii++)
                {
                    Assert.AreEqual(expectedSub[ii], subArray[ii]);
                }
            }
        }

        [TestMethod]
        public void ReaderNeedsToGetCorrectCountOfRows()
        {
            //Arrange
            var settings = new ExcelSettings();
            var reader = new XlsContentReader(settings);
            var memoryStream = new MemoryStream(Resources.Test_xls);
            
            //Act
            var actual = reader.RowCount(memoryStream);

            //Assert
            Assert.AreEqual(4, actual);
        }

    }
}
