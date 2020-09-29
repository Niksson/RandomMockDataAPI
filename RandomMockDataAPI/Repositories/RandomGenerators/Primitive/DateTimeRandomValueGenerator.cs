using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMockDataAPI.Repositories.RandomGenerators
{
    public class DateTimeRandomValueGenerator : PrimitiveRandomValueGenerator<DateTime> 
    {

        public DateTimeRandomValueGenerator(int seed) : base(seed)
        {
        }

        protected override DateTime GenerateRandomValue()
        {
            var start = new DateTime(2010, 1, 1);
            var end = new DateTime(2025, 1, 1);
            var range = (end - start).Days;

            return start.AddDays(_generator.Next(range));
        }
    }
}
