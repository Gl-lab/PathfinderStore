using System;

namespace Pathfinder.Infrastructure.Exceptions
{
    public class InfrastructureException : Exception
    {
        internal InfrastructureException()
        {
        }

        internal InfrastructureException(string message)
            : base(message)
        {
        }

        internal InfrastructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
