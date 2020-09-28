using GenericMockApi.Helpers;
using GenericMockApi.Repositories.RandomGenFactory;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class RandomObjectGenerator<T> : RandomValueGenerator<T> where T : class
    {

        private int _masterSeed;
        private readonly uint _depthLimit;

        private readonly IRandomGeneratorFactory _factory = new RandomGeneratorFactory();

        private IEnumerable<PropertyInfo> props;
        
        private Dictionary<PropertyInfo, RandomValueGenerator<double>> numericValueGenerators;
        private Dictionary<PropertyInfo, RandomValueGenerator<string>> stringValueGenerators;
        private Dictionary<PropertyInfo, RandomValueGenerator<bool>> booleanValueGenerators;
        private Dictionary<PropertyInfo, RandomValueGenerator<DateTime>> dateTimeValueGenerators;
        private Dictionary<PropertyInfo, RandomValueGenerator> collectionValueGenerators;
        private Dictionary<PropertyInfo, RandomValueGenerator> navigationPropertiesGenerators;
        private Dictionary<PropertyInfo, RandomValueGenerator> objectValueGenerators;

        // Key: navigation property, Value: its FK
        private Dictionary<PropertyInfo, PropertyInfo> navigationPropertiesWithFk;


        public RandomObjectGenerator(int masterSeed, uint depthLimit, IEnumerable<PropertyInfo> ignoredProperties = null) : base(masterSeed)
        {
            _masterSeed = masterSeed;
            _depthLimit = depthLimit;
            
            // Get all props of a type
            props = typeof(T).GetRuntimeProperties();

            if(ignoredProperties != null) props = props.Except(ignoredProperties);

            InitializeGenerators();
        }

        private void InitializeGenerators()
        {
            

            // 1. Filter out all read only props, we cannot assign them anyway
            props = props.Where(p => p.CanWrite);

            #region Navigation props generators
            navigationPropertiesGenerators = new Dictionary<PropertyInfo, RandomValueGenerator>();

            // 2. Try to find navigation properties (properties with a foreign key

            navigationPropertiesWithFk = new Dictionary<PropertyInfo, PropertyInfo>();

            if (_depthLimit > 0)
            {
                List<PropertyInfo> foreignKeyProps;

                // Method A: via ForeignKeyAttribute
                foreignKeyProps = props.Where(p => p.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();
                foreach (var prop in foreignKeyProps)
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
                        if (idProp != null)
                        {
                            var generatorType = typeof(RandomObjectGenerator<>).MakeGenericType(typeof(T));
                            var generator = (RandomValueGenerator)Activator.CreateInstance(generatorType, _masterSeed + GetAdditionalSeed<T>(navigationProp), _depthLimit - 1);
                            navigationPropertiesGenerators.Add(navigationProp, generator);
                            navigationPropertiesWithFk.Add(navigationProp, prop);
                        }
                        else foreignKeyProps.Remove(prop);
                    }
                    else foreignKeyProps.Remove(prop);
                }

                props = props.Except(navigationPropertiesWithFk.Keys);
                props = props.Except(navigationPropertiesWithFk.Values);

                // Method B: via checking the names

                // Get properties, types of which are classes (excluding string)
                var classProps = props.Where(p => p.PropertyType != typeof(string) && p.PropertyType.IsClass);
                foreach (var prop in classProps)
                {
                    var fkProp = props.FirstOrDefault(p => $"{prop.Name}Id".ToLower() == p.Name.ToLower());
                    if (fkProp != null && (prop.PropertyType
                                .GetProperties()
                                .FirstOrDefault(p => p.Name.ToLower() == "id") != null))
                    {
                        var generatorType = typeof(RandomObjectGenerator<>).MakeGenericType(prop.PropertyType);
                        var generator = (RandomValueGenerator)Activator.CreateInstance(generatorType, _masterSeed + GetAdditionalSeed<T>(prop), _depthLimit - 1);
                        navigationPropertiesGenerators.Add(prop, generator);
                        navigationPropertiesWithFk.Add(prop, fkProp);
                    }
                }

                props = props.Except(navigationPropertiesWithFk.Keys);
                props = props.Except(navigationPropertiesWithFk.Values);
            }

            #endregion

            #region Collection navigation props generators

            

            #endregion

            #region Primitive props generators

            // 3. Assign generators to properties with "primitive" types

            // 3.1 Numeric props
            numericValueGenerators = new Dictionary<PropertyInfo, RandomValueGenerator<double>>();

            var numericProps = props.Where(p => TypeCheckExtensions.IsTypeNumericOrChar(p.PropertyType));

            foreach (var prop in numericProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                numericValueGenerators.Add(prop, _factory.CreateNumericGenerator(_masterSeed + additionalSeed));
            }

            // 3.2 String props
            stringValueGenerators = new Dictionary<PropertyInfo, RandomValueGenerator<string>>();

            var stringProps = props.Where(p => p.PropertyType == typeof(string));

            foreach (var prop in stringProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                stringValueGenerators.Add(prop, _factory.CreateStringGenerator(_masterSeed + additionalSeed));
            }

            props = props.Except(stringProps).ToList();

            // 3.3 Boolean props
            booleanValueGenerators = new Dictionary<PropertyInfo, RandomValueGenerator<bool>>();

            var booleanProps = props.Where(p => p.PropertyType == typeof(bool));

            foreach (var prop in booleanProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                booleanValueGenerators.Add(prop, _factory.CreateBooleanGenerator(_masterSeed + additionalSeed));
            }

            props = props.Except(booleanProps).ToList();

            // 3.4 DateTime props
            dateTimeValueGenerators = new Dictionary<PropertyInfo, RandomValueGenerator<DateTime>>();

            var dateTimeProps = props.Where(p => p.PropertyType == typeof(DateTime));

            foreach (var prop in dateTimeProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                dateTimeValueGenerators.Add(prop, _factory.CreateDateTimeGenerator(_masterSeed + additionalSeed));
            }

            props = props.Except(dateTimeProps).ToList();

            #endregion

            #region Collection props generators

            // 4. Assign generators to collection props
            collectionValueGenerators = new Dictionary<PropertyInfo, RandomValueGenerator>();

            if(_depthLimit >= 0)
            {
                // We already excluded all props of type string, so we can safely
                // check only for IEnumerable
                var collectionProps = props.Where(p => p.PropertyType.GetInterface(nameof(IEnumerable)) != null);

                foreach (var prop in collectionProps)
                {
                    var additionalSeed = GetAdditionalSeed<T>(prop);

                    var generatorType = typeof(RandomCollectionGenerator<>).MakeGenericType(prop.PropertyType);
                    var generator = (RandomValueGenerator)Activator.CreateInstance(generatorType, _masterSeed + additionalSeed, _depthLimit - 1);

                    collectionValueGenerators.Add(prop, generator);
                }

            }
            #endregion

            #region Other props generators
            // 5. Leave all props that we can easily assign at runtime - which have a constructor with no parameters
            objectValueGenerators = new Dictionary<PropertyInfo, RandomValueGenerator>();

            if (_depthLimit >= 1)
            {
                props = props.Where(p => p.PropertyType.GetConstructor(Type.EmptyTypes) != null);
                foreach (var prop in props)
                {
                    var additionalSeed = GetAdditionalSeed<T>(prop);

                    var generatorType = typeof(RandomObjectGenerator<>).MakeGenericType(prop.PropertyType);
                    var generator = (RandomValueGenerator)Activator.CreateInstance(generatorType, additionalSeed, _depthLimit - 1);
                    objectValueGenerators.Add(prop, generator);
                }
            }

            #endregion

        }

        public override T GetNext()
        {
            if (_depthLimit < 1) return null;

            var nextSeed = ++_masterSeed;

            var instance = (T)Activator.CreateInstance(typeof(T));

            foreach(var navigationProp in navigationPropertiesWithFk)
            {
                dynamic generator = navigationPropertiesGenerators[navigationProp.Key];
                var generatedValue = generator.GetNext();
                navigationProp.Key.SetValue(instance, generatedValue);
                var idProp = navigationProp.Key.PropertyType.GetProperties().FirstOrDefault(p => p.Name.ToLower() == "id");
                navigationProp.Value.SetValue(instance, idProp.GetValue(generatedValue));
            }

            foreach(dynamic generator in navigationPropertiesGenerators.Values)
            {
                generator.IncrementSeed();
            }

            #region Primitive properties

            foreach(var propertyWithGenerator in numericValueGenerators)
            {
                var generatedValue = propertyWithGenerator.Value.GetNext();
                if(propertyWithGenerator.Key.PropertyType != typeof(double))
                {
                    var convertedValue = Convert.ChangeType(generatedValue, propertyWithGenerator.Key.PropertyType);
                    propertyWithGenerator.Key.SetValue(instance, convertedValue);
                }
                else propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());

                
            }

            foreach(var generator in numericValueGenerators.Values)
            {
                generator.IncrementSeed();
            }

            foreach (var propertyWithGenerator in stringValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }

            foreach (var generator in stringValueGenerators.Values)
            {
                generator.IncrementSeed();
            }

            foreach (var propertyWithGenerator in booleanValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }
            foreach (var generator in booleanValueGenerators.Values)
            {
                generator.IncrementSeed();
            }

            foreach (var propertyWithGenerator in dateTimeValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }

            foreach (var generator in dateTimeValueGenerators.Values)
            {
                generator.IncrementSeed();
            }

            #endregion

            foreach (var propertyWithGenerator in collectionValueGenerators)
            {
                dynamic generator = propertyWithGenerator.Value;
                var generatedValue = generator.GetNext();
                propertyWithGenerator.Key.SetValue(instance, generatedValue);
            }

            foreach (dynamic generator in collectionValueGenerators.Values)
            {
                generator.IncrementSeed();
            }

            foreach (var propertyWithGenerator in objectValueGenerators)
            {
                dynamic generator = propertyWithGenerator.Value;
                var generatedValue = generator.GetNext();
                propertyWithGenerator.Key.SetValue(instance, generatedValue);
            }

            foreach (dynamic generator in objectValueGenerators.Values)
            {
                generator.IncrementSeed();
            }

            return instance;
        }

        private int GetAdditionalSeed<TType>(PropertyInfo prop)
        {
            return GenerationHelpers.GetDeterministicHashCode((prop.Name + typeof(TType).Name));
        }

        public override RandomValueGenerator<T> Skip(int skip)
        {
            unchecked
            {
                _masterSeed += skip;
                InitializeGenerators();
                return this;
            }
        }
    }
}
