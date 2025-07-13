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
    public class InvalidUserCreatedException : Exception
    {
        public InvalidUserCreatedException(string message) : base(message) { }
    }
    public class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException(string message) : base(message) { }
    }
    public class InvalidAccessSession : Exception
    {
        public InvalidAccessSession(string message) : base(message) { }
    }
    public class InvalidDataInput : Exception
    {
        public InvalidDataInput(string message) : base(message) { }
    }

    public class InvalidAccessBookingAvailability : Exception
    {
        public InvalidAccessBookingAvailability(string message) : base(message) { }
    }
    public class InvalidAccessUserException : Exception
    {
        public InvalidAccessUserException(string message) : base(message) { }
    }
    public class BookingAvaiOverlapEx : Exception
    {
        public BookingAvaiOverlapEx(string message) : base(message) { }
    }
    public class BookingAvaiDuplicateEx : Exception
    {
        public BookingAvaiDuplicateEx(string message) : base(message) { }
    }


}