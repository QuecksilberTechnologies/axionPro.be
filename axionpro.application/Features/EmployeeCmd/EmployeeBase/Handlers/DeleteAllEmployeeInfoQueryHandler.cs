using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class DeleteEmployeeQuery : IRequest<ApiResponse<bool>>
    {
      public DeleteBaseEmployeeRequestDTO DTO;

        public DeleteEmployeeQuery(DeleteBaseEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class DeleteBaseEmployeeInfoQueryHandler : IRequestHandler<DeleteEmployeeQuery, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBaseEmployeeInfoQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IPermissionService _permissionService;
        public DeleteBaseEmployeeInfoQueryHandler(
            IBaseEmployeeRepository employeeRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            ILogger<DeleteBaseEmployeeInfoQueryHandler> logger, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, IPermissionService permissionService)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _permissionService = permissionService;
        }
        public async Task<ApiResponse<bool>> Handle(DeleteEmployeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                       validation.Claims.TenantEncriptionKey,
                          _idEncoderService
                         );

                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                var employee = await _unitOfWork.Employees.GetByIdAsync(request.DTO.Prop.EmployeeId,request.DTO.Prop.TenantId, true);
                  if(employee == null)
                  {
                      return ApiResponse<bool>.Fail("Employee not found for current user.");
                   }
                
                bool Issuccess =    await _unitOfWork.Employees.DeleteAllAsync(employee);
                if (!Issuccess)
                    return ApiResponse<bool>.Fail("Employee not found for current user.");

                return new ApiResponse<bool>
                {

                    IsSucceeded = true,
                    Message = $"successfully deleted" ,
                    Data = Issuccess


                };            

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching self employee info.");
                return ApiResponse<bool>.Fail("Something went wrong.", new List<string> { ex.Message });
            }
        }


    }



}


