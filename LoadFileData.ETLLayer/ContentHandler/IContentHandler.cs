﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public interface IContentHandler
    {
        void ProcessRowData(IDictionary<string, object> rowData);
    }
}