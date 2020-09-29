using System.Reflection;
using RandomMockDataAPI.Repositories.RandomGenerators;

namespace RandomMockDataAPI.Repositories
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