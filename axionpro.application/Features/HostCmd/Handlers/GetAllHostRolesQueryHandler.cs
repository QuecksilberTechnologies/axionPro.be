// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves all host roles that are not soft deleted.
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
    /// Represents the request to retrieve all host roles.
    /// </summary>
    public class GetAllHostRolesQuery
        : IRequest<ApiResponse<List<GetHostRoleResponseDTO>>>
    {
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles retrieval of all host roles.
    /// </summary>
    public class GetAllHostRolesQueryHandler
        : IRequestHandler<
            GetAllHostRolesQuery,
            ApiResponse<List<GetHostRoleResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllHostRolesQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllHostRolesQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work used to retrieve host roles.</param>
        /// <param name="logger">The logger used to record handler activity.</param>
        public GetAllHostRolesQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllHostRolesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Retrieves all host roles.
        /// </summary>
        /// <param name="request">The query to handle.</param>
        /// <param name="cancellationToken">A token to observe while handling the query.</param>
        /// <returns>A response containing all non-soft-deleted host roles.</returns>
        public async Task<ApiResponse<List<GetHostRoleResponseDTO>>> Handle(
            GetAllHostRolesQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all host roles.");

            var hostRoles = await _unitOfWork.HostRoleRepository
                .GetAllAsync();

            var response = hostRoles
                .Select(hostRole => new GetHostRoleResponseDTO
                {
                    Id = hostRole.Id,
                    Name = hostRole.Name,
                    Description = hostRole.Description,
                    IsActive = hostRole.IsActive,
                    AddedDateTime = hostRole.AddedDateTime,
                    UpdatedDateTime = hostRole.UpdatedDateTime
                })
                .ToList();

            return ApiResponse<List<GetHostRoleResponseDTO>>.Success(
                response,
                "Host roles retrieved successfully.");
        }

        #endregion
    }

    #endregion
}
