using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BowlingGame.Tests.Infrastructure
{
    public class RepeatAttribute : Xunit.Sdk.DataAttribute
    {
        private readonly int _start;
        private readonly int _count;

        public RepeatAttribute(int start, int count)
        {
            _start = start;
            _count = count;
        }


        public override IEnumerable<Object[]> GetData(System.Reflection.MethodInfo testMethod)
        {
            foreach (var iterationNumber in Enumerable.Range(_start, _count))
            {
                yield return new object[] { iterationNumber };
            }
        }
    }
}
