using System;

namespace Pathfinder.Application.Exceptions
{
    public class PathfiderApplicationException : Exception
    {
        internal PathfiderApplicationException()
        {
        }

        internal PathfiderApplicationException(string message)
            : base(message)
        {
        }

        internal PathfiderApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
