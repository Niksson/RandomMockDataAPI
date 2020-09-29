using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMockDataAPI.Helpers
{
    public static class TypeCheckExtensions
    {

        /// <summary>
        /// This extension method returns true if the type is a numeric type (integral or real) or a char type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static bool IsNumericOrChar(this Type type)
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

        /// <summary>
        /// This extension method returns true if the type can be generated
        /// by primitive generators (numeric, string and DateTime)
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static bool IsPrimitive(this Type type)
        {
            return IsNumericOrChar(type) || type == typeof(string) || type == typeof(DateTime);
        }

        /// <summary>
        /// This extension method returns true if the type is array or if it implements IEnumerable
        /// or ICollection
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static bool IsCollection(this Type type)
        {
            return type.IsArray || type.GetInterface(nameof(IEnumerable)) != null ||
                   type.GetInterface(nameof(ICollection)) != null;
        }
    }
}
