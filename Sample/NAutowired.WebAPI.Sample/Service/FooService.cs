using NAutowired.Core.Attributes;
using NAutowired.WebAPI.Sample.Repository;

namespace NAutowired.WebAPI.Sample.Service {
    [Service]
    public class FooService : IFooService {

        [Autowired]
        private FooRepository FooRepository { get; set; }


        public string GetFoo() {
            System.Console.WriteLine($"{FooRepository.ToString()} in service");
            return "Foo";
        }
    }
}
