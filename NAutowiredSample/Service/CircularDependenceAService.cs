using NAutowired.Core.Attributes;
using System;

namespace NAutowiredSample.Service {
  [Service]
  public class CircularDependenceAService {

    [Autowired]
    private CircularDependenceBService CyclicDependenceBService { get; set; }

    public string GetCyclicDependenceA() {
      Console.WriteLine($"{CyclicDependenceBService.ToString()} in CyclicDependenceAService");
      return $"CyclicDependenceA";
    }
  }
}
