using GenericMockApi.Repositories.RandomGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenFactory
{
    public class RandomGeneratorFactory : IRandomGeneratorFactory
    {
        public IRandomValueGenerator<bool> CreateBooleanGenerator(int seed)
        {
            return new BoolRandomValueGenerator(seed);
        }

        public IRandomValueGenerator<int> CreateCollectionSizeGenerator(int seed)
        {
            return new CollectionSizeGenerator(seed);
        }

        public IRandomValueGenerator<DateTime> CreateDateTimeGenerator(int seed)
        {
            return new DateTimeRandomValueGenerator(seed);
        }

        public IRandomValueGenerator<double> CreateNumericGenerator(int seed)
        {
            return new DoubleRandomValueGenerator(seed);
        }
        public IRandomValueGenerator<string> CreateStringGenerator(int seed)
        {
            return new StringRandomValueGenerator(seed);
        }

        // TODO: If things go wrong, consider returning skip variable
        public IRandomValueGenerator CreateCollectionGenerator(Type T, int seed, int depthLimit)
        {
            var type = typeof(RandomCollectionGenerator<>).MakeGenericType(T);
            var generator = (IRandomValueGenerator)Activator.CreateInstance(type, seed, depthLimit);
            return generator;
        }

        public IRandomValueGenerator CreateObjectGenerator(Type T, int seed, int depthLimit)
        {
            var type = typeof(RandomObjectGenerator<>).MakeGenericType(T);
            var generator = (IRandomValueGenerator)Activator.CreateInstance(type, seed, depthLimit);
            return generator;
        }
    }
}
