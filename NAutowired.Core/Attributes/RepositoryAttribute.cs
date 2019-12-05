using NAutowired.Core.Models;
using System;

namespace NAutowired.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryAttribute : Attribute
    {
        public Lifetime DependencyInjectionMode
        {
            get;
        } = Lifetime.Scoped;

        public Type ImplementInterface
        {
            get;
        } = null;

        public RepositoryAttribute()
        {
        }


        public RepositoryAttribute(Lifetime dependencyInjectionMode)
        {
            this.DependencyInjectionMode = dependencyInjectionMode;
        }
        public RepositoryAttribute(Type implementInterface)
        {
            this.ImplementInterface = implementInterface;
        }
        public RepositoryAttribute(Lifetime dependencyInjectionMode, Type implementInterface)
        {
            this.DependencyInjectionMode = dependencyInjectionMode;
            this.ImplementInterface = implementInterface;
        }
    }
}
