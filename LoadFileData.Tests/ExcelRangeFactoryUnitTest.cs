using System;
using LoadFileData.ContentReaders.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class ExcelRangeFactoryUnitTest
    {

        [TestMethod]
        public void LettersRepresentColumnNumbers()
        {
            //Arrange
            //Act
            var range = ExcelRangeFactory.CreateRange("A1:Z9");

            //Assert
            Assert.AreEqual(1, range.RowStart);
            Assert.AreEqual(1, range.ColumnStart);
            Assert.AreEqual(9, range.RowEnd);
            Assert.AreEqual(26, range.ColumnEnd);
        }

        [TestMethod]
        public void ColumnLettersCouldBeMoreThanOne()
        {
            //Arrange
            //Act
            var range = ExcelRangeFactory.CreateRange("ABC123:ZYX987");

            //Assert
            Assert.AreEqual(123, range.RowStart);
            Assert.AreEqual(731, range.ColumnStart);
            Assert.AreEqual(987, range.RowEnd);
            Assert.AreEqual(18250, range.ColumnEnd);
        }

        [TestMethod]
        public void QuestionMarkForEndRangeColumnResultInNull()
        {
            //Arrange
            //Act
            var range = ExcelRangeFactory.CreateRange("A1:Z?");

            //Assert
            Assert.AreEqual(1, range.RowStart);
            Assert.AreEqual(1, range.ColumnStart);
            Assert.IsNull(range.RowEnd);
            Assert.AreEqual(26, range.ColumnEnd);
        }

        [TestMethod]
        public void QuestionMarkForEndRangeRowResultInNull()
        {
            //Arrange
            //Act
            var range = ExcelRangeFactory.CreateRange("A1:?9");

            //Assert
            Assert.AreEqual(1, range.RowStart);
            Assert.AreEqual(1, range.ColumnStart);
            Assert.AreEqual(9, range.RowEnd);
            Assert.IsNull(range.ColumnEnd);
        }

        [TestMethod]
        public void QuestionMarkForEndRangeResultInNullRowAndNullColumn()
        {
            //Arrange
            //Act
            var range = ExcelRangeFactory.CreateRange("A1:?");

            //Assert
            Assert.AreEqual(1, range.RowStart);
            Assert.AreEqual(1, range.ColumnStart);
            Assert.IsNull(range.RowEnd);
            Assert.IsNull(range.ColumnEnd);
        }

        [TestMethod]
        public void EmptyEndRangeResultInNullRowAndNullColumn()
        {
            //Arrange
            //Act
            var range = ExcelRangeFactory.CreateRange("A1");

            //Assert
            Assert.AreEqual(1, range.RowStart);
            Assert.AreEqual(1, range.ColumnStart);
            Assert.IsNull(range.RowEnd);
            Assert.IsNull(range.ColumnEnd);
        }
    }
}
