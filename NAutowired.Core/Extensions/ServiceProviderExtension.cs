using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAutowired.Core.Extensions
{
    /// <summary>
    /// 解析服务扩展类，从容器获取服务并解析其Autowired依赖。
    /// </summary>
    public static class ServiceProviderExtension
    {
        /// <summary>
        /// 解析指定实例的Autowired依赖
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="instance">已创建的服务实例</param>
        public static void WithAutowired(this IServiceProvider serviceProvider, object instance)
        {
            NAutowired.Core.DependencyInjection.Resolve(serviceProvider, instance);
        }

        /// <summary>
        /// 从容器获取服务并解析Autowired依赖
        /// </summary>
        /// <typeparam name="T">服务类型，可以为IEnumerable&lt;&gt;</typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T GetServiceWithAutowired<T>(this IServiceProvider serviceProvider)
        {
            T instance = serviceProvider.GetRequiredService<T>();
            NAutowired.Core.DependencyInjection.Resolve(serviceProvider, instance);
            return instance;
        }
    }
}

