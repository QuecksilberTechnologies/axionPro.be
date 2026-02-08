using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Pagination;

using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetPolicyInsuranceRequestCommand : IRequest<ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>>
    {
        public GetPolicyTypeInsuranceMappingRequestDTO DTO { get; }    

        public GetPolicyInsuranceRequestCommand(GetPolicyTypeInsuranceMappingRequestDTO dto)
        {
            DTO = dto;
           
        }
    }
    public class GetPolicyTypeInsuranceMappingQueryHandler  : IRequestHandler<GetPolicyInsuranceRequestCommand, ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPolicyTypeInsuranceMappingQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetPolicyTypeInsuranceMappingQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetPolicyTypeInsuranceMappingQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>> Handle(
            GetPolicyInsuranceRequestCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Common validation (Tenant / User)
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                {
                    //return ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>
                    //    .Fail(validation.ErrorMessage);
                }
                // Assign decoded values coming from CommonRequestService
                request.DTO.Props.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Props.TenantId = validation.TenantId;  



                // 2️⃣ Paging defaults
                var pageNumber = request.DTO.PageNumber > 0
                    ? request.DTO.PageNumber
                    : 1;

                var pageSize = request.DTO.PageSize > 0
                    ? request.DTO.PageSize
                    : 10;

                var sortOrder = string.IsNullOrWhiteSpace(request.DTO.SortOrder)
                    ? "desc"
                    : request.DTO.SortOrder.ToLower();

                // 3️⃣ Call Repository (🔥 EXACT MATCH)
                var result =
                    await _unitOfWork.PolicyTypeInsuranceMappingRepository.GetListAsync(request.DTO);

                // 4️⃣ No data check
                if (result == null || !result.Items.Any())
                {
                    _logger.LogInformation(
                        "No insurance policies mapping found. TenantId: {TenantId}",
                        validation.TenantId);

                    return ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>
                        .Fail("No insurance policies mapping found.");
                }
                    
                // 5️⃣ Success response
                return ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>.SuccessPaginatedPercentage(
                  Data: result.Items,
                  Message: "PolicyInsurance mapping info retrieved successfully.",
                  PageNumber: result.PageNumber,
                  PageSize: result.PageSize,
                  TotalRecords: result.TotalCount,
                  TotalPages: result.TotalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching insurance policies. UserEmployeeId: {UserEmployeeId}",
                    request.DTO?.UserEmployeeId);

                return ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>
                    .Fail("An unexpected error occurred while fetching insurance policies.");
            }
        }
    }



}
