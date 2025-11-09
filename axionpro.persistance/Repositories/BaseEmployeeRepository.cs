using AutoMapper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace axionpro.persistance.Repositories
{
  
    public class BaseEmployeeRepository : IBaseEmployeeRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BaseEmployeeRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public BaseEmployeeRepository(WorkforceDbContext context, IMapper mapper, ILogger<BaseEmployeeRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public Task<long> AutoCreated(Employee entity)
        {
            throw new NotImplementedException();
        }



        public async Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> CreateAsync(Employee entity)
        {
            try
            {
                // 🔹 Basic Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Employee entity cannot be null.");

                if (string.IsNullOrWhiteSpace(entity.FirstName))
                    throw new ArgumentException("First name is required.");

                // 🔹 Insert Record
                await _context.Employees.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 🔹 Pagination Setup
                const int pageNumber = 1;
                const int pageSize = 10;

                // 🔹 Fetch recent list for same tenant
                var query = _context.Employees
                    .AsNoTracking()
                    .Where(x => x.TenantId == entity.TenantId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id);

                int totalCount = await query.CountAsync();

                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 🔹 Map to DTO
                var mappedData = _mapper.Map<List<GetBaseEmployeeResponseDTO>>(pagedData);

                // 🔹 Prepare paged response
                var result = new PagedResponseDTO<GetBaseEmployeeResponseDTO>
                {
                    Items = mappedData,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while creating employee for {TenantId}", entity.TenantId);
                throw new Exception($"Failed to add employee: {ex.Message}");
            }
        }



        public async Task<bool> Delete(string id, string tenantKey)
        {
            try
            {
                // 1️⃣ Decrypt the encrypted Id using the tenant key
                var decryptedId = _encryptionService.Decrypt(id, tenantKey);

                // 2️⃣ Convert to long (assuming primary key is long)
                if (!long.TryParse(decryptedId, out long employeeId))
                    throw new Exception("Invalid employee ID after decryption.");

                // 3️⃣ Fetch the employee record
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Id == employeeId && e.IsActive);

                if (employee == null)
                    throw new Exception("Employee not found or already inactive.");

                // 4️⃣ Soft delete (recommended)
                employee.IsActive = false;
                employee.UpdatedDateTime = DateTime.UtcNow;

                // 5️⃣ Save changes
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();

                // 6️⃣ Log and return success
                _logger.LogInformation($"Employee (ID: {employeeId}) successfully deleted.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee record.");
                return false;
            }
        }

        public Task<bool> DeleteAsync(string id, string tenantKey)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> GetImage(GetEmployeeImageRequestDTO dto, long decryptedTenantId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // 🧩 Step 1: Pagination Defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                
                // 🧩 Step 3: Base Query (no TenantId filtering since not in table)
                var query = context.EmployeeImages
                    .AsNoTracking()
                    .Where(x => x.IsSoftDeleted != true && x.TenantId== decryptedTenantId && x.IsActive== dto.IsActive);

                // 🧩 Step 4: Filter by EmployeeId or ImageId
                if (long.TryParse(dto.EmployeeId, out long employeeId) && employeeId > 0)
                    query = query.Where(x => x.EmployeeId == employeeId);

                if (long.TryParse(dto.Id, out long imageId) && imageId > 0)
                    query = query.Where(x => x.Id == imageId);

                // 🧩 Step 5: Sorting (default by Id DESC)
                query = dto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(x => x.Id)
                    : query.OrderByDescending(x => x.Id);

                // 🧩 Step 6: Get total count before paging
                int totalCount = await query.CountAsync();

                // 🧩 Step 7: Fetch paginated data
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetEmployeeImageReponseDTO
                    {
                        Id = x.Id.ToString(),
                        EmployeeImagePath = x.EmployeeImagePath,
                        IsActive = x.IsActive,
                        IsPrimary = x.IsPrimary,
                      
                    })
                    .ToListAsync();

                // 🧩 Step 8: Return structured response
                return new PagedResponseDTO<GetEmployeeImageReponseDTO>
                {
                    Items = data,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching employee images.");
                throw new Exception($"Failed to fetch employee images: {ex.Message}");
            }
       
        
        }
 

        public async  Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(GetBaseEmployeeRequestDTO dto, long decryptedTenantId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // 🧩 Step 1: Pagination defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // 🧩 Step 2: Base query (Filter by Tenant)
                var query = context.Employees
                    .AsNoTracking()
                    .Where(x => x.TenantId == decryptedTenantId && x.IsSoftDeleted != true);

                // 🧩 Step 3: Dynamic filters
                if (dto.EmployeeDocumentId.HasValue)
                    query = query.Where(x => x.EmployeeDocumentId == dto.EmployeeDocumentId);

                if (!string.IsNullOrWhiteSpace(dto.EmployementCode))
                    query = query.Where(x => x.EmployementCode.Contains(dto.EmployementCode));

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    query = query.Where(x => x.FirstName.Contains(dto.FirstName));

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                    query = query.Where(x => x.LastName.Contains(dto.LastName));

                if (dto.DateOfOnBoarding.HasValue)
                    query = query.Where(x => x.DateOfOnBoarding == dto.DateOfOnBoarding);
               

                if (dto.DesignationId.HasValue)
                    query = query.Where(x => x.DesignationId == dto.DesignationId);

                if (dto.DepartmentId.HasValue)
                    query = query.Where(x => x.DepartmentId == dto.DepartmentId);

                if (!string.IsNullOrWhiteSpace(dto.OfficialEmail))
                    query = query.Where(x => x.OfficialEmail.Contains(dto.OfficialEmail));

                if (dto.HasPermanent.HasValue)
                    query = query.Where(x => x.HasPermanent == dto.HasPermanent);

                if (dto.TypeId.HasValue)
                    query = query.Where(x => x.EmployeeTypeId == dto.TypeId);

                if (dto.IsEditAllowed)
                    query = query.Where(x => x.IsEditAllowed == dto.IsEditAllowed);

                if (dto.IsInfoVerified)
                    query = query.Where(x => x.IsInfoVerified == dto.IsInfoVerified);

                // 🧩 Step 4: Sorting (default by EmployeeDocumentId DESC)
                query = dto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(x => x.EmployeeDocumentId)
                    : query.OrderByDescending(x => x.EmployeeDocumentId);

                // 🧩 Step 5: Total count before paging
                int totalCount = await query.CountAsync();

                // 🧩 Step 6: Fetch paginated data
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetBaseEmployeeResponseDTO
                    {

                        Id = (x.Id).ToString(),
                        EmployementCode = x.EmployementCode,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        FirstName = x.FirstName,
                        GenderId = x.GenderId.ToString(),
                        DateOfBirth = x.DateOfBirth,
                        DateOfOnBoarding = x.DateOfOnBoarding,
                        DateOfExit = x.DateOfExit,
                        DesignationId = x.DesignationId.ToString(),
                        EmployeeTypeId = x.EmployeeTypeId.ToString(),
                        DepartmentId = x.DepartmentId.ToString(),
                        OfficialEmail = x.OfficialEmail,
                        HasPermanent = x.HasPermanent,
                        IsActive = x.IsActive,                       
                        IsEditAllowed = x.IsEditAllowed,
                        IsInfoVerified = x.IsInfoVerified
                    })
                    .ToListAsync();

                // 🧩 Step 7: Return paginated response
                return new PagedResponseDTO<GetBaseEmployeeResponseDTO>
                {
                    Items = data,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching base employee info.");
                throw new Exception($"Failed to fetch base employee info: {ex.Message}");
            }
        }


        public async Task<GetMinimalEmployeeResponseDTO> GetSingleRecordAsync(long id, bool isActive)
        {
            try
            {
                // 🧩 Validation
                if (id <= 0)
                    throw new ArgumentException("Invalid Id provided for fetching employee record.");
                // 🔍 Fetch employee + type + gender + department + designation info
                var record = await (
                    from emp in _context.Employees.AsNoTracking()
                    join et in _context.EmployeeTypes.AsNoTracking()
                        on emp.EmployeeTypeId equals et.Id
                    join gt in _context.Genders.AsNoTracking()
                        on emp.GenderId equals gt.Id into genderGroup
                    from gt in genderGroup.DefaultIfEmpty()

                        // 👇 Department join
                    join dept in _context.Departments.AsNoTracking()
                        on emp.DepartmentId equals dept.Id into deptGroup
                    from dept in deptGroup.DefaultIfEmpty()

                        // 👇 Designation join
                    join desig in _context.Designations.AsNoTracking()
                        on emp.DesignationId equals desig.Id into desigGroup
                    from desig in desigGroup.DefaultIfEmpty()

                    where emp.Id == id
                       && emp.IsActive == isActive
                       && emp.IsSoftDeleted != true
                       && et.IsActive == true
                       && et.IsSoftDeleted != true
                       && (dept == null || (dept.IsActive == true && dept.IsSoftDeleted != true))
                       && (desig == null || (desig.IsActive == true && desig.IsSoftDeleted != true))

                    select new GetMinimalEmployeeResponseDTO
                    {
                        Id = emp.Id,
                        TenantId = emp.TenantId,
                        FirstName = emp.FirstName,
                        MiddleName = emp.MiddleName,
                        LastName = emp.LastName,
                        EmployementCode = emp.EmployementCode,
                        EmployeeTypeId = emp.EmployeeTypeId,
                        EmployeeTypeName = et.TypeName,
                        IsActive = emp.IsActive,
                        HasPermanent = emp.HasPermanent,
                        GenderId = emp.GenderId,
                        GenderName = gt.GenderName,
                        DepartmentId = emp.DepartmentId,
                        DepartmentName = dept.DepartmentName,   // ✅ added
                        DesignationId = emp.DesignationId,
                        DesignationName = desig.DesignationName // ✅ added,
                        ,OfficialEmail = emp.OfficialEmail

                    }
                ).FirstOrDefaultAsync();


                // 🚫 Handle null result
                if (record == null)
                {
                    _logger.LogWarning("⚠️ No employee record found for Id: {Id} (IsActive: {IsActive})", id, isActive);
                    throw new InvalidOperationException($"No employee record found for Id: {id} (IsActive: {isActive})");
                }

                // ✅ Return DTO result
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching employee record for Id: {Id}", id);
                throw;
            }
        }


        public Task<bool> UpdateEmployeeFieldAsync(long Id, string entity, string fieldName, object? fieldValue, long updatedById)
        {
            throw new NotImplementedException();
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

     
       




        #region Employee-Base-info






        #endregion



    }
}




 
 






