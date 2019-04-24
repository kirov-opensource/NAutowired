using NAutowired.Core.Attributes;
using NAutowired.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NAutowired {
  public static class NAutowired {

    /// <summary>
    /// 属性依赖注入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <param name="typeInstance"></param>
    public static void FieldDependencyInjection<T>(IServiceProvider serviceProvider, T typeInstance) {
      AnalysisDependencyInjection(serviceProvider, typeInstance, new List<object>());
    }

    /// <summary>
    /// 分析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <param name="typeInstance"></param>
    /// <param name="resolvedInstances"></param>
    private static void AnalysisDependencyInjection<T>(IServiceProvider serviceProvider, T typeInstance, IList<object> resolvedInstances) {
      foreach (var propertity in typeInstance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        //判断当前属性是否具有DependencyInjectionAttribute特性
        var customeAttribute = propertity.GetCustomAttribute(typeof(AutowiredAttribute), false);
        if (customeAttribute == null) {
          continue;
        }
        var type = ((AutowiredAttribute)customeAttribute).RealType ?? propertity.PropertyType;
        //从以还原集合拿到Instance
        var value = resolvedInstances.FirstOrDefault(item => item.GetType() == type);
        if (value != null) {
          //将Instance赋值给属性
          propertity.SetValue(typeInstance, value);
          continue;
        }
        //从容器拿到Instance
        value = serviceProvider.GetService(type);
        if (value == null) {
          throw new UnableResolveDependencyException($"Unable to resolve dependency {propertity.PropertyType.FullName}");
        }
        resolvedInstances.Add(value);
        //将Instance赋值给属性
        propertity.SetValue(typeInstance, value);
        //递归注入的属性是否有其它依赖
        AnalysisDependencyInjection(serviceProvider, value, resolvedInstances);
      }
    }
  }
}
