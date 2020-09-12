using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Helpers
{
    public class TypeHelpers
    {

        /// <summary>
        /// Returns true if the provided type is a numeric type (integral or real) or a char type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static bool IsTypeNumericOrChar(Type type)
        {
            var numericTypes = new HashSet<Type>()
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
