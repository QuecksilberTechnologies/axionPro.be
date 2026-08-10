// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves all active host users that are not soft deleted.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.HostCmd.Handler
{
    #region Query

    /// <summary>
    /// Represents the request to retrieve all host users.
    /// </summary>
    public class GetAllHostUsersQuery
        : IRequest<ApiResponse<List<GetHostUserResponseDTO>>>
    {
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles retrieval of all host users.
    /// </summary>
    public class GetAllHostUsersQueryHandler
        : IRequestHandler<
            GetAllHostUsersQuery,
            ApiResponse<List<GetHostUserResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllHostUsersQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllHostUsersQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work used to retrieve host users.</param>
        /// <param name="logger">The logger used to record handler activity.</param>
        public GetAllHostUsersQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllHostUsersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Retrieves all host users.
        /// </summary>
        /// <param name="request">The query to handle.</param>
        /// <param name="cancellationToken">A token to observe while handling the query.</param>
        /// <returns>A response containing all non-soft-deleted host users.</returns>
        public async Task<ApiResponse<List<GetHostUserResponseDTO>>> Handle(
            GetAllHostUsersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all host users.");

            var hostUsers = await _unitOfWork.HostUserRepository
                .GetAllAsync();

            var response = hostUsers
                .Select(hostUser => new GetHostUserResponseDTO
                {
                    Id = hostUser.Id,
                    HostRoleId = hostUser.HostRoleId,
                    HostRoleName = hostUser.HostRole?.Name,
                    Name = hostUser.Name,
                    LoginId = hostUser.LoginId,
                    Email = hostUser.Email,
                    MobileNumber = hostUser.MobileNumber,
                    IsActive = hostUser.IsActive,
                    AddedDateTime = hostUser.AddedDateTime,
                    UpdatedDateTime = hostUser.UpdatedDateTime
                })
                .ToList();

            return ApiResponse<List<GetHostUserResponseDTO>>.Success(
                response,
                "Host users retrieved successfully.");
        }

        #endregion
    }

    #endregion
}
