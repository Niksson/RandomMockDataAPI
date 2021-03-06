﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomMockDataAPI.Repositories.RandomGenerators;

namespace RandomMockDataAPI.Repositories.RandomGenFactory
{
    public class RandomGeneratorFactory : IRandomGeneratorFactory
    {
        public RandomValueGenerator<bool> CreateBooleanGenerator(int seed)
        {
            return new BoolRandomValueGenerator(seed);
        }

        public RandomValueGenerator<int> CreateCollectionSizeGenerator(int seed)
        {
            return new CollectionSizeGenerator(seed);
        }

        public RandomValueGenerator<DateTime> CreateDateTimeGenerator(int seed)
        {
            return new DateTimeRandomValueGenerator(seed);
        }

        public RandomValueGenerator<double> CreateNumericGenerator(int seed)
        {
            return new DoubleRandomValueGenerator(seed);
        }
        public RandomValueGenerator<string> CreateStringGenerator(int seed)
        {
            return new StringRandomValueGenerator(seed);
        }

        // TODO: If things go wrong, consider returning skip variable
        public RandomValueGenerator CreateCollectionGenerator(Type T, int seed, uint depthLimit)
        {
            var type = typeof(RandomCollectionGenerator<>).MakeGenericType(T);
            var generator = (RandomValueGenerator)Activator.CreateInstance(type, seed, depthLimit);
            return generator;
        }

        public RandomValueGenerator CreateObjectGenerator(Type T, int seed, uint depthLimit)
        {
            var type = typeof(RandomObjectGenerator<>).MakeGenericType(T);
            var generator = (RandomValueGenerator)Activator.CreateInstance(type, seed, depthLimit);
            return generator;
        }

        public RandomValueGenerator<T> CreateObjectGenerator<T>(int seed, uint depthLimit) where T : class
        {
            return new RandomObjectGenerator<T>(seed, depthLimit);
        }
    }
}
