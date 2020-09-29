using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomMockDataAPI.Repositories.RandomGenerators;

namespace RandomMockDataAPI.Repositories.RandomGenFactory
{
    public interface IRandomGeneratorFactory
    {
        public RandomValueGenerator<double> CreateNumericGenerator(int seed);
        public RandomValueGenerator<string> CreateStringGenerator(int seed);
        public RandomValueGenerator<bool> CreateBooleanGenerator(int seed);
        public RandomValueGenerator<DateTime> CreateDateTimeGenerator(int seed);
        public RandomValueGenerator<int> CreateCollectionSizeGenerator(int seed);
        public RandomValueGenerator CreateCollectionGenerator(Type T, int seed, uint depthLimit);
        public RandomValueGenerator CreateObjectGenerator(Type T, int seed, uint depthLimit);
        public RandomValueGenerator<T> CreateObjectGenerator<T>(int seed, uint depthLimit) where T : class;
    }
}
