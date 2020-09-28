using System;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public abstract class PrimitiveRandomValueGenerator<T> : RandomValueGenerator<T>
    {
        protected Random _generator;

        protected PrimitiveRandomValueGenerator(int seed) : base(seed)
        {
            _generator = new Random(seed);
        }
    }
}