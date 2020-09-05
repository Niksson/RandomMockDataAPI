﻿using GenericMockApi.Repositories.RandomGenerators;
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

        public IRandomValueGenerator<DateTime> CreateDateTime(int seed)
        {
            return new DateTimeRandomValueGenerator(seed);
        }

        public IRandomValueGenerator<T> CreateNumeric<T>(int seed)
        {
            return new NumericRandomValueGenerator<T>(seed);
        }

        public IRandomValueGenerator<string> CreateString(int seed)
        {
            return new StringRandomValueGenerator(seed);
        }
    }
}