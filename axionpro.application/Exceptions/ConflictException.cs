// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines business-conflict errors for centralized responses.
// ================================================================

using axionpro.application.Constants;
using System.Net;

namespace axionpro.application.Exceptions
{
    /// <summary>
    /// Represents an expected business conflict, such as a duplicate resource.
    /// </summary>
    public sealed class ConflictException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class.
        /// </summary>
        /// <param name="message">The safe public conflict message.</param>
        public ConflictException(string message)
            : base(AppConstants.ErrorCodes.Conflict, message, (int)HttpStatusCode.Conflict)
        {
        }
    }
}
