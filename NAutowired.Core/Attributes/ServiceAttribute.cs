using System;

namespace NAutowired.Core.Attributes {
  [AttributeUsage(AttributeTargets.Class)]
  public class ServiceAttribute : Attribute {
    public DependencyInjectionModeEnum DependencyInjectionMode {
      get;
    } = DependencyInjectionModeEnum.Scoped;

    public ServiceAttribute() {
    }

    public ServiceAttribute(DependencyInjectionModeEnum dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }
  }
}
