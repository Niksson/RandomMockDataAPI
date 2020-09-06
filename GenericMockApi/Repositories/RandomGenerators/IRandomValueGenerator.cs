using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    // I know that this may be not the best solution, but
    // until I find a better way, let's leave it as is

    /// <summary>
    /// A non-generic helper interface to use with collections
    /// </summary>
    public interface IRandomValueGenerator
    { 
    }

    /// <summary>
    /// A generic interface defining methods for generating random values of type T
    /// </summary>
    /// <typeparam name="T">A type T values of which to generate</typeparam>
    public interface IRandomValueGenerator<T> : IRandomValueGenerator
    {
        public T GetNext();

        // Default implementations that should be the same between types

        public IRandomValueGenerator<T> Skip(int skip)
        {
            for (var i = 0; i < skip; i++)
            {
                // Throwing the next random value to the depth of /dev/null
                GetNext();
            }
            return this;
        }
        public IEnumerable<T> GetValues(int take)
        {
            var values = new List<T>();
            for (int i = 0; i < take; i++)
            {
                values.Add(GetNext());
            }
            return values;
        }
    }
}
