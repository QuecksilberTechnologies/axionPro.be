using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeManagerMappings;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeManagerMapping.Command
{
    public class AddEmployeeManagerMappingCommand
        : IRequest<ApiResponse<bool>>
    {
        public AddEmployeeManagerMappingDTO DTO { get; set; }

        public AddEmployeeManagerMappingCommand(AddEmployeeManagerMappingDTO dto)
        {
            DTO = dto;
        }
    }

    public class AddEmployeeManagerMappingCommandHandler
       : IRequestHandler<AddEmployeeManagerMappingCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddEmployeeManagerMappingCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public AddEmployeeManagerMappingCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<AddEmployeeManagerMappingCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            AddEmployeeManagerMappingCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                var dto = request.DTO;

                dto.Prop ??= new ExtraPropRequestDTO();
                dto.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ BUSINESS RULES
                // ===============================

                // ❌ Self mapping not allowed
                if (dto.EmployeeId == dto.ManagerId)
                    throw new ValidationErrorException("Employee cannot be their own manager.");

                // ❌ Primary manager duplicate check
                if (dto.ReportingTypeId == 1)
                {
                    var exists = await _unitOfWork.EmployeeManagerMappingRepository
                        .ExistsPrimaryAsync(dto.EmployeeId, dto.Prop.TenantId);

                    if (exists)
                        throw new ValidationErrorException("Primary manager already exists.");
                }

                // ❌ Date validation
                if (dto.EffectiveTo != null && dto.EffectiveFrom > dto.EffectiveTo)
                    throw new ValidationErrorException("EffectiveFrom cannot be greater than EffectiveTo.");

                // ===============================
                // 3️⃣ TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var entity = new domain.Entity.EmployeeManagerMapping
                {
                    EmployeeId = dto.EmployeeId,
                    ManagerId = dto.ManagerId,
                    ReportingTypeId = dto.ReportingTypeId,
                    DepartmentId = dto.DepartmentId,
                    DesignationId = dto.DesignationId,
                    EffectiveFrom = dto.EffectiveFrom,
                    EffectiveTo = dto.EffectiveTo,
                    TenantId = dto.Prop.TenantId,

                    Description = dto.Description,
                    Remark = dto.Remark,

                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,

                    IsActive = true,
                    IsSoftDeleted = false
                };

                await _unitOfWork.EmployeeManagerMappingRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "EmployeeManagerMapping added | EmployeeId: {EmployeeId}, ManagerId: {ManagerId}",
                    dto.EmployeeId,
                    dto.ManagerId);

                // ===============================
                // 4️⃣ RESPONSE
                // ===============================
                return ApiResponse<bool>.Success(true, "Mapping added successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error in AddEmployeeManagerMappingCommandHandler");
                throw;
            }
        }
    }

    }
