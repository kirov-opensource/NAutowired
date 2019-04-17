using System;

namespace NAutowired.Core {
  [AttributeUsage(AttributeTargets.Class)]
  public class ComponentAttribute : Attribute {

    public DependencyInjectionModeEnum DependencyInjectionMode {
      get;
    } = DependencyInjectionModeEnum.Scoped;

    public ComponentAttribute() {
    }


    public ComponentAttribute(DependencyInjectionModeEnum dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }

  }
}
