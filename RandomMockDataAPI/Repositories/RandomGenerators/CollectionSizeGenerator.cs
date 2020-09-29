using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMockDataAPI.Repositories.RandomGenerators
{
    public class CollectionSizeGenerator : RandomValueGenerator<int>
    {
        private readonly Random _generator;

        public CollectionSizeGenerator(int seed) : base(seed)
        {
            _generator = new Random(seed);
        }

        public override int GetNext()
        {
            return _generator.Next(2, 6);
        }
    }
}
