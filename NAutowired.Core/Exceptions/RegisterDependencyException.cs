using System;

namespace NAutowired.Core.Exceptions
{
    public class RegisterDependencyException : SystemException
    {

        public RegisterDependencyException()
        {

        }

        public RegisterDependencyException(string message) : base(message) { }

    }
}
