using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class DoubleRandomValueGenerator : IRandomValueGenerator<double>
    {
        private readonly Random _generator;

        public DoubleRandomValueGenerator(int seed)
        {
            _generator = new Random(seed);
        }

        public double GetNext()
        {
            var value = _generator.NextDouble() * 1000;

            return value;
        }

    }
}
