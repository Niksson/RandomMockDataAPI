﻿using GenericMockApi.Helpers;
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
    public class RandomObjectGenerator<T> : IRandomValueGenerator<T> where T : class
    {

        private readonly int _masterSeed;
        private readonly int _depthLimit;

        private readonly IRandomGeneratorFactory _factory = new RandomGeneratorFactory();

        private Dictionary<PropertyInfo, IRandomValueGenerator<double>> numericValueGenerators;
        private Dictionary<PropertyInfo, IRandomValueGenerator<string>> stringValueGenerators;
        private Dictionary<PropertyInfo, IRandomValueGenerator<bool>> booleanValueGenerators;
        private Dictionary<PropertyInfo, IRandomValueGenerator<DateTime>> dateTimeValueGenerators;
        private Dictionary<PropertyInfo, IRandomValueGenerator> collectionValueGenerators;
        private Dictionary<PropertyInfo, IRandomValueGenerator> objectValueGenerators;

        // Key: navigation property, Value: its FK
        private Dictionary<PropertyInfo, PropertyInfo> navigationPropertiesWithFk;


        public RandomObjectGenerator(int masterSeed, int depthLimit)
        {
            _masterSeed = masterSeed;
            _depthLimit = depthLimit;

            InitializeGenerators();
        }

        private void InitializeGenerators()
        {
            // Get all props of a type
            var props = typeof(T).GetRuntimeProperties().ToList();

            // 1. Filter out all read only props, we cannot assign them anyway
            props = props.Where(p => p.CanWrite).ToList();

            #region Navigation props generators

            // 2. Try to find navigation properties (properties with a foreign key)
            navigationPropertiesWithFk = new Dictionary<PropertyInfo, PropertyInfo>();

            if (_depthLimit > 1)
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
                            var generator = (IRandomValueGenerator)Activator.CreateInstance(generatorType, _masterSeed + GetAdditionalSeed<T>(navigationProp), _depthLimit - 1);
                            objectValueGenerators.Add(navigationProp, generator);
                            props.Remove(navigationProp);
                        }
                        else foreignKeyProps.Remove(prop);
                    }
                    else foreignKeyProps.Remove(prop);
                }

                props = props.Except(foreignKeyProps).ToList();

                // Method B: via checking the names

                // Get properties, types of which are classes (excluding string)
                var classProps = props.Where(p => p.PropertyType != typeof(string) && p.PropertyType.IsClass).ToList();
                foreach (var prop in classProps)
                {
                    var fkProp = props.FirstOrDefault(p => $"{prop.Name}Id".ToLower() == p.Name.ToLower());
                    if (fkProp != null && (prop.PropertyType
                                .GetProperties()
                                .FirstOrDefault(p => p.Name.ToLower() == "id") != null))
                    {
                        var generatorType = typeof(RandomObjectGenerator<>).MakeGenericType(typeof(T));
                        var generator = (IRandomValueGenerator)Activator.CreateInstance(generatorType, _masterSeed + GetAdditionalSeed<T>(prop), _depthLimit - 1);
                        objectValueGenerators.Add(prop, generator);

                        props.Remove(fkProp);
                        props.Remove(prop);
                    }
                } 
            }

            #endregion

            #region Primitive props generators

            // 3. Assign generators to properties with "primitive" types

            // 3.1 Numeric props
            numericValueGenerators = new Dictionary<PropertyInfo, IRandomValueGenerator<double>>();

            var numericProps = props.Where(p => TypeHelpers.IsTypeNumericOrChar(p.PropertyType));

            foreach (var prop in numericProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                numericValueGenerators.Add(prop, _factory.CreateNumericGenerator(_masterSeed + additionalSeed););
            }

            // 3.2 String props
            stringValueGenerators = new Dictionary<PropertyInfo, IRandomValueGenerator<string>>();

            var stringProps = props.Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var prop in stringProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                stringValueGenerators.Add(prop, _factory.CreateStringGenerator(_masterSeed + additionalSeed));
            }

            props = props.Except(stringProps).ToList();

            // 3.3 Boolean props
            booleanValueGenerators = new Dictionary<PropertyInfo, IRandomValueGenerator<bool>>();

            var booleanProps = props.Where(p => p.PropertyType == typeof(bool)).ToList();

            foreach (var prop in booleanProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                booleanValueGenerators.Add(prop, _factory.CreateBooleanGenerator(_masterSeed + additionalSeed));
            }

            props = props.Except(booleanProps).ToList();

            // 3.4 DateTime props
            dateTimeValueGenerators = new Dictionary<PropertyInfo, IRandomValueGenerator<DateTime>>();

            var dateTimeProps = props.Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var prop in dateTimeProps)
            {
                var additionalSeed = GetAdditionalSeed<T>(prop);

                dateTimeValueGenerators.Add(prop, _factory.CreateDateTimeGenerator(_masterSeed + additionalSeed));
            }

            props = props.Except(dateTimeProps).ToList();

            #endregion

            #region Collection props generators

            // 4. Assign generators to collection props

            if(_depthLimit > 0)
            {
                // We already excluded all props of type string, so we can safely
                // check only for IEnumerable
                var collectionProps = props.Where(p => p.PropertyType.GetInterface(nameof(IEnumerable)) != null).ToList();

                foreach (var prop in collectionProps)
                {
                    var additionalSeed = GetAdditionalSeed<T>(prop);

                    var generatorType = typeof(RandomCollectionGenerator<>).MakeGenericType(prop.PropertyType);
                    var generator = (IRandomValueGenerator)Activator.CreateInstance(generatorType, _masterSeed + additionalSeed, _depthLimit - 1);

                    collectionValueGenerators.Add(prop, generator);
                }

            }
            #endregion

            #region Other props generators
            // 5. Leave all props that we can easily assign at runtime - which have a constructor with no parameters
            
            if(_depthLimit > 1)
            {
                props = props.Where(p => p.PropertyType.GetConstructor(Type.EmptyTypes) != null).ToList();
                foreach (var prop in props)
                {
                    var additionalSeed = GetAdditionalSeed<T>(prop);

                    var generatorType = typeof(RandomObjectGenerator<>).MakeGenericType(prop.PropertyType);
                    var generator = (IRandomValueGenerator)Activator.CreateInstance(generatorType, additionalSeed, _depthLimit - 1);
                    objectValueGenerators.Add(prop, generator);
                }
            }

            #endregion

        }

        public T GetNext()
        {
            if (_depthLimit < 1) return null;

            var instance = (T)Activator.CreateInstance(typeof(T));

            foreach(var navigationProp in navigationPropertiesWithFk)
            {
                var generatorType = objectValueGenerators[navigationProp.Key].GetType();
                dynamic generator = Convert.ChangeType(objectValueGenerators[navigationProp.Key], generatorType);
                var generatedValue = generator.GetNext();
                navigationProp.Key.SetValue(instance, generatedValue);
                var idProp = navigationProp.Key.PropertyType.GetProperties().FirstOrDefault(p => p.Name.ToLower() == "id");
                navigationProp.Value.SetValue(instance, idProp.GetValue(generatedValue));
                objectValueGenerators.Remove(navigationProp.Key);
            }

            #region Primitive properties

            foreach(var propertyWithGenerator in numericValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }

            foreach (var propertyWithGenerator in stringValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }

            foreach (var propertyWithGenerator in booleanValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }

            foreach (var propertyWithGenerator in dateTimeValueGenerators)
            {
                propertyWithGenerator.Key.SetValue(instance, propertyWithGenerator.Value.GetNext());
            }

            #endregion

            foreach(var propWithGenerator in collectionValueGenerators)
            {
                var generatorType = propWithGenerator.Value.GetType();
                dynamic generator = Convert.ChangeType(propWithGenerator.Value, generatorType);
                var generatedValue = generator.GetNext();
                propWithGenerator.Key.SetValue(instance, generatedValue);
            }

            foreach (var propWithGenerator in objectValueGenerators)
            {
                var generatorType = propWithGenerator.Value.GetType();
                dynamic generator = Convert.ChangeType(propWithGenerator.Value, generatorType);
                var generatedValue = generator.GetNext();
                propWithGenerator.Key.SetValue(instance, generatedValue);
            }

            return instance;
        }

        private int GetAdditionalSeed<T>(PropertyInfo prop)
        {
            return (prop.Name + nameof(T)).GetHashCode();
        }
    }
}