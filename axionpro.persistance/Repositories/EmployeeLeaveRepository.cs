using AutoMapper;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class EmployeeLeaveRepository :IEmployeeLeaveRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeLeaveRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeLeaveRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeLeaveRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }
        public async Task<List<GetEmployeeLeavePolicyMappingReponseDTO>> CreateEmployeeLeaveMapAsync(CreateEmployeeLeavePolicyMappingRequestDTO requestDTO)
        {
            try
            {
                if (requestDTO.EmployeeId <= 0)
                    throw new ArgumentNullException(nameof(requestDTO.EmployeeId), "EmployeeId cannot be null or zero.");

                // ✅ Use AssignEmployeeId for EmployeeId
                var employeeLeaveMap = new EmployeeLeavePolicyMapping
                {
                    TenantId = requestDTO.TenantId,
                    EmployeeId = requestDTO.AssignEmployeeId, // assignEmployeeId
                    PolicyLeaveTypeMappingId = requestDTO.PolicyLeaveTypeMappingId,
                    EffectiveFrom = requestDTO.EffectiveFrom,
                    EffectiveTo = requestDTO.EffectiveTo,
                    IsActive = requestDTO.IsActive,
                    Remark = requestDTO.Remark,
                    AddedById = requestDTO.EmployeeId, // creator/admin
                    AddedDateTime = DateTime.UtcNow,
                    IsLeaveBalanceAssigned = false // Default value
                };

                await _context.EmployeeLeavePolicyMappings.AddAsync(employeeLeaveMap);
                await _context.SaveChangesAsync();

                var tenantData = await _context.EmployeeLeavePolicyMappings
                    .Where(e => e.TenantId == requestDTO.TenantId && e.IsActive)
                    .Select(e => new GetEmployeeLeavePolicyMappingReponseDTO
                    {
                        Id = e.Id,
                        EmployeeId = e.EmployeeId,
                        PolicyLeaveTypeMappingId = e.PolicyLeaveTypeMappingId,
                        EffectiveFrom = e.EffectiveFrom,
                        EffectiveTo = e.EffectiveTo,
                        IsActive = e.IsActive,
                        IsLeaveBalanceAssigned = e.IsLeaveBalanceAssigned,

                        AddedById = e.AddedById,
                        AddedDateTime = e.AddedDateTime
                    })
                    .ToListAsync();

                return tenantData;
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Invalid employee ID provided. Details: " + ex.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Database error occurred: " + dbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding employee leave mapping: " + ex.Message);
            }
        }


        public async Task<bool> UpdateEmployeeLeaveMap(UpdateEmployeeLeavePolicyMappingRequestDTO updateEmployeeLeavePolicy)
        {
            try
            {
                if (updateEmployeeLeavePolicy == null || updateEmployeeLeavePolicy.PolicyLeaveTypeMappingId <= 0)
                    throw new ArgumentNullException(nameof(updateEmployeeLeavePolicy), "Invalid request: Id cannot be null or zero.");

                // 🔹 Fetch existing record
                var existingRecord = await _context.EmployeeLeavePolicyMappings
                    .FirstOrDefaultAsync(e => e.PolicyLeaveTypeMappingId == updateEmployeeLeavePolicy.PolicyLeaveTypeMappingId
                                           && e.TenantId == updateEmployeeLeavePolicy.TenantId && e.EmployeeId == updateEmployeeLeavePolicy.AssignEmployeeId);

                if (existingRecord == null)
                    throw new KeyNotFoundException($"No EmployeeLeavePolicyMapping found for Id: {updateEmployeeLeavePolicy.PolicyLeaveTypeMappingId}");

                // 🔹 Update only IsActive field
                existingRecord.IsActive = updateEmployeeLeavePolicy.IsActive;
                existingRecord.UpdatedById = updateEmployeeLeavePolicy.EmployeeId;
                existingRecord.UpdatedDateTime = DateTime.UtcNow;

                // 🔹 Mark only specific property as modified
                _context.Entry(existingRecord).Property(x => x.IsActive).IsModified = true;
                _context.Entry(existingRecord).Property(x => x.UpdatedById).IsModified = true;
                _context.Entry(existingRecord).Property(x => x.UpdatedDateTime).IsModified = true;

                // 🔹 Save Changes
                await _context.SaveChangesAsync();

                return true;
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception($"❌ Record not found: {ex.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"❌ Database error while updating IsActive: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"❌ Unexpected error occurred while updating EmployeeLeavePolicyMapping: {ex.Message}");
            }
        }


        public Task<long> DeleteEmployeeLeaveMap(CreateEmployeeLeavePolicyMappingRequestDTO employeeLeavePolicyMappingRequestDTO)
        {
            throw new NotImplementedException();
        }
        public async Task<List<GetEmployeeLeavePolicyMappingReponseDTO>> GetAllEmployeeLeaveMap(GetEmployeeLeavePolicyMappingRequestDTO requestDTO)
        {
            try
            {
                var query = from empMap in _context.EmployeeLeavePolicyMappings
                            join pltm in _context.PolicyLeaveTypeMappings
                                on empMap.PolicyLeaveTypeMappingId equals pltm.Id
                            join lt in _context.LeaveTypes
                                on pltm.LeaveTypeId equals lt.Id
                            where empMap.TenantId == requestDTO.TenantId
                                  && empMap.EmployeeId == requestDTO.AssignEmployeeId
                            select new GetEmployeeLeavePolicyMappingReponseDTO
                            {
                                Id = empMap.Id,
                                TenantId = empMap.TenantId,
                                EmployeeId = empMap.EmployeeId,
                                PolicyLeaveTypeMappingId = empMap.PolicyLeaveTypeMappingId,
                                IsLeaveBalanceAssigned = empMap.IsLeaveBalanceAssigned,
                                EffectiveFrom = empMap.EffectiveFrom,
                                EffectiveTo = empMap.EffectiveTo,
                                IsActive = empMap.IsActive,
                                AddedById = empMap.AddedById,
                                AddedDateTime = empMap.AddedDateTime,

                                // 👇 Ye new field aayegi LeaveType table se
                                LeaveTypeId = pltm.LeaveTypeId,
                                LeaveName = lt.LeaveName
                            };

                // ✅ Optional filter (agar EmployeeId diya gaya hai)
                if (requestDTO.EmployeeId > 0)
                {
                    query = query.Where(e => e.EmployeeId == requestDTO.AssignEmployeeId);
                }

                var result = await query.ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"❌ Error while fetching Employee Leave Policy Mappings: {ex.Message}");
            }
        }
        public async Task<GetEmployeeLeavePolicyMappingReponseDTO?> GetEmployeeLeaveMapByLeaveTypeIdAsync(
   long tenantId,
   long employeeId,
   long policyLeaveTypeId)
        {
            try
            {
                var result = await (
                    from empMap in _context.EmployeeLeavePolicyMappings
                    join pltm in _context.PolicyLeaveTypeMappings
                        on empMap.PolicyLeaveTypeMappingId equals pltm.Id
                    join lt in _context.LeaveTypes
                        on pltm.LeaveTypeId equals lt.Id
                    where empMap.TenantId == tenantId
                          && empMap.EmployeeId == employeeId
                          && empMap.PolicyLeaveTypeMappingId == policyLeaveTypeId
                    select new GetEmployeeLeavePolicyMappingReponseDTO
                    {
                        Id = empMap.Id,
                        TenantId = empMap.TenantId,
                        EmployeeId = empMap.EmployeeId,
                        PolicyLeaveTypeMappingId = empMap.PolicyLeaveTypeMappingId,
                        IsLeaveBalanceAssigned = empMap.IsLeaveBalanceAssigned,
                        EffectiveFrom = empMap.EffectiveFrom,
                        EffectiveTo = empMap.EffectiveTo,
                        IsActive = empMap.IsActive,
                        AddedById = empMap.AddedById,
                        AddedDateTime = empMap.AddedDateTime,
                        LeaveTypeId = pltm.LeaveTypeId,
                        LeaveName = lt.LeaveName
                    }
                ).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"❌ Error while fetching single Employee Leave Policy Mapping: {ex.Message}");
            }
        }


        public async Task<GetEmployeeLeavePolicyMappingReponseDTO> AddLeaveBalanceToEmployee(AddLeaveBalanceToEmployeeRequestDTO dto)
        {
            try
            {
                // 🔹 Validation
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Request data cannot be null.");

                if (dto.EmployeeLeavePolicyMappingId <= 0)
                    throw new ArgumentException("Invalid EmployeeLeavePolicyMappingId provided.");

                // 🔹 Entity create karo
                var entity = new EmployeeLeaveBalance
                {
                    TenantId = dto.TenantId,
                    EmployeeLeavePolicyMappingId = dto.EmployeeLeavePolicyMappingId,
                    LeaveYear = dto.LeaveYear,
                    OpeningBalance = dto.OpeningBalance,
                    Availed = dto.Availed,
                    CurrentBalance = dto.CurrentBalance,
                    CarryForwarded = dto.CarryForwarded,
                    Encashed = dto.Encashed,
                    LeavesOnHold = dto.LeavesOnHold,
                    IsAllBalanceOnHold = dto.IsAllBalanceOnHold,
                    IsActive = true,
                    AddedById = dto.EmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                // 🔹 Database me insert karo
                await _context.EmployeeLeaveBalances.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 🔹 Return single mapping DTO after insert
                var response = await GetEmployeeLeaveMapByLeaveTypeIdAsync(
                    entity.TenantId,
                    dto.AssignLeaveEmployeeId,
                    dto.PolicyLeaveTypeMappingId
                );

                if (response == null)
                    throw new Exception("Employee Leave Policy Mapping not found after adding leave balance.");

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"❌ Error while adding employee leave balance: {ex.Message}");
            }
        }

        public async Task<bool> UpdateIsLeaveBalanceAssigned(long policyLeaveTypeMappingId)
        {
            try
            {
                // 🔹 Record ढूंढो
                var empLeaveMap = await _context.EmployeeLeavePolicyMappings
                    .FirstOrDefaultAsync(p => p.Id == policyLeaveTypeMappingId);

                // 🔹 अगर नहीं मिला तो false return
                if (empLeaveMap == null)
                    return false;

                // 🔹 सिर्फ IsLeaveBalanceAssigned field update करो
                empLeaveMap.IsLeaveBalanceAssigned = true;
                empLeaveMap.UpdatedDateTime = DateTime.UtcNow;

                // 🔹 Save Changes
                _context.EmployeeLeavePolicyMappings.Update(empLeaveMap);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // 🔹 Log or handle error
                throw new Exception($"❌ Error while updating IsLeaveBalanceAssigned for PolicyLeaveTypeMappingId {policyLeaveTypeMappingId}: {ex.Message}");
            }
        }

        public async Task<GetLeaveBalanceToEmployeeResponseDTO> UpdateLeaveBalanceToEmployee(UpdateLeaveBalanceToEmployeeRequestDTO dto)
        {
            try
            {
                // 🔹 Validation
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "Request data cannot be null.");

                if (dto.Id <= 0)
                    throw new ArgumentException("Invalid Leave Balance Id provided.");

                // 🔹 Fetch existing record
                var existingBalance = await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(e => e.Id == dto.Id
                                           && e.TenantId == dto.TenantId);

                if (existingBalance == null)
                    throw new Exception("Employee Leave Balance record not found.");

                // 🔹 Update fields
                existingBalance.LeaveYear = dto.LeaveYear;
                existingBalance.OpeningBalance = dto.OpeningBalance;
                existingBalance.Availed = dto.Availed;
                existingBalance.CurrentBalance = dto.CurrentBalance;
                existingBalance.CarryForwarded = dto.CarryForwarded;
                existingBalance.Encashed = dto.Encashed;
                existingBalance.LeavesOnHold = dto.LeavesOnHold;
                existingBalance.IsAllBalanceOnHold = dto.IsAllBalanceOnHold;
                existingBalance.UpdatedDateTime = DateTime.UtcNow;
                existingBalance.UpdatedById = dto.EmployeeId;


                // 🔹 Save changes
                await _context.SaveChangesAsync();

                // 🔹 Map to Response DTO
                var response = new GetLeaveBalanceToEmployeeResponseDTO
                {
                    Id = existingBalance.Id,
                    TenantId = existingBalance.TenantId,
                    EmployeeLeavePolicyMappingId = existingBalance.EmployeeLeavePolicyMappingId,
                    LeaveYear = existingBalance.LeaveYear,
                    OpeningBalance = existingBalance.OpeningBalance,
                    Availed = existingBalance.Availed,
                    CurrentBalance = existingBalance.CurrentBalance,
                    CarryForwarded = existingBalance.CarryForwarded,
                    Encashed = existingBalance.Encashed,
                    LeavesOnHold = existingBalance.LeavesOnHold,
                    IsAllBalanceOnHold = existingBalance.IsAllBalanceOnHold,
                    IsActive = existingBalance.IsActive,
                    AddedById = existingBalance.AddedById,
                    AddedDateTime = existingBalance.AddedDateTime,
                    UpdatedById = existingBalance.UpdatedById,
                    UpdatedDateTime = existingBalance.UpdatedDateTime
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"❌ Error while updating employee leave balance: {ex.Message}");
            }
        }




    }
}
