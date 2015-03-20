using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;

namespace LoadFileData.Tests.MockFactory
{
    public class ParentMarkerPolicy : IBuilderPolicy
    {
        private readonly ILifetimeContainer lifetime;

        public ParentMarkerPolicy(ILifetimeContainer lifetime)
        {
            this.lifetime = lifetime;
        }

        public void AddToLifetime(object o)
        {
            lifetime.Add(o);
        }

    }
}
