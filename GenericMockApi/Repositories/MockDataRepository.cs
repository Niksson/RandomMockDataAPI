using GenericMockApi.Options;
using GenericMockApi.Repositories.RandomGeneratorFactory;
using GenericMockApi.Repositories.RandomGenerators;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories
{
    public class MockDataRepository : IMockDataRepository
    {
        private readonly IRandomGeneratorFactory _generatorFactory;
        private readonly TypeFinder _finder;
        private readonly IOptionsSnapshot<MockGeneratorOptions> _options;

        public MockDataRepository(IRandomGeneratorFactory factory, TypeFinder finder, IOptionsSnapshot<MockGeneratorOptions> options)
        {
            _generatorFactory = factory;
            _finder = finder;
            _options = options;
        }
        public IEnumerable<object> GetMockObjects(string typeName, int skip, int take)
        {
            var type = _finder.GetTypeFromAssemblies(typeName);
        }

        public IReadOnlyCollection<T> GetRandomObjects<T>(int depthLimit, int masterSeed, int skip, int take)
        {
            if (depthLimit < 1) return null; // The needed depth was reached? Then get outta here!

            var objects = new List<T>();
            var propGenerators = new Dictionary<PropertyInfo, IRandomValueGenerator>();

            // Get all props of a type provided in arguments
            var props = typeof(T).GetRuntimeProperties().ToList();

            // 1. Filter out all read only props, we cannot assign them anyway
            props = props.Where(p => p.CanWrite).ToList();

            // 2. Try to find navigation properties (properties with a foreign key)
            List<PropertyInfo> foreignKeyProps;
            // Key: ID prop, Value: Navi prop
            var navigationProperties = new Dictionary<PropertyInfo, PropertyInfo>();

            // Method A: via ForeignKeyAttribute
            foreignKeyProps = props.Where(p => p.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();
            foreach(var prop in foreignKeyProps)
            {
                var attr = prop.GetCustomAttribute<ForeignKeyAttribute>();
                var navigationProp = foreignKeyProps.FirstOrDefault(p => p.Name == attr.Name);
                
                // If the navigation property wasn't found
                // (which can happen since it's not checked compile-time)
                // we remove it from the list of props that are foreign key ID
                if (navigationProp != null)
                {
                    // Check if the type of navigation property really contains the field "ID" (case insensitive)
                    var idProp = navigationProp.PropertyType.GetProperties().FirstOrDefault(p => p.Name.ToLower() == "id");
                    if(idProp != null)
                    {
                        navigationProperties.Add(prop, navigationProp);
                        props.Remove(navigationProp);
                    }
                    else foreignKeyProps.Remove(prop);
                }
                else foreignKeyProps.Remove(prop);
            }

            // Remove all found props from main property list, they will be instantiated separately
            props = props.Except(foreignKeyProps).ToList();

            // Method B: via checking the names

            // Get properties, types of which are classes (excluding string)
            var classProps = props.Where(p => p.PropertyType != typeof(string) && p.PropertyType.IsClass).ToList();
            foreach(var prop in classProps &&)
            {
                var fkProp = props.FirstOrDefault(p => $"{prop.Name}Id".ToLower() == p.Name.ToLower());
                if (fkProp != null 
                    && (prop.PropertyType
                            .GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower() == "id")
                {
                    navigationProperties.Add(fkProp, prop);
                    props.Remove(fkProp);
                    props.Remove(prop);
                }
            }

            // 3. Assign generators to properties with "primitive" types
            var propsWithGenerators = new Dictionary<PropertyInfo, IRandomValueGenerator>();

            // 3.1. Numeric props

            // A very ugly way of checking if the type is numeric
            var numericProps = props.Where(p =>
            {
                return p.PropertyType == typeof(int) ||
                       p.PropertyType == typeof(double) ||
                       p.PropertyType == typeof(decimal) ||
                       p.PropertyType == typeof(float) ||
                       p.PropertyType == typeof(byte) ||
                       p.PropertyType == typeof(short) ||
                       p.PropertyType == typeof(long) ||
                       p.PropertyType == typeof(uint) ||
                       p.PropertyType == typeof(ulong);

            }).ToList();

            foreach(var prop in numericProps)
            {
                // This is an additional seed based on type name and property name
                // It will be added to the master seed to aquire a stable pseudo-random
                // sequence for each property
                var additionalSeed = GetAdditionalSeed<T>(prop);

                propsWithGenerators.Add(prop, _generatorFactory.CreateNumeric(masterSeed + additionalSeed));
            }

            props = props.Except(numericProps).ToList();

            // 3.2 String props
            var stringProps = props.Where(p => p.PropertyType == typeof(string)).ToList();

            foreach(var prop in stringProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                propsWithGenerators.Add(prop, _generatorFactory.CreateString(masterSeed + additionalSeed));
            }

            props = props.Except(stringProps).ToList();

            // 3.3 Boolean props
            var booleanProps = props.Where(p => p.PropertyType == typeof(bool)).ToList();

            foreach (var prop in booleanProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                propsWithGenerators.Add(prop, _generatorFactory.CreateBoolean(masterSeed + additionalSeed));
            }

            props = props.Except(booleanProps).ToList();

            // 3.4 DateTime props
            var dateTimeProps = props.Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var prop in dateTimeProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                propsWithGenerators.Add(prop, _generatorFactory.CreateDateTime(masterSeed + additionalSeed));
            }

            props = props.Except(dateTimeProps).ToList();

            // 4. Get collection props
            var collectionProps = props.Where(p => p.PropertyType != typeof(string) && p.PropertyType.GetInterface(nameof(ICollection<object>)) != null).ToList();

            var collectionsWithAmountGenerator = new Dictionary<PropertyInfo, Random>();

            foreach(var prop in collectionProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);
                collectionsWithAmountGenerator.Add(prop, new Random(masterSeed + additionalSeed));
            }

            props = props.Except(collectionProps).ToList();

            // 5. Leave all props that we can easily assign at runtime - which have a constructor with no parameters
            props = props.Where(p => p.PropertyType.GetConstructor(Type.EmptyTypes) != null).ToList();


            // 6. Skip the requested amount of random values for each generator
            // TODO: The stability of random generation may fail due to the cast, be vigilant
            foreach(var propWithGen in propsWithGenerators)
            {
                var generatorType = propWithGen.Value.GetType();
                dynamic generator = Convert.ChangeType(propWithGen.Value, generatorType);
                // This step is done to ensure that generator has random generator with skipped sequence
                propsWithGenerators[propWithGen.Key] = generator.Skip(skip);
            }

            // 7. Instantiate the objects

            for (int i = 0; i < take; i++)
            {
                var objectOfSpecifiedType = Activator.CreateInstance(typeof(T));

                // Set values of "primitive" types
                foreach(var propWithGen in propsWithGenerators)
                {
                    var generatorType = propWithGen.Value.GetType();
                    dynamic generator = Convert.ChangeType(propWithGen.Value, generatorType);
                    propWithGen.Key.SetValue(objectOfSpecifiedType, generator.GetNext());
                    propsWithGenerators[propWithGen.Key] = generator;
                }

                // The hard work starts here - generate objects of reference types
                
                // Set values of navigation types
                foreach(var propWithFk in navigationProperties)
                {
                    // Property representing foreign key
                    var fkProp = propWithFk.Key;
                    // Property representing navigation property
                    var navigationProp = propWithFk.Value;

                    // We made sure that we will have the Id field upper in code
                    var idProp = navigationProp.PropertyType.GetProperties().First(p => p.Name.ToLower() == "id");

                    // A VERY, VERY ugly way to recursively generate value for a reference type
                    var getRandomObjectsMethod = typeof(MockDataRepository).GetMethod(nameof(GetRandomObjects));
                    var getRandomObjectsGeneric = getRandomObjectsMethod.MakeGenericMethod(navigationProp.PropertyType);
                    var generatedValueEnumerable = getRandomObjectsGeneric.Invoke(this, new object[] { --depthLimit, masterSeed, 0, 1 });

                    var ienumerableType = typeof(IEnumerable<>).MakeGenericType(navigationProp.PropertyType);

                    dynamic generatedValue = Convert.ChangeType(generatedValueEnumerable, ienumerableType);
                    // Set the value for property with reference type
                    navigationProp.SetValue(objectOfSpecifiedType, generatedValue.First());
                    // Then, set the value for its FK property
                    fkProp.SetValue(objectOfSpecifiedType, idProp.GetValue(generatedValue));
                }

                // Set value of collection properties
                {

                }
            }
        }

        private int GetAdditionalSeed<T>(PropertyInfo prop)
        {
            return (prop.Name + nameof(T)).GetHashCode();
        }

    }

}
