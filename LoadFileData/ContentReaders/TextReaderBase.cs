using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoadFileData.ContentReaders.Settings;

namespace LoadFileData.ContentReaders
{
    public abstract class TextReaderBase : IContentReader
    {
        private readonly TextReaderSettings settings;

        protected TextReaderBase(TextReaderSettings settings)
        {
            this.settings = settings;
        }

        public abstract IEnumerable<string> ReadRowValues(string line);

        public IEnumerable<IEnumerable<object>> ReadContent(Stream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if ((string.IsNullOrEmpty(line)))
                    {
                        continue;
                    }
                    yield return ReadRowValues(line);
                }
            }
        }


        public int RowCount(Stream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            var rowCount = 1;
            using (var reader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                while (!reader.EndOfStream)
                {
                    rowCount++;
                }
            }
            return rowCount;
        }
    }
}
