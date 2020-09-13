using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMockApi.Repositories.RandomGenerators
{
    public class StringRandomValueGenerator : RandomValueGenerator<string>
    {
        private readonly string _base = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
           
        public StringRandomValueGenerator(int seed) : base(seed)
        {
        }

        public override string GetNext()
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < 15; i++)
            {
                var c = _base[_generator.Next(0, _base.Length)];
                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }
}
