using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Helpers
{
    public class TypeHelpers
    {
        public static bool IsTypeNumericOrChar(Type type)
        {
            var numericTypes = new List<Type>()
            {
                typeof(int),
                typeof(int),
                typeof(double),
                typeof(decimal),
                typeof(float),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(long),
                typeof(uint),
                typeof(ulong),
                typeof(ushort),
                // char can be cast from any numeric type (integral or floating point
                // so we can generate chars via numeric generator
                typeof(char)
            };

            return numericTypes.Contains(type);
        }
    }
}
