using GenericMockApi.Repositories.RandomGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenFactory
{
    public interface IRandomGeneratorFactory
    {
        public RandomValueGenerator<double> CreateNumericGenerator(int seed);
        public RandomValueGenerator<string> CreateStringGenerator(int seed);
        public RandomValueGenerator<bool> CreateBooleanGenerator(int seed);
        public RandomValueGenerator<DateTime> CreateDateTimeGenerator(int seed);
        public RandomValueGenerator<int> CreateCollectionSizeGenerator(int seed);
        public AbstractRandomValueGenerator CreateCollectionGenerator(Type T, int seed, int depthLimit);
        public AbstractRandomValueGenerator CreateObjectGenerator(Type T, int seed, int depthLimit);
        public RandomValueGenerator<T> CreateObjectGenerator<T>(int seed, int depthLimit) where T : class;
    }
}
