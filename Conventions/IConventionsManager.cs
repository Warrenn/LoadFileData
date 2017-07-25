using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conventions
{
    public interface IConventionsManager
    {
        Task<T> HydrateInstance<T>(T instance, IDictionary<string, object> dictionary);
    }
}