using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message) { }
    }
    public class InvalidRefreshToken : Exception
    {
        public InvalidRefreshToken(string message) : base(message) { }
    }
    public class NotFoundTokenFromClient : Exception
    {
        public NotFoundTokenFromClient(string message) : base(message) { }
    }
    public class EmptyTokenGoogleLoginException : Exception
    {
        public EmptyTokenGoogleLoginException(string message) : base(message) { }
    }
    public class InvalidCGoogleTokenException : Exception
    {
        public InvalidCGoogleTokenException(string message) : base(message) { }
    }
}