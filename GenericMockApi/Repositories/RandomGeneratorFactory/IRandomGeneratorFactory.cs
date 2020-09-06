using GenericMockApi.Repositories.RandomGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGeneratorFactory
{
    public interface IRandomGeneratorFactory
    {
        public IRandomValueGenerator<double> CreateNumeric(int seed);
        public IRandomValueGenerator<string> CreateString(int seed);
        public IRandomValueGenerator<bool> CreateBoolean(int seed);
        public IRandomValueGenerator<DateTime> CreateDateTime(int seed);
        public IRandomValueGenerator<int> CreateCollectionSize(int seed);
    }
}
