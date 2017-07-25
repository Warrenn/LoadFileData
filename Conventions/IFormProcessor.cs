using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Conventions
{
    public interface IFormProcessor
    {
        Task<IDictionary<string, object>> ReadFormValuesAsync(Stream stream);
    }
}
