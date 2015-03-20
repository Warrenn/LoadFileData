using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.ETLLayer.ContentHandler;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace LoadFileData.ETLLayer.FileHandler
{
    public class FileHandlerBase : IFileHandler
    {
        private IContentHandler[] ContentHandlers;

        public FileHandlerBase()
        {
            //for each settings.ContentHandlerSettings
        }

        #region IFileHandler Members

        public void ProcessFile(string fullPath, Stream stream)
        {
            throw new NotImplementedException();
        }

        public void ProcessComplete(string fullPath)
        {
            throw new NotImplementedException();
        }

        public void ReportError(string fullPath, Exception exception)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
