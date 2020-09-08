using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class DoubleRandomValueGenerator : RandomValueGenerator<double>
    {
        private readonly Random _generator;

        public DoubleRandomValueGenerator(int seed)
        {
            _generator = new Random(seed);
        }

        public override double GetNext()
        {
            var value = _generator.NextDouble() * 1000;

            return value;
        }

    }
}
