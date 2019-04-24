using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NAutowired.Exceptions;
using System;

namespace NAutowired.Attributes {
  public class ServiceFilterAttribute : Attribute, IFilterFactory, IOrderedFilter {

    public Type ServiceType { get; private set; }

    public int Order { get; set; }

    bool IFilterFactory.IsReusable => false;

    public ServiceFilterAttribute(Type type) {
      ServiceType = type;
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) {
      var service = serviceProvider.GetRequiredService(ServiceType);
      if (service == null) {
        throw new UnableResolveDependencyException($"Unable to resolve dependency {ServiceType.FullName}");
      }
      var filter = service as IFilterMetadata;
      if (filter == null) {
        throw new NotImplementedException($"{ServiceType.FullName} not implement IFilterMetadata");
      }
      NAutowired.FieldDependencyInjection(serviceProvider, filter);
      return filter;
    }
  }
}
