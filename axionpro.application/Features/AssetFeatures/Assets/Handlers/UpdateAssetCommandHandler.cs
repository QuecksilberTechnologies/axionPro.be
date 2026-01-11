using AutoMapper;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class UpdateAssetCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateAssetRequestDTO DTO { get; set; }

        public UpdateAssetCommand(UpdateAssetRequestDTO dTO)
        {
            DTO = dTO;
        }

    }

    //public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, ApiResponse<bool>>
    //{
       
    //    private readonly IMapper _mapper;
    //    private readonly IUnitOfWork _unitOfWork;
    //     private readonly ILogger<UpdateAssetCommand> _logger; // यदि logger का उपयोग करना हो

    //    public UpdateAssetCommandHandler(IAssetRepository assetRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateAssetCommand> logger)
    //    {
           
    //        _mapper = mapper;
    //        _unitOfWork = unitOfWork;
    //        _logger = logger;
    //    }

    //    //public async Task<ApiResponse<bool>> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    //    //{
    //    //    bool updated = false;
            
    //    //        await _unitOfWork.BeginTransactionAsync();

    //    //        try
    //    //        {
    //    //            // 1️⃣ Common validation
    //    //            var validation = await _commonRequestService
    //    //                .ValidateRequestAsync(request.DTO.UserEmployeeId);

    //    //            if (!validation.Success)
    //    //                return ApiResponse<GetBaseEmployeeResponseDTO>
    //    //                    .Fail(validation.ErrorMessage);

    //    //            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
    //    //            request.DTO.Prop.TenantId = validation.TenantId;

    //    //            // 2️⃣ Permission check (optional)
    //    //            var permissions = await _permissionService
    //    //                .GetPermissionsAsync(validation.RoleId);

    //    //            // Asset को update करना
    //    //            updated = await _unitOfWork.AssetRepository.UpdateAssetInfoAsync(request.DTO);
              

    //    //        if (!updated )
    //    //        {
    //    //            return new ApiResponse<bool>
    //    //            {
    //    //                IsSucceeded = false,
    //    //                Message = "No asset was updated.",
    //    //                Data = updated
    //    //            };
    //    //        }
 

    //    //        return new ApiResponse<bool>
    //    //        {
    //    //            IsSucceeded = true,
    //    //            Message = "Asset updated successfully.",
    //    //            Data = updated
    //    //        };
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        // _logger.LogError(ex, "Error occurred while updating asset.");
    //    //        return new ApiResponse<bool>
    //    //        {
    //    //            IsSucceeded = false,
    //    //            Message = $"An error occurred: {ex.Message}",
    //    //            Data = updated
    //    //        };
    //    //    }
    //    //}
    
    
    //}


}