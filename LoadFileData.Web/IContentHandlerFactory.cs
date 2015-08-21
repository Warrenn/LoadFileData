using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.ContentHandlers;
using LoadFileData.FileHandlers;

namespace LoadFileData.Web
{
    public interface IContentHandlerFactory
    {
        IContentHandler Create(string jsonData);
    }
}
