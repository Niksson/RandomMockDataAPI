using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Options
{
    public class MockGeneratorOptions
    {
        public int DepthLimit { get; set; }
        public int MasterSeed { get; set; }
    }
}
