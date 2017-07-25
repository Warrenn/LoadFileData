using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conventions
{
    public class ConventionsManager : IConventionsManager
    {
        public async Task<T> HydrateInstance<T>(T instance, IDictionary<string, object> dictionary)
        {
            var type = typeof(T);
            var propertyPathDictionary = await Task.FromResult(ConventionsBuilder.GetPropertyPathDictionary(type));
            return await Task.FromResult(PropertyPathProvider.HydrateInstance(instance, dictionary, propertyPathDictionary));
        }
    }
}
