using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core;
using NAutowired.Core.Attributes;
using NAutowired.Core.Models;
using NAutowired.Exceptions;
using NAutowired.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NAutowired {
  public static class ServiceCollectionExtensions {

    private static List<DependencyModel> container { get; set; }
    private static bool Flag = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies">assembly name</param>
    public static void AddAutoDependencyInjection(this IServiceCollection services, IList<string> assemblies) {
      if (Flag) {
        throw new AddDependencyInjectionException($"Do not add dependencies repeatedly");
      }
      Flag = true;
      if (assemblies == null || !assemblies.Any()) {
        return;
      }
      var types = new List<Type>();
      foreach (var assembly in assemblies) {
        //拿到程序集下所有类
        types.AddRange(Assembly.Load(assembly).GetTypes());
      }
      container = new List<DependencyModel>();
      foreach (var type in types) {
        //循环attribute
        foreach (var attribute in type.GetCustomAttributes(false)) {
          if (attribute is ServiceAttribute) {
            container.Add(new DependencyModel {
              Lifetime = ((ServiceAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          } else if (attribute is RepositoryAttribute) {
            container.Add(new DependencyModel {
              Lifetime = ((RepositoryAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          } else if (attribute is ComponentAttribute) {
            container.Add(new DependencyModel {
              Lifetime = ((ComponentAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          } else if (attribute is FilterAttribute) {
            container.Add(new DependencyModel {
              Lifetime = ((FilterAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          }
        }
      }

      AddDependencyInjection(services);
    }

    /// <summary>
    /// 分析依赖树
    /// </summary>
    /// <param name="dependencyTreeModel"></param>
    private static void AnalysisDependencyTree(DependencyTreeModel dependencyTreeModel) {
      dependencyTreeModel.Dependency.Type.BaseType
      foreach (var memberInfo in dependencyTreeModel.Dependency.Type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        //非属性和字段
        if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property) {
          continue;
        }
        //判断当前属性是否具有DependencyInjectionAttribute特性
        var customeAttribute = memberInfo.GetCustomAttribute(typeof(AutowiredAttribute), false);
        if (customeAttribute == null) {
          continue;
        }
        //等待注入的类型
        var injectionType = ((AutowiredAttribute)customeAttribute).RealType ?? memberInfo.GetRealType();
        //自己依赖自己
        if (injectionType == dependencyTreeModel.Dependency.Type) {
          throw new UnableResolveDependencyException($"Unable to resolve dependency {injectionType.FullName}.");
        }
        //从父树查找是否已经有依赖
        var parentDependencyTreeModel = GetDependencyTree(dependencyTreeModel.ParentDependencyTree, injectionType);
        if (parentDependencyTreeModel != null) {
          //查找父树的依赖是否是Transient模式,如是则此循环依赖无法支持
          if (parentDependencyTreeModel.Dependency.Lifetime == Lifetime.Transient) {
            throw new UnableResolveDependencyException($"Unable to resolve dependency {injectionType.FullName}. {parentDependencyTreeModel.Dependency.Type.FullName} DependencyInjectionMode should not be Transient when using circular dependencies");
          }
          continue;
        }
        //查看是否加入到了容器
        var dependency = container.FirstOrDefault(item => item.Type == injectionType);
        if (dependency == null) {
          //虽然未找到实例,但是可能通过IServiceCollection加入到了容器
          continue;
        }
        var nextDependencyInjectionTreeModel = new DependencyTreeModel {
          Dependency = dependency,
          ParentDependencyTree = dependencyTreeModel
        };
        AnalysisDependencyTree(nextDependencyInjectionTreeModel);
      }
    }

    /// <summary>
    /// 递归查找父节点
    /// </summary>
    /// <param name="dependencyTreeModel"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static DependencyTreeModel GetDependencyTree(DependencyTreeModel dependencyTreeModel, Type type) {
      if (dependencyTreeModel == null) { return null; }
      if (dependencyTreeModel.Dependency.Type == type) {
        return dependencyTreeModel;
      }
      if (dependencyTreeModel.ParentDependencyTree == null) { return null; }
      return GetDependencyTree(dependencyTreeModel.ParentDependencyTree, type);
    }

    private static void AddDependencyInjection(IServiceCollection services) {
      foreach (var dependencyInjection in container) {
        AnalysisDependencyTree(new DependencyTreeModel { Dependency = dependencyInjection });
        switch (dependencyInjection.Lifetime) {
          case Lifetime.Transient:
            services.AddTransient(dependencyInjection.Type);
            break;
          case Lifetime.Scoped:
            services.AddScoped(dependencyInjection.Type);
            break;
          case Lifetime.Singleton:
            services.AddSingleton(dependencyInjection.Type);
            break;
        }
      }

    }

  }
}
