using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.Transform
{
    public interface ITransformer<out TOut, in TIn>
    {
        IEnumerable<TOut> Transform(IEnumerable<TIn> input);
    }
}
