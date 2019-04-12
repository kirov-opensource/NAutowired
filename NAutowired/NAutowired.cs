using NAutowired.Attributes;
using NAutowired.Exceptions;
using System;
using System.Reflection;

namespace NAutowired {
  public static class NAutowired {

    /// <summary>
    /// 循环解析依赖
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <param name="typeInstance"></param>
    public static void RecursionDependencyInjection<T>(IServiceProvider serviceProvider, T typeInstance) {
      foreach (var propertity in typeInstance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        //判断当前属性是否具有DependencyInjectionAttribute特性
        var customeAttribute = propertity.GetCustomAttribute(typeof(AutowiredAttribute), false);
        if (customeAttribute == null) {
          continue;
        }
        //从容器拿到Instance
        var value = serviceProvider.GetService(((AutowiredAttribute)customeAttribute).RealType ?? propertity.PropertyType);
        if (value == null) {
          throw new UnableResolveDependencyException($"Unable to resolve dependency {propertity.PropertyType.FullName}");
        }
        //将Instance赋值给属性
        propertity.SetValue(typeInstance, value);
        //递归注入的属性是否有其它依赖
        RecursionDependencyInjection(serviceProvider, value);
      }
    }
  }
}
