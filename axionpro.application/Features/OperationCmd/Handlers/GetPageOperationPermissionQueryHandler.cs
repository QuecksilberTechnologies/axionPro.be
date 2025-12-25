using AutoMapper;
using axionpro.application.DTOs.Operation;
using axionpro.application.Features.OperationCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.OperationCmd.Handlers
{
    public class GetPageOperationPermissionQueryHandler : IRequestHandler<GetPageOperationPermissionQuery, ApiResponse<GetHasAccessOperationDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPageOperationPermissionQueryHandler> _logger;

        public GetPageOperationPermissionQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetPageOperationPermissionQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetHasAccessOperationDTO>> Handle(GetPageOperationPermissionQuery request, CancellationToken cancellationToken)
        {

            var response = new ApiResponse<GetHasAccessOperationDTO>();

            try
            {
                var requestDTO = request.CheckOperationPermissionRequest;


                // Repository se permission check karo
                bool result = await _unitOfWork.StoreProcedureRepository.GetHasAccessOperation(requestDTO);
                if (result)
                {
                    response.IsSucceeded = result;

                    response.Data = new GetHasAccessOperationDTO
                    {
                        Status = result, // Assign the bool directly
                        Message = "✅ Permission checked successfully.",
                        Success = result

                    };
                }
                else
                {
                    response.IsSucceeded = result;
                    response.Data = new GetHasAccessOperationDTO
                    {
                        Status = result, // Assign the bool directly
                        Message = "✅ Not have permission.",
                        Success = result

                    };
                }

              
            }
            catch (Exception ex)
            {
                response.IsSucceeded = false;
                response.Message = "❌ An error occurred while checking permission.";
                _logger.LogError($"🚨 Error in GetPageOperationPermissionQueryHandler: {ex.Message}");
            }

            return response;
        }
    }
}
