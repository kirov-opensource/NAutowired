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

    private static List<DependencyInjectionModel> DependencyInjections { get; set; }
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
      DependencyInjections = new List<DependencyInjectionModel>();

      foreach (var type in types) {
        //循环attribute
        foreach (var attribute in type.GetCustomAttributes(false)) {
          if (attribute is ServiceAttribute) {
            DependencyInjections.Add(new DependencyInjectionModel {
              DependencyInjectionMode = ((ServiceAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          } else if (attribute is RepositoryAttribute) {
            DependencyInjections.Add(new DependencyInjectionModel {
              DependencyInjectionMode = ((RepositoryAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          } else if (attribute is ComponentAttribute) {
            DependencyInjections.Add(new DependencyInjectionModel {
              DependencyInjectionMode = ((ComponentAttribute)attribute).DependencyInjectionMode,
              Type = type
            });
            break;
          } else if (attribute is FilterAttribute) {
            DependencyInjections.Add(new DependencyInjectionModel {
              DependencyInjectionMode = ((FilterAttribute)attribute).DependencyInjectionMode,
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
    /// <param name="dependencyInjectionTreeModel"></param>
    private static void AnalysisDependencyInjectionTree(DependencyInjectionTreeModel dependencyInjectionTreeModel) {
      foreach (var memberInfo in dependencyInjectionTreeModel.DependencyInjection.Type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
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
        if (injectionType == dependencyInjectionTreeModel.DependencyInjection.Type) {
          throw new UnableResolveDependencyException($"Unable to resolve dependency {injectionType.FullName}.");
        }
        //从父树查找是否已经有依赖
        var parentInjectionTreeModel = GetDependencyInjectionTree(dependencyInjectionTreeModel.ParentDependencyInjectionTree, injectionType);
        if (parentInjectionTreeModel != null) {
          //查找父树的依赖是否是Transient模式,如是则此循环依赖无法支持
          if (parentInjectionTreeModel.DependencyInjection.DependencyInjectionMode == DependencyInjectionModeEnum.Transient) {
            throw new UnableResolveDependencyException($"Unable to resolve dependency {injectionType.FullName}. {parentInjectionTreeModel.DependencyInjection.Type.FullName} DependencyInjectionMode should not be Transient when using circular dependencies");
          }
          continue;
        }
        //查看是否加入到了容器
        var dependencyInjection = DependencyInjections.FirstOrDefault(item => item.Type == injectionType);
        if (dependencyInjection == null) {
          //虽然未找到实例,但是可能通过IServiceCollection加入到了容器
          continue;
        }
        var nextDependencyInjectionTreeModel = new DependencyInjectionTreeModel {
          DependencyInjection = dependencyInjection,
          ParentDependencyInjectionTree = dependencyInjectionTreeModel
        };
        AnalysisDependencyInjectionTree(nextDependencyInjectionTreeModel);
      }
    }

    /// <summary>
    /// 递归查找父节点
    /// </summary>
    /// <param name="dependencyInjectionTreeModel"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static DependencyInjectionTreeModel GetDependencyInjectionTree(DependencyInjectionTreeModel dependencyInjectionTreeModel, Type type) {
      if (dependencyInjectionTreeModel == null) { return null; }
      if (dependencyInjectionTreeModel.DependencyInjection.Type == type) {
        return dependencyInjectionTreeModel;
      }
      if (dependencyInjectionTreeModel.ParentDependencyInjectionTree == null) { return null; }
      return GetDependencyInjectionTree(dependencyInjectionTreeModel.ParentDependencyInjectionTree, type);
    }


    private static void AddDependencyInjection(IServiceCollection services) {
      foreach (var dependencyInjection in DependencyInjections) {
        AnalysisDependencyInjectionTree(new DependencyInjectionTreeModel { DependencyInjection = dependencyInjection });
        switch (dependencyInjection.DependencyInjectionMode) {
          case DependencyInjectionModeEnum.Transient:
            services.AddTransient(dependencyInjection.Type);
            break;
          case DependencyInjectionModeEnum.Scoped:
            services.AddScoped(dependencyInjection.Type);
            break;
          case DependencyInjectionModeEnum.Singleton:
            services.AddSingleton(dependencyInjection.Type);
            break;
        }
      }

    }

  }
}
