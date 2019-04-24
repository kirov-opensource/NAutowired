using NAutowired.Core.Attributes;
using NAutowired.Core.Models;
using NAutowired.Exceptions;
using System;
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
      AnalysisDependencyInjection(serviceProvider, new InstanceScopeModel { Instance = typeInstance });
    }

    /// <summary>
    /// 分析
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="instanceScopeModel"></param>
    private static void AnalysisDependencyInjection(IServiceProvider serviceProvider, InstanceScopeModel instanceScopeModel) {
      foreach (var propertity in instanceScopeModel.Instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        //判断当前属性是否具有DependencyInjectionAttribute特性
        var customeAttribute = propertity.GetCustomAttribute(typeof(AutowiredAttribute), false);
        if (customeAttribute == null) {
          continue;
        }

        var type = ((AutowiredAttribute)customeAttribute).RealType ?? propertity.PropertyType;
        var value = GetInstance(instanceScopeModel, type);
        //从parent instance 还原
        if (value != null) {
          propertity.SetValue(instanceScopeModel.Instance, value);
          continue;
        }
        //从容器拿到Instance
        value = serviceProvider.GetService(type);
        if (value == null) {
          throw new UnableResolveDependencyException($"Unable to resolve dependency {propertity.PropertyType.FullName}");
        }
        //将Instance赋值给属性
        propertity.SetValue(instanceScopeModel.Instance, value);
        //构建下一个节点
        var nextInstanceScopeModel = new InstanceScopeModel {
          Instance = value,
          ParentInstanceScope = instanceScopeModel
        };
        //递归注入的属性是否有其它依赖
        AnalysisDependencyInjection(serviceProvider, nextInstanceScopeModel);
      }
    }

    /// <summary>
    /// 递归查找父节点
    /// </summary>
    /// <param name="instanceScopeModel"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static object GetInstance(InstanceScopeModel instanceScopeModel, Type type) {
      if (instanceScopeModel.Instance.GetType() == type) {
        return instanceScopeModel.Instance;
      }
      if (instanceScopeModel.ParentInstanceScope == null) { return null; }
      return GetInstance(instanceScopeModel.ParentInstanceScope, type);
    }
  }
}
