using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMockDataAPI.Repositories.RandomGenerators
{
    public class DoubleRandomValueGenerator : PrimitiveRandomValueGenerator<double>
    {

        public DoubleRandomValueGenerator(int seed) : base(seed)
        {
        }

        protected override double GenerateRandomValue()
        {
            var value = _generator.NextDouble() * 1000;

            return value;
        }

    }
}
