using System.Collections.Generic;

namespace LoadFileData.Transformer
{
    public interface ITransformer<out TOut, in TIn>
    {
        IEnumerable<TOut> Transform(IEnumerable<TIn> input);
    }
}
