using System;
using System.Collections.Generic;

namespace Conventions
{
    public class DynamicEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T, T, bool> func;

        public DynamicEqualityComparer(Func<T, T, bool> func)
        {
            this.func = func;
        }

        public bool Equals(T x, T y) => func(x, y);

        public int GetHashCode(T obj) => 0;
    }
}
