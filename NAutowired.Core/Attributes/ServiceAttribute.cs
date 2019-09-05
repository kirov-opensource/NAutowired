using NAutowired.Core.Models;
using System;

namespace NAutowired.Core.Attributes {
  [AttributeUsage(AttributeTargets.Class)]
  public class ServiceAttribute : Attribute {
    public Lifetime DependencyInjectionMode {
      get;
    } = Lifetime.Scoped;

    public ServiceAttribute() {
    }

    public ServiceAttribute(Lifetime dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }
  }
}
