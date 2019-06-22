using NAutowired.Exceptions;
using System;
using System.Reflection;

namespace NAutowired.Extensions {
  internal static class MemberInfoExtentsion {
    internal static void SetValue(this MemberInfo memberInfo, object obj, object value) {
      switch (memberInfo.MemberType) {
        case MemberTypes.Field:
          ((FieldInfo)memberInfo).SetValue(obj, value);
          break;
        case MemberTypes.Property:
          ((PropertyInfo)memberInfo).SetValue(obj, value);
          break;
        default:
          throw new UnableResolveDependencyException($"Unable to resolve dependency {memberInfo.ReflectedType.FullName}");
      }
    }

    internal static Type GetRealType(this MemberInfo memberInfo) {
      switch (memberInfo.MemberType) {
        case MemberTypes.Field:
          return ((FieldInfo)memberInfo).FieldType;
        case MemberTypes.Property:
          return ((PropertyInfo)memberInfo).PropertyType;
        default:
          throw new UnableResolveDependencyException($"Unable to resolve dependency {memberInfo.ReflectedType.FullName}");
      }
    }
  }
}
