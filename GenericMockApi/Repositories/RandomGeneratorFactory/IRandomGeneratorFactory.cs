using GenericMockApi.Repositories.RandomGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenFactory
{
    public interface IRandomGeneratorFactory
    {
        public IRandomValueGenerator<double> CreateNumericGenerator(int seed);
        public IRandomValueGenerator<string> CreateStringGenerator(int seed);
        public IRandomValueGenerator<bool> CreateBooleanGenerator(int seed);
        public IRandomValueGenerator<DateTime> CreateDateTimeGenerator(int seed);
        public IRandomValueGenerator<int> CreateCollectionSizeGenerator(int seed);
        public IRandomValueGenerator CreateCollectionGenerator(Type T, int seed, int depthLimit);
        public IRandomValueGenerator CreateObjectGenerator(Type T, int seed, int depthLimit);
    }
}
