using System;
using System.Collections.Generic;
using System.Reflection;

namespace NAutowired.Core.Extensions {
  internal static class TypeExtension {

    private readonly static Type baseType = typeof(object);

    internal static IList<FieldInfo> GetFullFields(this Type type) {
      return GetFullFields(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, new List<FieldInfo>());
    }

    private static IList<FieldInfo> GetFullFields(this Type type, BindingFlags bindingFlags, List<FieldInfo> fieldInfos) {
      fieldInfos.AddRange(type.GetFields(bindingFlags));
      if (type.BaseType == baseType) {
        return fieldInfos;
      }
      return GetFullFields(type.BaseType, bindingFlags, fieldInfos);
    }

  }
}
