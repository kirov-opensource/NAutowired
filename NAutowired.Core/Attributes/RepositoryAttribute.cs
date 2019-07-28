using System;

namespace NAutowired.Core.Attributes {
  [AttributeUsage(AttributeTargets.Class)]
  public class RepositoryAttribute : Attribute {
    public Lifetime DependencyInjectionMode {
      get;
    } = Lifetime.Scoped;

    public RepositoryAttribute() {
    }


    public RepositoryAttribute(Lifetime dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }
  }
}
