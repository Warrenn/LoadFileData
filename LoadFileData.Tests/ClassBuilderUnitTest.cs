using System;
using LoadFileData.DAL;
using LoadFileData.DAL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests
{
    [TestClass]
    public class ClassBuilderUnitTest
    {
        [TestMethod]
        public void ClassBuilderMustBeAbleToCreateSameTypeNameMultipleTimes()
        {
            //Arrange
            var baseBuilder = ClassBuilder.CreateBaseBuilder();
            var basebuilder2 = ClassBuilder.CreateBaseBuilder();

            var builder = ClassBuilder
                .Build("classA")
                .Property<int>("a");

            var builder2 = ClassBuilder
                .Build("classA")
                .Property<string>("a");

            //Act
            var t1 = builder.ToType(baseBuilder);

            var t2 = builder2.ToType(basebuilder2);

            //Assert
            Assert.AreNotEqual(t1, t2);
        }

        [TestMethod]
        public void DataContextCanBeRecreatedWithDifferentDbSets()
        {
            //Arrange
            var baseBuilder = ClassBuilder.CreateBaseBuilder();
            var context1 = ClassBuilder
                .Build<DbContextBase>("WebDbContext");

            var builder = ClassBuilder
                .Build("classA",typeof(DataEntry))
                .Property<int>("a");

            var t1 = builder.ToType(baseBuilder);

            var db1 = context1.CreateSet(t1, "t1");

            var basebuilder2 = ClassBuilder.CreateBaseBuilder();
            var context2 = ClassBuilder
                .Build<DbContextBase>("WebDbContext");

            var builder2 = ClassBuilder
                .Build("classA")
                .Property<string>("a");

            var t2 = builder2.ToType(basebuilder2);

            var db2 = context2.CreateSet(t2, "t1");

            //Act
            var db1Inst = db1.ToInstance(baseBuilder);
            var db2Inst = db2.ToInstance(basebuilder2);

            //Assert
            Assert.AreNotEqual(db1Inst, db2Inst);
        }
    }
}
