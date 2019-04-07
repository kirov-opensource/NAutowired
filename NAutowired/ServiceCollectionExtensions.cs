using Microsoft.Extensions.DependencyInjection;
using NAutowired.Attributes;
using System;
using System.Reflection;

namespace NAutowired {
  public static class ServiceCollectionExtensions {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly">程序集</param>
    public static void AddAutoDependencyInjection(this IServiceCollection services, string assembly) {
      //拿到程序集下所有类
      var types = Assembly.Load(assembly).GetTypes();
      foreach (var type in types) {
        //循环自定义attribute
        foreach (var attribute in type.GetCustomAttributes(false)) {
          if (attribute is ServiceAttribute) {
            AddDependencyInjection(services, type, ((ServiceAttribute)attribute).DependencyInjectionMode);
            break;
          } else if (attribute is RepositoryAttribute) {
            AddDependencyInjection(services, type, ((RepositoryAttribute)attribute).DependencyInjectionMode);
            break;
          } else if (attribute is ComponentAttribute) {
            AddDependencyInjection(services, type, ((ComponentAttribute)attribute).DependencyInjectionMode);
            break;
          }
        }
      }
    }

    private static void AddDependencyInjection(IServiceCollection services, Type type, DependencyInjectionModeEnum dependencyInjectionMode) {
      switch (dependencyInjectionMode) {
        case DependencyInjectionModeEnum.Transient:
          services.AddTransient(type);
          break;
        case DependencyInjectionModeEnum.Scoped:
          services.AddScoped(type);
          break;
        case DependencyInjectionModeEnum.Singleton:
          services.AddSingleton(type);
          break;
      }
    }

  }
}
