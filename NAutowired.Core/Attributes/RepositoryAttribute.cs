using System;

namespace NAutowired.Core.Attributes {
  [AttributeUsage(AttributeTargets.Class)]
  public class RepositoryAttribute : Attribute {
    public DependencyInjectionModeEnum DependencyInjectionMode {
      get;
    } = DependencyInjectionModeEnum.Scoped;

    public RepositoryAttribute() {
    }


    public RepositoryAttribute(DependencyInjectionModeEnum dependencyInjectionMode) {
      this.DependencyInjectionMode = dependencyInjectionMode;
    }
  }
}
