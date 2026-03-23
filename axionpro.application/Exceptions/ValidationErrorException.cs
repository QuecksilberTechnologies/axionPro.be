using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Exceptions
{
    public class ValidationErrorException : Exception
    {
        public List<string> Errors { get; }

        public ValidationErrorException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }

        public ValidationErrorException(string message, List<string> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
