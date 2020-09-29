using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RandomMockDataAPI.Repositories
{
    public class TypeFinder
    {
        private readonly IMemoryCache _cache;

        public TypeFinder(IMemoryCache cache)
        {
            _cache = cache;
        }

#nullable enable
        /// <summary>
        /// This methods scans all assemblies in the current assembly directory.
        /// This method will return null if type isn't found or doesn't have a parameterless constructor
        /// </summary>
        /// <param name="typeName">Name of type to look for</param>
        /// <returns></returns>
        public Type? GetTypeFromAssemblies(string typeName)
        {
            #region Get from cache
            // First, let's try to find the type in cache
            Type type;

            if(_cache.TryGetValue(typeName.ToLower(), out type)) return type;

            // If not found, we'll need to try to find the type within assemblies
            #endregion

            // Get files from the executable directory

            var assemblyLocation = Assembly.GetExecutingAssembly().Location;

            var dir = new DirectoryInfo(Path.GetDirectoryName(assemblyLocation));

            var files = dir.GetFiles();

            // Filter out all files which don't have .dll extension or where file name is equal to entry assembly name
            files = files.Where(f =>
            {
                return Regex.IsMatch(f.Name, @".*\.dll") && Path.GetFileNameWithoutExtension(f.Name) != Assembly.GetExecutingAssembly().GetName().Name;
            }).ToArray();

            // Let's try to find the needed type in file with name equal to typeName
            var matchingFile = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name).ToLower() == typeName);
            if(matchingFile != null)
            {
                var types = GetLoadableTypes(matchingFile);
                type = types.FirstOrDefault(t => t.Name.ToLower() == typeName.ToLower());
                if (type != null) return type;
            }

            // in other case let's try to find it in other files
            files.Where(f => f != matchingFile).ToArray();

            foreach(var f in files)
            {
                var types = GetLoadableTypes(f);
                type = types.FirstOrDefault(t => t.Name.ToLower() == typeName.ToLower());

                // Ignore the type without parameterless constructor
                if (type != null && type.GetConstructor(Type.EmptyTypes) != null)  return type;
            }

            // Everything failed! Return null
            return null;

        }

        private IEnumerable<Type>? GetLoadableTypes(FileInfo path)
        {
            var assembly = Assembly.LoadFile(path.FullName);
            Type[]? types;
            try
            {
                types = assembly.GetTypes();
            }
            catch(ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            return types;

        }
    }
}
