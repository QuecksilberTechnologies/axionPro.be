using AutoMapper;
 
using axionpro.application.Features.LeaveCmd.Commands;
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
    public class CreateOperationCommandHandler :  IRequestHandler<CreateOperationCommand, ApiResponse<List<GetOperationResponseDTO>>>
    {
        private readonly IOperationRepository _operationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOperationCommandHandler(IOperationRepository operationRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _operationRepository = operationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<List<GetOperationResponseDTO>>> Handle(CreateOperationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Operation operationEntity = _mapper.Map<Operation>(request.createOperationDTO);

                List<Operation> operations = await _operationRepository.CreateOperationAsync(operationEntity);

                if (operations == null || !operations.Any())
                {
                    return new ApiResponse<List<GetOperationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Operation were created.",
                        Data = new List<GetOperationResponseDTO>()
                    };
                }

                List<GetOperationResponseDTO> operationDTOs = _mapper.Map<List<GetOperationResponseDTO>>(operations);

                return new ApiResponse<List<GetOperationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = " Operation created successfully",
                    Data = operationDTOs
                };
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Error occurred while creating role.");
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
