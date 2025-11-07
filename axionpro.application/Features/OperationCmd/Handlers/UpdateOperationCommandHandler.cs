using AutoMapper;

using axionpro.application.Features.TransportCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.Features.OperationCmd.Commands;
using axionpro.application.DTOs.Operation;

namespace axionpro.application.Features.OperationCmd.Handlers
{
    public class UpdateOperationCommandHandler : IRequestHandler<UpdateOperationCommand, ApiResponse<List<GetOperationResponseDTO>>>
    {
        private readonly IOperationRepository operationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOperationCommandHandler(IOperationRepository operationRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.operationRepository = operationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<GetOperationResponseDTO>>> Handle(UpdateOperationCommand request, CancellationToken cancellationToken)
        {

            try
            {

                Operation operation = _mapper.Map<Operation>(request.updateOperationDTO);
                operation.UpdatedById = (long)request.updateOperationDTO.ProductOwnerId;
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


    }
}