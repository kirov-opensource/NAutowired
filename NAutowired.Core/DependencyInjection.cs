using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core.Attributes;
using NAutowired.Core.Exceptions;
using NAutowired.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace NAutowired.Core
{
    public static class DependencyInjection
    {
        private readonly static Type autowiredAttributeType = typeof(AutowiredAttribute);
        private static ConcurrentDictionary<Type, IList<MemberInfo>> typeMemberCache = new ConcurrentDictionary<Type, IList<MemberInfo>>();

        /// <summary>
        /// 属性依赖注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="typeInstance"></param>
        public static void Resolve<T>(IServiceProvider serviceProvider, T typeInstance)
        {
            // 创建一个队列，用于存储需要注入属性的对象
            Queue<object> targetQueue = new Queue<object>();
            targetQueue.Enqueue(typeInstance);
            // 已解析的实例集合，用于处理循环依赖
            HashSet<object> instanceSet = new HashSet<object>() { typeInstance };
            while (targetQueue.Count > 0)
            {
                var targetObject = targetQueue.Dequeue();
                var members = typeMemberCache.GetOrAdd(targetObject.GetType(), (type) =>
                {
                    return type.GetFullMembers();
                });
                foreach (var memberInfo in members)
                {
                    var customeAttribute = memberInfo.GetCustomAttribute(autowiredAttributeType, false);
                    var memberType = ((AutowiredAttribute)customeAttribute).RealType ?? memberInfo.GetRealType();
                    object instance = null;
                    bool memberIsEnumerable = typeof(IEnumerable).IsAssignableFrom(memberType) && memberType.IsGenericType;
                    if (memberIsEnumerable)
                    {
                        Type elementType = memberType.GetGenericArguments()[0];
                        instance = serviceProvider.GetServices(elementType);
                        memberInfo.SetValue(targetObject, instance);
                        // 如果是不存在的新实例，则添加到待解析依赖队列
                        if (!instanceSet.Contains(instance))
                        {
                            instanceSet.Add(instance);
                            foreach (object t in (instance as IEnumerable<object>))
                            {
                                targetQueue.Enqueue(t);
                            }
                        }
                    }
                    else
                    {
                        instance = serviceProvider.GetService(memberType);
                        if (instance == null)
                        {
                            throw new UnableResolveDependencyException($"Unable to resolve dependency {memberType.FullName}");
                        }

                        memberInfo.SetValue(targetObject, instance);
                        if (!instanceSet.Contains(instance))
                        {
                            instanceSet.Add(instance);
                            targetQueue.Enqueue(instance);
                        }
                    }
                }
            }
        }
    }
}
