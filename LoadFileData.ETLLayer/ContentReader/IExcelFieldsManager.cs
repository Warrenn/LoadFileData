using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IExcelFieldsManager : IRegexFieldsManager
    {
        void SetSheetName(string sheetName);
        void SetSheetNumber(int sheetNumber);
        void SetHeaderLineNumber(int lineNumber);
        void SetContentLineNumber(int lineNumber);

    }
}
