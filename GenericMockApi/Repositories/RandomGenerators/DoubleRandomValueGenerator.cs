using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class DoubleRandomValueGenerator : PrimitiveRandomValueGenerator<double>
    {

        public DoubleRandomValueGenerator(int seed) : base(seed)
        {
        }

        public override double GetNext()
        {
            var value = _generator.NextDouble() * 1000;

            return value;
        }

    }
}
