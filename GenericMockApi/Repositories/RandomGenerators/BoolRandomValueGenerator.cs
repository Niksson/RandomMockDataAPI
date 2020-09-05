using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class BoolRandomValueGenerator : IRandomValueGenerator<bool>
    {
        private readonly Random _generator;

        public BoolRandomValueGenerator(int seed)
        {
            _generator = new Random(seed);
        }

        public bool GetNext()
        {
            // There are different methods to generate a bool value,
            // so let's choose the first one that comes to head
            return _generator.Next(2) == 1;
        }
    }
}
