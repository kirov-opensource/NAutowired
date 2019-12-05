using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core.Attributes;
using NAutowired.Core.Exceptions;
using NAutowired.Core.Extensions;
using NAutowired.Core.Models;
using System;
using System.Linq;
using System.Reflection;

namespace NAutowired.Core
{
    public static class DependencyInjection
    {
        private readonly static Type autowiredAttributeType = typeof(AutowiredAttribute);

        /// <summary>
        /// 属性依赖注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="typeInstance"></param>
        public static void Resolve<T>(IServiceProvider serviceProvider, T typeInstance)
        {
            ResolveDependencyTree(serviceProvider, new InstanceScopeModel { Instance = typeInstance });
        }

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="instanceScopeModel"></param>
        private static void ResolveDependencyTree(IServiceProvider serviceProvider, InstanceScopeModel instanceScopeModel)
        {
            foreach (var memberInfo in instanceScopeModel.Instance.GetType().GetFullMembers())
            {
                var customeAttribute = memberInfo.GetCustomAttribute(autowiredAttributeType, false);
                var memberType = memberInfo.GetRealType();
                var realType = ((AutowiredAttribute)customeAttribute).RealType ?? memberType;
                var instance = GetInstance(instanceScopeModel, realType);
                //如果依赖树能找到,则说明此处含有循环依赖,从依赖树还原
                //从parent instance 还原
                if (instance != null)
                {
                    memberInfo.SetValue(instanceScopeModel.Instance, instance);
                    continue;
                }
                //从容器拿到Instance
                instance = serviceProvider.GetServices(memberType)?.FirstOrDefault(i => i.GetType() == realType);
                if (instance == null)
                {
                    throw new UnableResolveDependencyException($"Unable to resolve dependency {memberType.FullName} with {realType.FullName}");
                }
                //将Instance赋值给属性
                memberInfo.SetValue(instanceScopeModel.Instance, instance);
                //构建下一个节点
                var nextInstanceScopeModel = new InstanceScopeModel
                {
                    Instance = instance,
                    ParentInstanceScope = instanceScopeModel
                };
                //递归注入的属性是否有其它依赖
                ResolveDependencyTree(serviceProvider, nextInstanceScopeModel);
            }
        }

        /// <summary>
        /// 递归查找父节点
        /// </summary>
        /// <param name="instanceScopeModel"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object GetInstance(InstanceScopeModel instanceScopeModel, Type type)
        {
            if (instanceScopeModel.Instance.GetType() == type)
            {
                return instanceScopeModel.Instance;
            }
            if (instanceScopeModel.ParentInstanceScope == null) { return null; }
            return GetInstance(instanceScopeModel.ParentInstanceScope, type);
        }
    }
}
