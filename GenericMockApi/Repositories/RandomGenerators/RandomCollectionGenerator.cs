
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

    public class RandomCollectionGenerator<T> : RandomValueGenerator<T> where T : IEnumerable
    {
        private readonly IRandomGeneratorFactory _generatorFactory = new RandomGeneratorFactory();
        private readonly int _seed;
        private readonly int _depthLimit;

        private RandomValueGenerator<int> _randomLengthGenerator;
        private AbstractRandomValueGenerator _randomValueGenerator;

        public RandomCollectionGenerator(int seed, int depthLimit)
        {
            _seed = seed;
            _depthLimit = depthLimit;
        }

        private void InitializeGenerators()
        {
            _randomLengthGenerator = _generatorFactory.CreateCollectionSizeGenerator(_seed);

            var collectionType = typeof(T);
            var typeParameter = collectionType.GetGenericArguments().FirstOrDefault();

            if (TypeHelpers.IsTypeNumericOrChar(typeParameter))
                _randomValueGenerator = _generatorFactory.CreateNumericGenerator(_seed);
            else if (typeParameter == typeof(string))
                _randomValueGenerator = _generatorFactory.CreateStringGenerator(_seed);
            else if (typeParameter == typeof(bool))
                _randomValueGenerator = _generatorFactory.CreateBooleanGenerator(_seed);
            else if (typeParameter == typeof(DateTime))
                _randomValueGenerator = _generatorFactory.CreateDateTimeGenerator(_seed);

            // Just for sake of not making this too complicated let's assume that if a type
            // implements IEnumerable then it's a collection 
            // We've also already checked if the type is string, so it's safe to just check IEnumerable
            else if (typeParameter.GetInterface(nameof(IEnumerable)) != null)
                _randomValueGenerator = _generatorFactory.CreateCollectionGenerator(typeParameter, _seed + _depthLimit, _depthLimit - 1);

            else if (typeParameter.IsClass && !(typeof(Delegate).IsAssignableFrom(typeParameter)) && typeParameter.GetConstructor(Type.EmptyTypes) != null)
                _randomValueGenerator = _generatorFactory.CreateObjectGenerator(typeParameter, _seed, _depthLimit - 1);

            // We can't assign value types other than we know about or types without a parameterless constructor
            else throw new ArgumentException("The type parameter of collection is not a simple type, class type or a collection type, or it doesn't have a parameterless constructor");
           
        }

        public override T GetNext()
        {
            var length = _randomLengthGenerator.GetNext();

            // Create instance of collection
            var collectionType = typeof(T);
            var typeParameter = collectionType.GetGenericArguments().FirstOrDefault();
            var iListType = typeof(IList<>).MakeGenericType(typeParameter);

            var instance = (IList)Activator.CreateInstance(iListType);

            var generatorType = _randomValueGenerator.GetType();
            dynamic generator = Convert.ChangeType(_randomValueGenerator, generatorType);

            for (int i = 0; i < length; i++)
            {
                instance.Add(generator.GetNext());
            }

            return (T)Convert.ChangeType(instance, typeof(T));
        }

    }
}
