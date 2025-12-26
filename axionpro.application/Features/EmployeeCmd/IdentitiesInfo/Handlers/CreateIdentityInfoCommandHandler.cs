using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers
{
    public class CreateIdentityInfoCommand : IRequest<ApiResponse<bool>>
    {
        public CreateEmployeeIdentityRequestDTO DTO { get; set; }

        public CreateIdentityInfoCommand(CreateEmployeeIdentityRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateEmployeeIdentityCommandHandler  : IRequestHandler<CreateIdentityInfoCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;

        public CreateEmployeeIdentityCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<bool>> Handle(
            CreateIdentityInfoCommand request,
            CancellationToken cancellationToken)
        {
            if (request.DTO.Identities == null || !request.DTO.Identities.Any())
                return ApiResponse<bool>.Fail("No identity data received.");

            // ✅ Validate ONCE
            var first = request.DTO.Identities.First();
            var validation = await _commonRequestService
                .ValidateRequestAsync(first.UserEmployeeId);

            if (!validation.Success)
                return ApiResponse<bool>.Fail(validation.ErrorMessage);

            await _unitOfWork.BeginTransactionAsync();
            var entities = new List<EmployeeIdentity>();
            try
            {
                foreach (var identity in request.DTO.Identities)
                {
                    var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                        identity.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService
                    );

                    string? documentPath = null;
                    string? documentName = null;

                    // ✅ Generic file upload (PAN / Passport / Aadhaar ALL SAME)
                    if (identity.IdentityDocumentFile != null &&
                        identity.IdentityDocumentFile.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await identity.IdentityDocumentFile.CopyToAsync(ms, cancellationToken);



                        var safeValue = identity.IdentityValue.Length > 4
                            ? identity.IdentityValue[^4..]
                            : "XXXX";

                        documentName =
                            $"ID-{identity.IdentityCategoryDocumentId}-{employeeId}-{safeValue}.pdf";

                        var folder = _fileStorageService.GetEmployeeFolderPath(
                            validation.TenantId,
                            employeeId,
                            "identity"
                        );

                        var fullPath = await _fileStorageService
                            .SaveFileAsync(ms.ToArray(), documentName, folder);

                        documentPath = _fileStorageService.GetRelativePath(fullPath);

                    }

                    entities.Add(new EmployeeIdentity
                    {
                        EmployeeId = employeeId,
                        IdentityCategoryDocumentId = identity.IdentityCategoryDocumentId,
                        IdentityValue = identity.IdentityValue,

                        DocumentFileName = documentName,
                        DocumentFilePath = documentPath,

                        EffectiveFrom = identity.EffectiveFrom,
                        EffectiveTo = identity.EffectiveTo,

                        HasIdentityUploaded = documentPath != null,
                        IsEditAllowed = true,
                        IsActive = true,

                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow
                    });
                  

              
                }
                bool isSuccess = await _unitOfWork.EmployeeIdentityRepository.CreateAsync(entities);
                if (!isSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Internal Error");

                }


                await _unitOfWork.CommitTransactionAsync();
                return ApiResponse<bool>.Success(true, "Identity details saved successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.Fail(ex.Message);
            }
        }
    }



}
