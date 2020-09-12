using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Helpers
{
    public class GenerationHelpers
    {
        // .NET Core makes default GetHashCode implementation generate different hashes between program runs

        /// <summary>
        /// Get a hash in a 64 bit system that will be the same between program runs
        /// </summary>
        /// <param name="str">String to generate the hash for</param>
        /// <returns></returns>
        public static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
