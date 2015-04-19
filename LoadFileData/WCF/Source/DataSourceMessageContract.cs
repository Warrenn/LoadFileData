using System.IO;
using System.ServiceModel;
using LoadFileData.DAL.Source;

namespace LoadFileData.WCF.Source
{
    [MessageContract]
    public class DataSourceMessageContract
    {
        [MessageHeader(MustUnderstand = true)]
        public string UserName { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string OriginalFileName { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public virtual string HandlerName { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public virtual string MediaType { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }
}
