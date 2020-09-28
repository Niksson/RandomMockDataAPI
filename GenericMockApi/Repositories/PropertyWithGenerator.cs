using System.Reflection;
using GenericMockApi.Repositories.RandomGenerators;

namespace GenericMockApi.Repositories
{
    public class PropertyWithGenerator
    {
        public PropertyInfo Property { get; private set; }

        public RandomValueGenerator RandomGenerator { get; private set; }

        public PropertyWithGenerator()
        {
            
        }
    }
}