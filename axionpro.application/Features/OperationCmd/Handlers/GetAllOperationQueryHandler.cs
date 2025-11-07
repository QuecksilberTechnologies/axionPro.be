using AutoMapper;
using axionpro.application.DTOs.Operation;
using axionpro.application.Features.OperationCmd.Commands;
using axionpro.application.Features.TransportCmd.Handlers;
using axionpro.application.Features.TransportCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.OperationCmd.Handlers
{
    public class GetAllOperationQueryHandler : IRequestHandler<GetAllOperationCommand, ApiResponse<List<GetOperationResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllOperationQueryHandler> _logger;

        public GetAllOperationQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllOperationQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetOperationResponseDTO>>> Handle(GetAllOperationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Correcting the method call
                
                List<Operation> operationDTOs = await _unitOfWork.OperationRepository.GetAllOperationAsync();

                //if (roles == null || !roles.Any())
                //{
                //    _logger.LogWarning("No Operations found.");
                //    return new ApiResponse<List<GetAllRoleDTO>>(false, "No Operations found", new List<GetAllRoleDTO>());
                //}

                //// ✅ Map Role entities to DTOs
                var getAllOperationDTOs = _mapper.Map<List<GetOperationResponseDTO>>(operationDTOs);
                 
                _logger.LogInformation("Successfully retrieved {Count} Operations.", getAllOperationDTOs.Count);
                return new ApiResponse<List<GetOperationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Operations fetched successfully.",
                    Data = getAllOperationDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Operations.");
                return new ApiResponse<List<GetOperationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Operations fetched successfully.",
                    Data = null
                };
            }
        }



    }
}
