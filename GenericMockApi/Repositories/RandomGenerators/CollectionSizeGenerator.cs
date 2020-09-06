using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class CollectionSizeGenerator : IRandomValueGenerator<int>
    {
        private readonly Random _generator;

        public CollectionSizeGenerator(int seed)
        {
            _generator = new Random(seed);
        }

        public int GetNext()
        {
            return _generator.Next(2, 6);
        }
    }
}
