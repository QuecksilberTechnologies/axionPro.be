// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Update Operation.
// ================================================================

using axionpro.application.DTOs.Operation;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Features.OperationCmd.Commands;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;

namespace axionpro.application.Features.OperationCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to Update Operation.
    /// </summary>
public class UpdateOperationCommand : IRequest<ApiResponse<List<GetOperationResponseDTO>>>
    {

        public UpdateOperationRequestDTO updateOperationDTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperationCommand"/> class.
        /// </summary>

        public UpdateOperationCommand(UpdateOperationRequestDTO updateOperationDTO)
        {
            this.updateOperationDTO = updateOperationDTO;
        }

    }

    #endregion
}

namespace axionpro.application.Features.OperationCmd.Handlers
{
    /// <summary>
    /// Handles the request to Update Operation.
    /// </summary>
public class UpdateOperationCommandHandler : IRequestHandler<UpdateOperationCommand, ApiResponse<List<GetOperationResponseDTO>>>
    {
        #region Fields

        private readonly IOperationRepository operationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperationCommandHandler"/> class.
        /// </summary>


        public UpdateOperationCommandHandler(
            IOperationRepository operationRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService)
        {
            this.operationRepository = operationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied UpdateOperationCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<GetOperationResponseDTO>>> Handle(UpdateOperationCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var hostContext =
                    await _commonRequestService.ValidateHostSuperAdminRequestAsync();
                var hostUserId = hostContext.HostUserId;

                var dto = request?.updateOperationDTO
                    ?? throw new ValidationErrorException("Operation details are required.");

                var existingOperation = await operationRepository
                    .GetOperationByIdAsync(dto.Id)
                    ?? throw new ApiException("Operation not found.", 404);

                Operation operation = _mapper.Map<Operation>(dto);
                operation.IsActive = dto.IsActive ?? existingOperation.IsActive;
                operation.UpdatedById = hostUserId;
                operation.UpdateDateTime = DateTime.UtcNow;
                List<Operation> operations = await operationRepository.UpdateOperationAsync(operation);

                if (operations == null || !operations.Any())
                {
                    return new ApiResponse<List<GetOperationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No operation were updated.",
                        Data = new List<GetOperationResponseDTO>()
                    };
                }

                List<GetOperationResponseDTO> getAllOperationDTOs = _mapper.Map<List<GetOperationResponseDTO>>(operations);

                return new ApiResponse<List<GetOperationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Travel created successfully",
                    Data = getAllOperationDTOs
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error occurred while Updatiing Operation.");
                return new ApiResponse<List<GetOperationResponseDTO>>

                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }


    
        #endregion
}
}
