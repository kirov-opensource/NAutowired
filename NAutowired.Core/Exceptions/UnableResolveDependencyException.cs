using System;

namespace NAutowired.Core.Exceptions
{
    public class UnableResolveDependencyException : SystemException
    {

        public UnableResolveDependencyException()
        {

        }

        public UnableResolveDependencyException(string message) : base(message) { }

    }
}
