﻿using GenericMockApi.Repositories.RandomGenFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    // There are two approaches to generating a value of any type:
    // generics and converting to object.
    // Converting to object and casting to other types (boxing and unboxing) is SLOW
    // so I use generic random generators. This produces not so clean code to later
    // generate the value itself, but it is faster than boxing/unboxing

    /// <summary>
    /// A non-generic helper interface to use with collections
    /// </summary>
    public abstract class AbstractRandomValueGenerator
    {
        
    }

    /// <summary>
    /// A generic interface defining methods for generating random values of type T
    /// </summary>
    /// <typeparam name="T">A type T values of which to generate</typeparam>
    public abstract class RandomValueGenerator<T> : AbstractRandomValueGenerator
    {
        protected int _seed;
        protected Random _generator;

        public RandomValueGenerator(int seed)
        {
            _seed = seed;
            _generator = new Random(seed);
        }

        public abstract T GetNext();

        // Default implementations that should be the same between types

        public virtual RandomValueGenerator<T> Skip(int skip)
        {
            for (var i = 0; i < skip; i++)
            {
                // Throwing the next random value to the depth of /dev/null
                GetNext();
            }
            return this;
        }
        public IEnumerable<T> GetValues(int take)
        {
            var values = new List<T>();
            for (int i = 0; i < take; i++)
            {
                values.Add(GetNext());
            }
            return values;
        }

        public virtual RandomValueGenerator<T> SetSeed(int seed)
        {
            _generator = new Random(seed);
            return this;
        }

        public virtual RandomValueGenerator<T> IncrementSeed()
        {
            _seed++;
            _generator = new Random(_seed);
            return this;
        }
    }
}
