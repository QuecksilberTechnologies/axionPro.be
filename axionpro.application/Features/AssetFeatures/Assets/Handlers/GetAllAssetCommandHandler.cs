using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class GetAllAssetCommand
    : IRequest<ApiResponse<List<GetAssetResponseDTO>>>
    {
        public GetAssetRequestDTO DTO { get; set; }

        public GetAllAssetCommand(GetAssetRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetAllAssetCommandHandler
        : IRequestHandler<GetAllAssetCommand, ApiResponse<List<GetAssetResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllAssetCommandHandler> _logger;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _config;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllAssetCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllAssetCommandHandler> logger,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService,
            IConfiguration config,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
            _config = config;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetAssetResponseDTO>>> Handle(
            GetAllAssetCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Assets");

                // ✅ VALIDATION
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ✅ FETCH DATA
                var pagedAssets = await _unitOfWork.AssetRepository
                    .GetAssetsByFilterAsync(request.DTO);
 

                // ✅ PROJECTION
                var encryptedList = ProjectionHelper.ToGetAssetResponseDTOs(
                    pagedAssets.Data,
                    _idEncoderService,
                    validation.Claims.TenantEncriptionKey,
                    _config,
                    _fileStorageService
                );

                _logger.LogInformation(
                    "Successfully retrieved {Count} assets for TenantId: {TenantId}",
                    encryptedList.Count,
                    request.DTO.Prop.TenantId);

                // ✅ 🔥 YOUR CUSTOM RESPONSE
                return ApiResponse<List<GetAssetResponseDTO>>.SuccessPaginatedOnly(
                    Data: encryptedList,
                    PageNumber: pagedAssets.PageNumber,
                    PageSize: pagedAssets.PageSize,
                    TotalRecords: pagedAssets.TotalCount,
                    TotalPages: pagedAssets.TotalPages,
                    Message: "Assets fetched successfully.",
                    HasUploadedAll: pagedAssets.HasUploadedAll
                     
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching assets for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }

}

 