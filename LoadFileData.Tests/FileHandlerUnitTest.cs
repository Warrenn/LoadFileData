using System;
using System.IO;
using System.Linq;
using System.Threading;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentReaders;
using LoadFileData.DAL;
using LoadFileData.DAL.Models;
using LoadFileData.FileHandlers;
using LoadFileData.Tests.MockFactory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LoadFileData.Tests
{
    [TestClass]
    public class FileHandlerUnitTest
    {
        [TestMethod]
        public void FileHandlerNeedsToProcessStreamSuccessfully()
        {
            //Arrange
            var helper = new MockHelper();
            var entryId = Guid.NewGuid();

            helper
                .Mock<IContentHandler>()
                .Setup(h => h.HandleContent(It.IsAny<ContentHandlerContext>()))
                .Returns(() => new[] {new DataEntry {Id = entryId}})
                .Verifiable();

            helper
                .Mock<IContentReader>()
                .Setup(r => r.RowCount(It.IsAny<Stream>()))
                .Returns(1)
                .Verifiable();

            helper
                .Mock<IServiceFactory>()
                .Setup(f => f.Create())
                .Returns(helper.Instance<IDataService>())
                .Verifiable();

            var settings = helper.Mock<FileHandlerSettings>();
            settings.Object.DestinationPathTemplate = "template";
            settings.Object.ContentHandler = helper.Instance<IContentHandler>();
            settings.Object.Reader = helper.Instance<IContentReader>();
            settings.Object.ServiceFactory = helper.Instance<IServiceFactory>();
            settings.Object.StreamManager = helper.Instance<IStreamManager>();

            var sut = helper.Instance<FileHandler>();
            

            //Act
            sut.ProcessFile("", new MemoryStream(), new CancellationToken());

            //Assert
            //DataService
            var service = helper.Mock<IDataService>();
            service.Verify(srv => srv.UpdateTotalRows(It.IsAny<FileSource>(), It.Is<int>(i => i == 1)));
            service.Verify(srv => srv.AddDataEntry(It.IsAny<FileSource>(), It.Is<DataEntry>(d => d.Id == entryId),It.Is<int>(i => i == 1)));
            service.Verify(srv => srv.PendingExtration(It.IsAny<string>()), Times.Never);
            service.Verify(srv => srv.AddFileSource(It.IsAny<FileSource>()));
            service.Verify(srv => srv.IsDuplicate(It.IsAny<FileSource>()));
            service.Verify(srv => srv.MarkFileComplete(It.IsAny<FileSource>()));
            //ContentReader
            var reader = helper.Mock<IContentReader>();
            reader.Verify(rdr => rdr.ReadContent(It.IsAny<Stream>()));
            reader.VerifyAll();
            //ContentHandler
            var handler = helper.Mock<IContentHandler>();
            handler.VerifyAll();
            //StreamManager
            var strm = helper.Mock<IStreamManager>();
            strm.Verify(s => s.OpenRead(It.IsAny<string>()), Times.Never);
            strm.Verify(s => s.CopyFile(It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void FileHandlerNeedsToPauseWorkflowWhenCancelationRequested()
        {
            //Arrange
            var helper = new MockHelper();
            var entryId = Guid.NewGuid();
            var tokenSource = new CancellationTokenSource();

            helper
                .Mock<IContentHandler>()
                .Setup(h => h.HandleContent(It.IsAny<ContentHandlerContext>()))
                .Callback(() =>
                {
                    tokenSource.Cancel();
                })
                .Returns(() => new[] { new DataEntry { Id = entryId } })
                .Verifiable();

            helper
                .Mock<IContentReader>()
                .Setup(r => r.RowCount(It.IsAny<Stream>()))
                .Returns(1)
                .Verifiable();

            helper
                .Mock<IServiceFactory>()
                .Setup(f => f.Create())
                .Returns(helper.Instance<IDataService>())
                .Verifiable();

            var settings = helper.Mock<FileHandlerSettings>();
            settings.Object.DestinationPathTemplate = "template";
            settings.Object.ContentHandler = helper.Instance<IContentHandler>();
            settings.Object.Reader = helper.Instance<IContentReader>();
            settings.Object.ServiceFactory = helper.Instance<IServiceFactory>();
            settings.Object.StreamManager = helper.Instance<IStreamManager>();

            var sut = helper.Instance<FileHandler>();


            //Act
            sut.ProcessFile("", new MemoryStream(), tokenSource.Token);

            //Assert
            helper.Mock<IDataService>().Verify(srv => srv.MarkFilePaused(It.IsAny<FileSource>()));
        }

        [TestMethod]
        public void FileHandlerNeedsToRecoverPausedWorkflows()
        {
            //Arrange
            var helper = new MockHelper();
            var entryId = Guid.NewGuid();
            var fileHash = Guid.NewGuid().ToString();

            helper
                .Mock<IContentHandler>()
                .Setup(h => h.HandleContent(It.IsAny<ContentHandlerContext>()))
                .Returns(() => new[] { new DataEntry { Id = entryId } })
                .Verifiable();

            helper
                .Mock<IDataService>()
                .Setup(d => d.PendingExtration("settingsName"))
                .Returns(new[] {new FileSource {FileHash = fileHash,CurrentFileName = fileHash}}.AsQueryable())
                .Verifiable();

            helper
                .Mock<IServiceFactory>()
                .Setup(f => f.Create())
                .Returns(helper.Instance<IDataService>())
                .Verifiable();

            var settings = helper.Mock<FileHandlerSettings>();
            settings.Object.Name = "settingsName";
            settings.Object.DestinationPathTemplate = "template";
            settings.Object.ContentHandler = helper.Instance<IContentHandler>();
            settings.Object.Reader = helper.Instance<IContentReader>();
            settings.Object.ServiceFactory = helper.Instance<IServiceFactory>();
            settings.Object.StreamManager = helper.Instance<IStreamManager>();

            var sut = helper.Instance<FileHandler>();


            //Act
            sut.RecoverExistingFiles(new CancellationToken());

            //Assert
            //DataService
            var service = helper.Mock<IDataService>();
            service.Verify(srv => srv.UpdateTotalRows(It.IsAny<FileSource>(), It.IsAny<int>()),Times.Never);
            service.Verify(srv => srv.AddDataEntry(It.Is<FileSource>(f=>f.FileHash==fileHash && f.CurrentFileName==fileHash), It.Is<DataEntry>(d => d.Id == entryId), It.Is<int>(i => i == 1)));
            service.Verify(srv => srv.AddFileSource(It.IsAny<FileSource>()), Times.Never);
            service.Verify(srv => srv.IsDuplicate(It.IsAny<FileSource>()), Times.Never);
            service.Verify(srv => srv.MarkFileExtracting(It.Is<FileSource>(f=>f.FileHash == fileHash)));
            service.Verify(srv => srv.MarkFileComplete(It.Is<FileSource>(f=>f.FileHash == fileHash)));
            service.VerifyAll();
            //ContentReader
            var reader = helper.Mock<IContentReader>();
            reader.Verify(rdr => rdr.ReadContent(It.IsAny<Stream>()));
            reader.Verify(rdr => rdr.RowCount(It.IsAny<Stream>()), Times.Never);
            //ContentHandler
            var handler = helper.Mock<IContentHandler>();
            handler.VerifyAll();
            //StreamManager
            var strm = helper.Mock<IStreamManager>();
            strm.Verify(s => s.OpenRead(It.Is<string>(st => st == fileHash)));
            strm.Verify(s => s.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}