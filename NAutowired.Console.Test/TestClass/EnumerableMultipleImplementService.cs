using NAutowired.Core.Attributes;
using System.Collections.Generic;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class EnumerableMultipleImplementService
    {
        [Autowired]
        private readonly IEnumerable<IMultipleImplement> multipleImplements;

        public string SayHello()
        {
            string rtn = "";
            foreach(var impl in multipleImplements)
            {
                rtn += impl.SayHello() + ";";
            }
            return rtn;
        }

    }
}
