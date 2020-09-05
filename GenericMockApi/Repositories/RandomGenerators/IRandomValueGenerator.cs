using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public interface IRandomValueGenerator<T>
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
