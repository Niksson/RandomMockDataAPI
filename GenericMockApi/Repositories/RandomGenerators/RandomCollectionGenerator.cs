
using GenericMockApi.Helpers;
using GenericMockApi.Repositories.RandomGenFactory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{

    public class RandomCollectionGenerator<TCollection> : RandomValueGenerator<TCollection> where TCollection : IEnumerable
    {
        private readonly IRandomGeneratorFactory _generatorFactory = new RandomGeneratorFactory();
        private readonly int _seed;
        private readonly uint _depthLimit;

        private readonly Type _collectionType;
        private readonly Type _typeParameter;

        private RandomValueGenerator<int> _randomLengthGenerator;
        private AbstractRandomValueGenerator _randomValueGenerator;

        public RandomCollectionGenerator(int seed, uint depthLimit) : base(seed)
        {
            _seed = seed;
            _depthLimit = depthLimit;

            _collectionType = typeof(TCollection);
            if (_collectionType.IsArray) _typeParameter = _collectionType.GetElementType();
            else _typeParameter = _collectionType.GetGenericArguments().FirstOrDefault();

            InitializeGenerators();
        }

        private void InitializeGenerators()
        {
            _randomLengthGenerator = _generatorFactory.CreateCollectionSizeGenerator(_seed);

            if (TypeHelpers.IsTypeNumericOrChar(_typeParameter))
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
                _randomValueGenerator = _generatorFactory.CreateCollectionGenerator(_typeParameter, (int)(_seed + _depthLimit), _depthLimit - 1);

            else if (_depthLimit >= 0 && _typeParameter.IsClass && !(typeof(Delegate).IsAssignableFrom(_typeParameter)) && _typeParameter.GetConstructor(Type.EmptyTypes) != null)
                _randomValueGenerator = _generatorFactory.CreateObjectGenerator(_typeParameter, _seed, _depthLimit - 1);

            // We can't assign value types other than we know about or types without a parameterless constructor
            else _randomValueGenerator = null;
           
        }

        public override TCollection GetNext()
        {
            var length = _randomLengthGenerator.GetNext();

            // Create instance of collection
            var iListType = typeof(List<>).MakeGenericType(_typeParameter);

            var instance = (IList)Activator.CreateInstance(iListType);

            if(_randomValueGenerator != null)
            {
                var generatorType = _randomValueGenerator.GetType();
                dynamic generator = Convert.ChangeType(_randomValueGenerator, generatorType);

                for (int i = 0; i < length; i++)
                {
                    var value = generator.GetNext();
                    if(TypeHelpers.IsTypeNumericOrChar(_typeParameter))
                    {
                        var convertedValue = Convert.ChangeType(value, _typeParameter);
                        var addingMethod = instance.GetType().GetMethod("Add");
                        addingMethod.Invoke(instance, new object[] { convertedValue });
                    }
                    else instance.Add(generator.GetNext());
                }
            }
            else
                for (int i = 0; i < length; i++)
                {
                    instance.Add(Activator.CreateInstance(_typeParameter));
                }
            if (_collectionType.IsArray)
            {
                var arrayType = _typeParameter.MakeArrayType();
                dynamic arrayInstance = Activator.CreateInstance(arrayType, length);
                instance.CopyTo(arrayInstance, 0);
                return arrayInstance;
            }
            return (TCollection)instance;
        }

    }
}
