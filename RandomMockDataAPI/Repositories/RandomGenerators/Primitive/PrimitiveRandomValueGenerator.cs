using System;

namespace RandomMockDataAPI.Repositories.RandomGenerators
{
    public abstract class PrimitiveRandomValueGenerator<T> : RandomValueGenerator<T>
    {
        protected Random _generator;

        protected PrimitiveRandomValueGenerator(int seed) : base(seed)
        {
            _generator = new Random(seed);
        }

        protected override void IncrementSeed()
        {
            _seed++;
            _generator = new Random(_seed);
        }

        protected override void SetSeed(int seed)
        {
            _generator = new Random(seed);
        }
    }
}