using AutoMapper;
using axionpro.application.Features.AssetFeatures.Status.Commands;
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

namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStatusCommandHandler> _logger;

        public UpdateStatusCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<UpdateStatusCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // DTO ko Entity mein map karein

               // request.DTO.IsActive = request.DTO.IsActive ?? false;
               
                var isUpdated = await _unitOfWork.AssetStatusRepository.UpdateAsync(request.DTO);
               
                 if(!isUpdated)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Asset Status not found or update failed.",
                        Data = false
                    };
                }
                    return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset Status updated successfully.",
                    Data = true
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Updateing asset status.");
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while Updateing asset status.",
                    Data = false
                };
            }
        }
    }

}
