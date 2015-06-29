﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
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
            var qouteStrings = settings.CommentStrings.ToArray();

            using (var reader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if ((string.IsNullOrEmpty(line)) ||
                        (qouteStrings.Any(line.StartsWith)))
                    {
                        continue;
                    }
                    line = (settings.RemoveWhiteSpace) ? line.Trim() : line;
                    yield return ReadRowValues(line);
                }
            }
        }
    }
}
