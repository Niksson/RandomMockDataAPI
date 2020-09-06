using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories
{
    public interface IMockDataRepository
    {
        public IEnumerable<object> GetMockObjects(string typeName, int skip, int take);
    }
}
