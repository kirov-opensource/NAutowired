using System;

namespace NAutowired.Core.Models {
  public class DependencyInjectionModel {

    /// <summary>
    /// 类型
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 注入模式
    /// </summary>
    public DependencyInjectionModeEnum DependencyInjectionMode { get; set; }
  }
}
