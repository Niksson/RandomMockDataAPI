using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class DateTimeRandomValueGenerator : RandomValueGenerator<DateTime> 
    {
        private readonly Random _generator;

        public DateTimeRandomValueGenerator(int seed)
        {
            _generator = new Random(seed);
        }

        public override DateTime GetNext()
        {
            var start = new DateTime(2010, 1, 1);
            var end = new DateTime(2025, 1, 1);
            var range = (end - start).Days;

            return start.AddDays(_generator.Next(range));
        }
    }
}
