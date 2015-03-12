using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IExcelSettings
    {
        string SheetName { set; }
        int SheetNumber { set; }
        int HeaderLineNumber { set; }
        int ContentLineNumber { set; }
    }
}
