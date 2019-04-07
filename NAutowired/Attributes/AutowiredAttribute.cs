using System;

namespace NAutowired.Attributes {
  [AttributeUsage(AttributeTargets.Property)]
  public class AutowiredAttribute : Attribute {

    public Type RealType {
      get;
    } = null;

    public AutowiredAttribute() {
    }

    /// <summary>
    /// 需要DI进的类型
    /// </summary>
    /// <param name="type"></param>
    public AutowiredAttribute(Type type) {
      this.RealType = type;
    }
  }
}
