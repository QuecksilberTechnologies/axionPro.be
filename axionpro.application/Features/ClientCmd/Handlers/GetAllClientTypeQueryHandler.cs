// ===============================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves client types.
// ===============================================================

using axionpro.application.DTOs.Client;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.ClientCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Features.ClientCmd.Queries;
using axionpro.application.Features.LeaveCmd.Handlers;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ClientCmd.Queries
{
    #region Query

    /// <summary>
    /// Represents the request to retrieve client types.
    /// </summary>
    public class GetClientTypeQuery : IRequest<ApiResponse<List<GetClientTypeDTO>>>
    {
        public ClientRequestTypeDTO clientTypeRequestDTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetClientTypeQuery"/> class.
        /// </summary>
        public GetClientTypeQuery(ClientRequestTypeDTO clientTypeRequestDTO)
        {
            this.clientTypeRequestDTO = clientTypeRequestDTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.ClientCmd.Handlers
{
/// <summary>
/// Handles the GetClientTypeQuery request.
/// </summary>
internal class GetAllClientTypeQueryHandler : IRequestHandler<GetClientTypeQuery, ApiResponse<List<GetClientTypeDTO>>>
    {
#region Fields

       // private readonly IClientRepository _clienttypeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllLeaveRuleQueryHandler> _logger;

#endregion

#region Constructor

/// <summary>
/// Initializes a new instance of the <see cref="GetAllClientTypeQueryHandler"/> class.
/// </summary>



        public GetAllClientTypeQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllLeaveRuleQueryHandler> logger)      
        {
           
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

#endregion

#region Handler

/// <summary>
/// Handles the request asynchronously.
/// </summary>
/// <param name="request">The request to process.</param>
/// <param name="cancellationToken">A token used to cancel the operation.</param>
/// <returns>The response produced by handling the request.</returns>


        public async Task<ApiResponse<List<GetClientTypeDTO>>> Handle(GetClientTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Correcting the method call
                List<ClientType> clientTypes = await _unitOfWork.ClientsRepository.GetAllClientTypeAsync();

                //if (clientTypes == null || !clientTypes.Any())
                //{
                //    _logger.LogWarning("No clientTypes found.");
                //    return new ApiResponse<List<GetAllClientTypeDTO>>(false, "No clientTypes found", new List<GetAllClientTypeQuery>());
                //}

                //// ✅ Map Role entities to DTOs
                var getAllClientTypeDTOs = _mapper.Map<List<GetClientTypeDTO>>(clientTypes);

                _logger.LogInformation("Successfully retrieved {Count} roles.", getAllClientTypeDTOs.Count);
                return new ApiResponse<List<GetClientTypeDTO>>
                {
                    IsSucceeded = true,
                    Message = "Categories fetched successfully.",
                    Data = getAllClientTypeDTOs
                };
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching roles.");
                return new ApiResponse<List<GetClientTypeDTO>>
                {
                    IsSucceeded = false,
                    Message = "Categories fetched successfully.",
                    Data = null
                };
            }

        }
    
#endregion
}
}
