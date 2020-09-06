using GenericMockApi.Repositories.RandomGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGeneratorFactory
{
    public class RandomGeneratorFactory : IRandomGeneratorFactory
    {
        public IRandomValueGenerator<bool> CreateBoolean(int seed)
        {
            return new BoolRandomValueGenerator(seed);
        }

        public IRandomValueGenerator<int> CreateCollectionSize(int seed)
        {
            return new CollectionSizeGenerator(seed);
        }

        public IRandomValueGenerator<DateTime> CreateDateTime(int seed)
        {
            return new DateTimeRandomValueGenerator(seed);
        }

        public IRandomValueGenerator<double> CreateNumeric(int seed)
        {
            return new DoubleRandomValueGenerator(seed);
        }
        public IRandomValueGenerator<string> CreateString(int seed)
        {
            return new StringRandomValueGenerator(seed);
        }
    }
}
