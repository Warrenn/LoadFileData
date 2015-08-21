using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.ContentReaders;

namespace LoadFileData.Web
{
    public interface IContentReaderFactory
    {
        IContentReader Create(string jsonData);
    }
}
