using System;

namespace Pathfinder.Store.Application.Exceptions;

public class PathfinderApplicationException : Exception
{
    internal PathfinderApplicationException()
    {
    }

    internal PathfinderApplicationException(string message)
        : base(message)
    {
    }

    internal PathfinderApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}