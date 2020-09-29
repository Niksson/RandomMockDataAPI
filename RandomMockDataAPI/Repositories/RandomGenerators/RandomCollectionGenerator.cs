using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RandomMockDataAPI.Helpers;
using RandomMockDataAPI.Repositories.RandomGenFactory;

namespace RandomMockDataAPI.Repositories.RandomGenerators
{

    public class RandomCollectionGenerator<TCollection> : RandomValueGenerator<TCollection> where TCollection : IEnumerable
    {
        private readonly IRandomGeneratorFactory _generatorFactory = new RandomGeneratorFactory();
        private readonly uint _depthLimit;

        private readonly Type _collectionType;
        private readonly Type _typeParameter;

        private RandomValueGenerator<int> _randomLengthGenerator;
        private RandomValueGenerator _randomValueGenerator;

        public RandomCollectionGenerator(int seed, uint depthLimit) : base(seed)
        {
            _depthLimit = depthLimit;

            _collectionType = typeof(TCollection);
            if (_collectionType.IsArray) _typeParameter = _collectionType.GetElementType();
            else _typeParameter = _collectionType.GetGenericArguments().FirstOrDefault();

            InitializeGenerators();
        }

        private void InitializeGenerators()
        {
            _randomLengthGenerator = _generatorFactory.CreateCollectionSizeGenerator(_seed);

            if (TypeCheckExtensions.IsTypeNumericOrChar(_typeParameter))
                _randomValueGenerator = _generatorFactory.CreateNumericGenerator(_seed);
            else if (_typeParameter == typeof(string))
                _randomValueGenerator = _generatorFactory.CreateStringGenerator(_seed);
            else if (_typeParameter == typeof(bool))
                _randomValueGenerator = _generatorFactory.CreateBooleanGenerator(_seed);
            else if (_typeParameter == typeof(DateTime))
                _randomValueGenerator = _generatorFactory.CreateDateTimeGenerator(_seed);

            // Just for sake of not making this too complicated let's assume that if a type
            // implements IEnumerable then it's a collection 
            // We've also already checked if the type is string, so it's safe to just check IEnumerable
            else if (_depthLimit >= 0 && _typeParameter.GetInterface(nameof(IEnumerable)) != null)
                _randomValueGenerator = _generatorFactory.CreateCollectionGenerator(_typeParameter, (int)(_seed + _depthLimit), _depthLimit);

            else if (_depthLimit > 0 && _typeParameter.IsClass && !(typeof(Delegate).IsAssignableFrom(_typeParameter)) && _typeParameter.GetConstructor(Type.EmptyTypes) != null)
                _randomValueGenerator = _generatorFactory.CreateObjectGenerator(_typeParameter, _seed, _depthLimit - 1);

            // We can't assign value types other than we know about or types without a parameterless constructor
            else _randomValueGenerator = null;
           
        }

        public override TCollection GetNext()
        {
            var length = _randomLengthGenerator.GetNext();

            if(_collectionType.IsArray)
            {
                // Create instance of collection
                var arrayType = _typeParameter.MakeArrayType();

                dynamic instance = Activator.CreateInstance(arrayType, length);

                if (_randomValueGenerator != null)
                {
                    dynamic generator = _randomValueGenerator;

                    for (int i = 0; i < length; i++)
                    {
                        var value = generator.GetNext();
                        if (TypeCheckExtensions.IsTypeNumericOrChar(_typeParameter))
                        {
                            dynamic convertedValue = Convert.ChangeType(value, _typeParameter);
                            instance[i] = convertedValue;
                        }
                        else instance[i] = value;
                    }
                }
                else return default;

                return (TCollection)instance;
            }
            else
            {
                // Create an instance of collection
                var genericType = typeof(List<>).MakeGenericType(_typeParameter);

                dynamic instance = Activator.CreateInstance(genericType);

                if (_randomValueGenerator != null)
                {
                    dynamic generator = _randomValueGenerator;

                    for (int i = 0; i < length; i++)
                    {
                        var value = generator.GetNext();
                        if (TypeCheckExtensions.IsTypeNumericOrChar(_typeParameter))
                        {
                            dynamic convertedValue = Convert.ChangeType(value, _typeParameter);
                            instance.Add(convertedValue);
                        }
                        else instance.Add(value);
                    }
                }
                else return default;

                return (TCollection)instance;
            }
        }

        public override RandomValueGenerator<TCollection> SetSeed(int seed)
        {
            _seed = seed;
            InitializeGenerators();
            return this;
        }

        public override RandomValueGenerator<TCollection> IncrementSeed()
        {
            _seed++;
            InitializeGenerators();
            return this;
        }

    }
}
