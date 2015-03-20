using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using LoadFileData.ETLLayer;
using LoadFileData.Tests.MockFactory;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace LoadFileData.Tests.Controllers
{
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

        public abstract class ServiceBase
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
