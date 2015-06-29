using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.Transformer
{
    public class TransFormActivity<TOut, TIn> : ITransformer<TOut, TIn>
    {

        public IEnumerable<TOut> Transform(IEnumerable<TIn> input)
        {
            //Persist each entry
            //Validate each entry
            //if noerrors or continueOnErrors
            return (IEnumerable<TOut>) input;
        }
    }
}
