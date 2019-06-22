using NAutowired.Core.Attributes;
using System;

namespace NAutowiredSample.Service {
  [Service]
  public class CircularDependenceBService {

    [Autowired]
    private CircularDependenceAService CyclicDependenceAService { get; set; }

    public string GetCyclicDependenceB() {
      Console.WriteLine($"{CyclicDependenceAService.ToString()} in CyclicDependenceBService");
      return $"CyclicDependenceB";
    }
  }
}
