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
    public class GetAllInsuranceQuery  : IRequest<List<GetAlllnsurancePolicyResponseDTO>>
        {
        public GetAllInsurancePolicyRequestDTO DTO { get; }

    

        public GetAllInsuranceQuery(GetAllInsurancePolicyRequestDTO dto)
        {
            DTO = dto;
           
        }
    }
    public class GetAllInsuranceQueryHandler
        : IRequestHandler<
            GetAllInsuranceQuery,
            List<GetAlllnsurancePolicyResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllInsuranceQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllInsuranceQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllInsuranceQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<List<GetAlllnsurancePolicyResponseDTO>> Handle(
            GetAllInsuranceQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 🔐 Common validation (Tenant / User)
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return new List<GetAlllnsurancePolicyResponseDTO>();

                // 🔹 Repository call (DDL only)
                var result =
                    await _unitOfWork.InsuranceRepository.GetAllListAsync(
                        request.DTO.PolicyId,
                        request.DTO.IsActive
                    );

                // ❗ DDL ke liye empty list OK hoti hai
                return result ?? new List<GetAlllnsurancePolicyResponseDTO>(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching InsurancePolicy DDL");

                return new List<GetAlllnsurancePolicyResponseDTO>();
            }
        }
    }






}
