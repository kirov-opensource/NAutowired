using NAutowired.Core.Attributes;
using NAutowired.Core.Exceptions;
using NAutowired.Core.Extensions;
using NAutowired.Core.Models;
using System;
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
            typeof(T).InvokeConstructor(typeInstance);
        }

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="instanceScopeModel"></param>
        private static void ResolveDependencyTree(IServiceProvider serviceProvider, InstanceScopeModel instanceScopeModel)
        {
            foreach (var fieldInfo in instanceScopeModel.Instance.GetType().GetFullFields())
            {
                //判断当前属性是否具有DependencyInjectionAttribute特性
                var customeAttribute = fieldInfo.GetCustomAttribute(autowiredAttributeType, false);
                if (customeAttribute == null)
                {
                    continue;
                }

                var type = ((AutowiredAttribute)customeAttribute).RealType ?? fieldInfo.FieldType;
                var value = GetInstance(instanceScopeModel, type);
                //从parent instance 还原
                if (value != null)
                {
                    fieldInfo.SetValue(instanceScopeModel.Instance, value);
                    continue;
                }
                //从容器拿到Instance
                value = serviceProvider.GetService(type);
                if (value == null)
                {
                    throw new UnableResolveDependencyException($"Unable to resolve dependency {fieldInfo.FieldType.FullName}");
                }
                //将Instance赋值给属性
                fieldInfo.SetValue(instanceScopeModel.Instance, value);
                //构建下一个节点
                var nextInstanceScopeModel = new InstanceScopeModel
                {
                    Instance = value,
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
