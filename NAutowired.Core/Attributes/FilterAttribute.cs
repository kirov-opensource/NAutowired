using System;

namespace NAutowired.Core.Attributes {

  [AttributeUsage(AttributeTargets.Class)]
  public class FilterAttribute : Attribute {
    public Lifetime DependencyInjectionMode {
      get;
    } = Lifetime.Scoped;

    public FilterAttribute() {
    }


    public FilterAttribute(Lifetime dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }
  }
}
