using System;

namespace NAutowired.Exceptions {
  public class AddDependencyInjectionException : SystemException {

    public AddDependencyInjectionException() {

    }

    public AddDependencyInjectionException(string message) : base(message) { }

  }
}
