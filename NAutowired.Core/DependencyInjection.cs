using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core.Attributes;
using NAutowired.Core.Exceptions;
using NAutowired.Core.Extensions;
using NAutowired.Core.Models;
using System;
using System.Collections;
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
                var memberType = ((AutowiredAttribute)customeAttribute).RealType ?? memberInfo.GetRealType();
                var instance = GetInstance(instanceScopeModel, memberType);
                //如果依赖树能找到,则说明此处含有循环依赖,从依赖树还原
                //从parent instance 还原
                if (instance != null)
                {
                    memberInfo.SetValue(instanceScopeModel.Instance, instance);
                    continue;
                }

                bool memberIsEnumerable = typeof(IEnumerable).IsAssignableFrom(memberType) && memberType.IsGenericType;
                if (memberIsEnumerable)
                {
                    Type elementType = memberType.GetGenericArguments()[0];
                    instance = serviceProvider.GetServices(elementType);
                }
                else
                {
                    instance = serviceProvider.GetServices(memberType)?.FirstOrDefault();
                }

                if (instance == null)
                {
                    throw new UnableResolveDependencyException($"Unable to resolve dependency {memberType.FullName}");
                }

                //将Instance赋值给属性
                memberInfo.SetValue(instanceScopeModel.Instance, instance);

                foreach (var instanceElement in (IEnumerable)(memberIsEnumerable ? instance : new object[] { instance }))
                {
                    //构建下一个节点
                    var nextInstanceScopeModel = new InstanceScopeModel
                    {
                        Instance = instanceElement,
                        ParentInstanceScope = instanceScopeModel
                    };
                    //递归注入的属性是否有其它依赖
                    ResolveDependencyTree(serviceProvider, nextInstanceScopeModel);
                }
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
            if (type.IsInterface && type.IsAssignableFrom(instanceScopeModel.Instance.GetType()) ||
                instanceScopeModel.Instance.GetType() == type)
            {
                return instanceScopeModel.Instance;
            }
            if (instanceScopeModel.ParentInstanceScope == null) { return null; }
            return GetInstance(instanceScopeModel.ParentInstanceScope, type);
        }
    }
}
