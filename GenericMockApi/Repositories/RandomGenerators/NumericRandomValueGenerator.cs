using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class NumericRandomValueGenerator<T> : IRandomValueGenerator<T>
    {
        private readonly Random _generator;

        public NumericRandomValueGenerator(int seed)
        {
            _generator = new Random(seed);
        }

        public T GetNext()
        {
            var value = _generator.NextDouble() * 1000;

            return (T)Activator.CreateInstance(typeof(T), value);
        }

    }
}
