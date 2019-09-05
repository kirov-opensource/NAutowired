using NAutowired.Core.Models;
using System;

namespace NAutowired.Core.Attributes {
  [AttributeUsage(AttributeTargets.Class)]
  public class ComponentAttribute : Attribute {

    public Lifetime DependencyInjectionMode {
      get;
    } = Lifetime.Scoped;

    public ComponentAttribute() {
    }


    public ComponentAttribute(Lifetime dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }

  }
}
