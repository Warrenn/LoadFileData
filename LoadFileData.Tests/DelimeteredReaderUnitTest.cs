using System;
using LoadFileData.ContentReaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class DelimeteredReaderUnitTest
    {

        [TestMethod]
        public void TrimMustRemoveCommentsAsWellAsWhiteSpace()
        {
           //Arrange
            var testString = " ' comment   '   ,   \"  anotherComment    \"   ";

            //Act
            var parts = DelimiteredReader.Split(testString, true);

            //Assert
            Assert.AreEqual(2, parts.Length);
            Assert.AreEqual(" comment   ", parts[0]);
            Assert.AreEqual("  anotherComment    ", parts[1]);
        }

        [TestMethod]
        public void DisabledTrimMustKeepCommentsAsWellAsWhiteSpace()
        {
            //Arrange
            var testString = " ' comment   '   ,   \"  anotherComment    \"   ";

            //Act
            var parts = DelimiteredReader.Split(testString, false);

            //Assert
            Assert.AreEqual(2, parts.Length);
            Assert.AreEqual(" ' comment   '   ", parts[0]);
            Assert.AreEqual("   \"  anotherComment    \"   ", parts[1]);
        }

        [TestMethod]
        public void AllowCommentsToBeIncludedInsideOfStrings()
        {
            //Arrange
            var testString = @" ' """"comment""""   '   ,   ""  anoth""""erCo""""mment    ""   ";

            //Act
            var parts = DelimiteredReader.Split(testString);

            //Assert
            Assert.AreEqual(2, parts.Length);
            Assert.AreEqual(" \"comment\"   ", parts[0]);
            Assert.AreEqual("  anoth\"erCo\"mment    ", parts[1]);
        }

        [TestMethod]
        public void DelimitersBetweenCommentsMustNotSplitString()
        {
            //Arrange
            var testString = @" ' """"co,m|ment""""   '   ,   ""  an|other,Comment|    ""   ";

            //Act
            var parts = DelimiteredReader.Split(testString);

            //Assert
            Assert.AreEqual(2, parts.Length);
            Assert.AreEqual(" \"co,m|ment\"   ", parts[0]);
            Assert.AreEqual("  an|other,Comment|    ", parts[1]);
        }

    }
}
