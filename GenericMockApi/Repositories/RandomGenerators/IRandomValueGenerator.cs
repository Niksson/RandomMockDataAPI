using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    // There are two approaches to generating a value of any type:
    // generics and converting to object.
    // Converting to object and casting to other types (boxing and unboxing) is SLOW
    // so I use generic random generators. This produces not so clean code to later
    // generate the value itself, but it is faster than boxing/unboxing

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
