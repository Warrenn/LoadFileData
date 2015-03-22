using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.DAL.Source;

namespace LoadFileData.WCF.Source
{
    [MessageContract]
    public class DataSourceMessageContract
    {
        [MessageHeader(MustUnderstand = true)]
        public DataSource Source { get; set; }

        [MessageBodyMember(Order = 1)] 
        public Stream FileByteStream;
    }
}
