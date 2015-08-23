using System;
using System.Linq;
using LoadFileData.ContentReaders;
using LoadFileData.ContentReaders.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class FixedWidthReaderUnitTest
    {
        [TestMethod]
        public void StringsMustBeSplitByFieldWidths()
        {
            //Arrange
            var settings = new FixedWidthSettings
            {
                FieldWidths = new[] {17, 2, 9, 13},
                RemoveWhiteSpace = false
            };
            var reader = new FixedWidthReader(settings);
            var line = "ab3456789abcd 234";

            //Act
            var parts = reader.ReadRowValues(line).ToArray();

            //Assert
            Assert.AreEqual("ab", parts[0]);
            Assert.AreEqual("3456789", parts[1]);
            Assert.AreEqual("abcd", parts[2]);
            Assert.AreEqual(" 234", parts[3]);
        }

        [TestMethod]
        public void TrimMustRemoveWhiteSpace()
        {
            //Arrange
            var settings = new FixedWidthSettings
            {
                FieldWidths = new[] { 17, 2, 9, 13 },
                RemoveWhiteSpace = true
            };
            var reader = new FixedWidthReader(settings);
            var line = "ab  567  abcd\t\t34";

            //Act
            var parts = reader.ReadRowValues(line).ToArray();

            //Assert
            Assert.AreEqual("ab", parts[0]);
            Assert.AreEqual("567", parts[1]);
            Assert.AreEqual("abcd", parts[2]);
            Assert.AreEqual("34", parts[3]);
        }
    }
}
