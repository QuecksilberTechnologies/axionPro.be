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
using Microsoft.Extensions.Logging;

using axionpro.application.Features.AssetFeatures.Assets.Commands;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, ApiResponse<bool>>
    {
       
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
         private readonly ILogger<UpdateAssetCommand> _logger; // यदि logger का उपयोग करना हो

        public UpdateAssetCommandHandler(IAssetRepository assetRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateAssetCommand> logger)
        {
           
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
        {
            bool updated = false;
            try
            {
          
               
                
                // Asset को update करना
                  updated = await _unitOfWork.AssetRepository.UpdateAssetInfoAsync(request.DTO);
              

                if (!updated )
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "No asset was updated.",
                        Data = updated
                    };
                }
 

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset updated successfully.",
                    Data = updated
                };
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error occurred while updating asset.");
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = updated
                };
            }
        }
    }


}