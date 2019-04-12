using System;

namespace NAutowired.Exceptions {
  public class UnableResolveDependencyException : SystemException {

    public UnableResolveDependencyException() {

    }

    public UnableResolveDependencyException(string message) : base(message) { }

  }
}
