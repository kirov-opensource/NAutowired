using NAutowired.Core.Attributes;
using System.Collections.Generic;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class EnumerableMultipleImplementService
    {
        [Autowired]
        private readonly IEnumerable<IMultipleImplement> multipleImplements;

        public IEnumerable<IMultipleImplement> SayHello()
        {
            return multipleImplements;
        }

    }
}
