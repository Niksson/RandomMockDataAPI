using GenericMockApi.Options;
using GenericMockApi.Repositories.RandomGenFactory;
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
    public class RandomMockDataRepository : IMockDataRepository
    {
        private readonly IRandomGeneratorFactory _generatorFactory;
        private readonly TypeFinder _finder;
        private readonly IOptionsSnapshot<MockGeneratorOptions> _options;

        public RandomMockDataRepository(IRandomGeneratorFactory factory, TypeFinder finder, IOptionsSnapshot<MockGeneratorOptions> options)
        {
            _generatorFactory = factory;
            _finder = finder;
            _options = options;
        }
        public IEnumerable<object> GetMockObjects(string typeName, int skip, int take)
        {
            var masterSeed = _options.Value.MasterSeed;
            var depthLimit = _options.Value.DepthLimit;

            var objects = new List<object>();

            var type = _finder.GetTypeFromAssemblies(typeName);

            var generator = _generatorFactory.CreateObjectGenerator(type, masterSeed, depthLimit);

            var generatorType = generator.GetType();

            dynamic dynamicGenerator = Convert.ChangeType(generator, generatorType);
            dynamicGenerator = dynamicGenerator.Skip(skip);

            for (int i = 0; i < take; i++)
            {
                objects.Add((object)dynamicGenerator.GetNext());
            }

            return objects;

        }


    }

}
