using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMockDataAPI.Options
{
    public class MockGeneratorOptions
    {
        public uint DepthLimit { get; set; }
        public int MasterSeed { get; set; }
        public uint ObjectsPerRequest { get; set; }
    }
}
