using System;

namespace axionpro.application.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }  // ✅ ye missing hai

        public ApiException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}