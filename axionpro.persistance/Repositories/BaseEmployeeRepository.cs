using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

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



        public async Task<bool> DeleteAsync(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔹 Main Employee record
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Id == id && (e.IsSoftDeleted == null || e.IsSoftDeleted == false));

                if (employee == null)
                {
                    _logger.LogWarning($"⚠️ Employee with ID {id} not found or already deleted.");
                    return false;
                }

                // 🔹 Soft delete main Employee
                employee.IsSoftDeleted = true;
                employee.UpdatedDateTime = DateTime.UtcNow;

                _context.Employees.Update(employee);

                // 🔹 Related Employee Details
                var bankDetails = await _context.EmployeeBankDetails
                    .Where(d => d.EmployeeId == id && (d.IsSoftDeleted == null || d.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var detail in bankDetails)
                {
                    detail.IsSoftDeleted = true;
                    detail.UpdatedDateTime = DateTime.UtcNow;
                }

                _context.EmployeeBankDetails.UpdateRange(bankDetails);

                // 🔹 Related Bank Info
                var bankInfos = await _context.EmployeeContacts
                    .Where(b => b.EmployeeId == id && (b.IsSoftDeleted == null || b.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var bank in bankInfos)
                {
                    bank.IsSoftDeleted = true;
                    bank.UpdatedDateTime = DateTime.UtcNow;
                }

                _context.EmployeeContacts.UpdateRange(bankInfos);

                // 🔹 Related Address
                var addresses = await _context.EmployeeContacts
                    .Where(a => a.EmployeeId == id && (a.IsSoftDeleted == null || a.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var addr in addresses)
                {
                    addr.IsSoftDeleted = true;
                    addr.UpdatedDateTime = DateTime.UtcNow;
                }

                _context.EmployeeContacts.UpdateRange(addresses);

                // 🔹 Save changes in single transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"✅ Employee {id} and related records soft deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"❌ Failed to delete employee and related records for ID {id}");
                throw new Exception($"Failed to delete employee {id}: {ex.Message}", ex);
            }
        }
        public async Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> GetImage(GetEmployeeImageRequestDTO dto, long decryptedTenantId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Pagination defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // Base query
                var query = context.EmployeeImages
                    .AsNoTracking()
                    .Where(x => x.IsSoftDeleted != true &&
                                x.TenantId == decryptedTenantId);

                if (dto.IsActive)
                    query = query.Where(x => x.IsActive == dto.IsActive);

                if (dto._EmployeeId > 0)
                    query = query.Where(x => x.EmployeeId == dto._EmployeeId);

                if (dto.Id_long > 0)
                    query = query.Where(x => x.Id == dto.Id_long);

                // Safe sorting
                bool isAscending =
                    !string.IsNullOrWhiteSpace(dto.SortOrder) &&
                    dto.SortOrder.Trim().Equals("asc", StringComparison.OrdinalIgnoreCase);

                query = isAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);

                int totalCount = await query.CountAsync();

                // Just check primary, don't load all rows
                bool hasPrimary = await query.AnyAsync(x => x.IsPrimary == true);

                double completionPercentage = hasPrimary ? 100 : 0;

                // Paging
                var pagedDataRaw = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedData = pagedDataRaw.Select(x => new GetEmployeeImageReponseDTO
                {
                    EmployeeId = x.EmployeeId.ToString(),
                    Id = x.Id.ToString(),
                    FilePath = x.FilePath,
                    IsActive = x.IsActive,
                    IsPrimary = x.IsPrimary,
                    CompletionPercentage = completionPercentage
                }).ToList();

                return new PagedResponseDTO<GetEmployeeImageReponseDTO>
                {
                    Items = pagedData,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    CompletionPercentage = completionPercentage,
                    IsPrimaryMarked = hasPrimary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching employee images.");
                throw;
            }
        }


        public async Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(GetBaseEmployeeRequestDTO dto, long decryptedTenantId, long id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
          
            try
            {
                // 🧩 Step 1: Pagination defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // 🧩 Step 2: Base query (Tenant + Active filter)
                var query = context.Employees
                    .AsNoTracking()
                    .Where(x => x.TenantId == decryptedTenantId && x.IsSoftDeleted != true);
                
                // 🧩 Step 3: Safe parse helpers (convert string → long safely)
                   
                int designationId = SafeParser.TryParseInt(dto.DesignationId); 
                int typeId = SafeParser.TryParseInt(dto.Id);

                // 🧩 Step 4: Dynamic filters (null-safe + condition-based)
                if (id > 0)
                    query = query.Where(x => x.Id == id);

                if (!string.IsNullOrWhiteSpace(dto.EmployementCode))
                    query = query.Where(x => x.EmployementCode.Contains(dto.EmployementCode));

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    query = query.Where(x => x.FirstName.Contains(dto.FirstName));

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                    query = query.Where(x => x.LastName.Contains(dto.LastName));

                if (dto.DateOfOnBoarding.HasValue)
                    query = query.Where(x => x.DateOfOnBoarding == dto.DateOfOnBoarding);

                if (dto.DateOfBirth.HasValue)
                    query = query.Where(x => x.DateOfBirth == dto.DateOfBirth);

               

                if (designationId > 0)
                    query = query.Where(x => x.DesignationId == designationId);

                if (typeId > 0)
                    query = query.Where(x => x.EmployeeTypeId == typeId);

                if (dto.HasPermanent.HasValue)
                    query = query.Where(x => x.HasPermanent == dto.HasPermanent);

                if (dto.IsEditAllowed.HasValue)
                    query = query.Where(x => x.IsEditAllowed == dto.IsEditAllowed);

                if (dto.IsInfoVerified.HasValue)
                    query = query.Where(x => x.IsInfoVerified == dto.IsInfoVerified);

                // 🧩 Step 5: Sorting (custom field ya default)
                bool isAscending = string.Equals(dto.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);
                query = dto.SortBy?.ToLower() switch
                {
                    "firstname" => isAscending ? query.OrderBy(x => x.FirstName) : query.OrderByDescending(x => x.FirstName),
                    "lastname" => isAscending ? query.OrderBy(x => x.LastName) : query.OrderByDescending(x => x.LastName),
                    "employementcode" => isAscending ? query.OrderBy(x => x.EmployementCode) : query.OrderByDescending(x => x.EmployementCode),
                    _ => isAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id)
                };

                // 🧩 Step 6: Pagination & total count
                int totalCount = await query.CountAsync();

                var records = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetBaseEmployeeResponseDTO
                    {
                        Id = x.Id.ToString(),
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

                // 🧩 Step 7: Final paged response
                return new PagedResponseDTO<GetBaseEmployeeResponseDTO>
                {
                    Items = records,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalCount/pageSize)

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching base employee info.");
                throw new Exception($"Failed to fetch base employee info: {ex.Message}");
            }
        }

        public async Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(
              GetAllEmployeeInfoRequestDTO dto,
    long decryptedTenantId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // 🔹 SAFE BASE QUERY
                var query =


                       from emp in context.Employees.AsNoTracking()
                        where emp.TenantId == decryptedTenantId && emp.IsSoftDeleted != true

                      join g in context.Genders on emp.GenderId equals g.Id into genderJoin
                      from gender in genderJoin.DefaultIfEmpty()

                     join d in context.Designations on emp.DesignationId equals d.Id into desigJoin
                     from designation in desigJoin.DefaultIfEmpty()

                       join et in context.EmployeeTypes on emp.EmployeeTypeId equals et.Id into typeJoin
                      from empType in typeJoin.DefaultIfEmpty()

                      join dep in context.Departments on emp.DepartmentId equals dep.Id into deptJoin
                       from department in deptJoin.DefaultIfEmpty()


           // ⭐ FIXED IMAGE SELECTION
       let image = context.EmployeeImages
           .Where(i => i.EmployeeId == emp.Id && i.IsSoftDeleted != true)
           .OrderByDescending(i => i.IsPrimary)
           .ThenByDescending(i => i.HasImageUploaded)
           .ThenByDescending(i => i.Id)
           .FirstOrDefault()

       select new
       {
           emp,
           GenderName = gender != null ? gender.GenderName : null,
           DesignationName = designation != null ? designation.DesignationName : null,
           EmployeeTypeName = empType != null ? empType.TypeName : null,
           DepartmentName = department != null ? department.DepartmentName : null,

           EmployeeImagePath = image != null ? image.FilePath : null,
           HasImagePicUploaded =
               (image != null && image.IsPrimary == true && image.HasImageUploaded == true)
       };
                // 🔹 FILTERS
                if (dto.Id_long > 0)
                    query = query.Where(x => x.emp.Id == dto.Id_long);

                if (!string.IsNullOrWhiteSpace(dto.EmailId))
                    query = query.Where(x => x.emp.OfficialEmail == dto.EmailId);

                if (dto._DepartmentId >0 )
                        query = query.Where(x => x.emp.DepartmentId == dto._DepartmentId);

                if (dto._DesignationId > 0)
                    query = query.Where(x => x.emp.DepartmentId == dto._DepartmentId);

                if (dto._EmployeeTypeId > 0)
                    query = query.Where(x => x.emp.EmployeeTypeId == dto._EmployeeTypeId);
              
                if (dto._GenderId > 0)
                    query = query.Where(x => x.emp.GenderId == dto._GenderId);


                if (!string.IsNullOrWhiteSpace(dto.EmployementCode))
                    query = query.Where(x => x.emp.EmployementCode.Contains(dto.EmployementCode));

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    query = query.Where(x => x.emp.FirstName.Contains(dto.FirstName));

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                    query = query.Where(x => x.emp.LastName.Contains(dto.LastName));

                if (dto.IsMarried.HasValue)
                {
                    bool married = dto.IsMarried.Value;

                    query = query.Where(x =>
                        context.EmployeeDependents.Any(dep =>
                            dep.EmployeeId == x.emp.Id && dep.IsMarried == married));
                }
             
                

                // 🔹 SORTING
                bool isAsc = string.Equals(dto.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

                query = dto.SortBy?.ToLower() switch
                {
                    "firstname" => isAsc ? query.OrderBy(x => x.emp.FirstName ?? "")
                                         : query.OrderByDescending(x => x.emp.FirstName ?? ""),

                    "lastname" => isAsc ? query.OrderBy(x => x.emp.LastName ?? "")
                                        : query.OrderByDescending(x => x.emp.LastName ?? ""),



                    _ => isAsc ? query.OrderBy(x => x.emp.Id)
                               : query.OrderByDescending(x => x.emp.Id)
                };

                int totalCount = await query.CountAsync();

                // 🔹 FINAL DTO SELECT
                var records = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetAllEmployeeInfoResponseDTO
                    {
                        EmployeeId = x.emp.Id.ToString(),
                        EmployementCode = x.emp.EmployementCode,
                        FirstName = x.emp.FirstName,
                        LastName = x.emp.LastName,
                        DateOfOnBoarding = x.emp.DateOfOnBoarding.ToString(),

                        // 🔹 Added IDs
                        GenderId = x.emp.GenderId.ToString(),
                        EmployeeTypeId = x.emp.EmployeeTypeId.ToString(),
                        DesignationId = x.emp.DesignationId.ToString(),
                        DepartmentId = x.emp.DepartmentId.ToString(),

                        // Already existing names
                        GenderName = x.GenderName,
                        EmployeeTypeName = x.EmployeeTypeName,
                        DesignationName = x.DesignationName,
                        DepartmentName = x.DepartmentName,
                        OfficialEmail = x.emp.OfficialEmail,
                        //EmployeeImagePath = x.EmployeeImagePath,
                        //HasImagePicUploaded = x.HasImagePicUploaded,
                        EmployeeImagePath = context.EmployeeImages
                        .Where(i => i.EmployeeId == x.emp.Id
                          && i.IsSoftDeleted != true
                             && i.IsPrimary == true)
                             .Select(i => i.FilePath)
                           .FirstOrDefault(),

                          HasImagePicUploaded = context.EmployeeImages
                         .Any(i => i.EmployeeId == x.emp.Id
                            && i.IsSoftDeleted != true
                            && i.IsPrimary == true),

                        CompletionPercentage = (
                             new int[]
                                {
                               string.IsNullOrEmpty(x.emp.FirstName) ? 0 : 1,
                               string.IsNullOrEmpty(x.emp.LastName) ? 0 : 1,
                               x.emp.DateOfOnBoarding == null ? 0 : 1,
                               x.emp.DesignationId > 0 ? 1 : 0,
                               x.emp.DepartmentId > 0 ? 1 : 0,
                              string.IsNullOrEmpty(x.emp.OfficialEmail) ? 0 : 1,
                            x.emp.IsActive == true ? 1 : 0,
                             x.emp.IsEditAllowed == true ? 1 : 0,
                                  x.emp.IsInfoVerified == true ? 1 : 0,
                                     // ⭐ Correct: if NO PRIMARY = incomplete
                                       context.EmployeeImages.Any(i => i.EmployeeId == x.emp.Id
                                     && i.IsSoftDeleted != true
                                    && i.IsPrimary == true) ? 1 : 0
                               }.Sum()
                                       / 10.0
                          ) * 100

                    })
                    .ToListAsync();

                 
                // 🧩 Final paged response
                return new PagedResponseDTO<GetAllEmployeeInfoResponseDTO>
                {
                    Items = records,  
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),

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

        public async Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> AddImageAsync(EmployeeImage entity)
        {
            try
            {
                int pageNumber = 1;
                int pageSize = 10;

                // 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Image entity can't be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid Image entity provided.");

                // 2️⃣ Insert record
                await _context.EmployeeImages.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 3️⃣ Fetch all images of that employee
                var allImages = await _context.EmployeeImages
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                int totalRecords = allImages.Count;

                // 4️⃣ Pagination
                var pagedImages = allImages
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 5️⃣ Check if employee has any primary image
                bool hasPrimary = allImages.Any(x => x.IsPrimary == true);

                // 6️⃣ Completion Percentage based only on IsPrimary
                double completionPercentage = hasPrimary ? 100 : 0;

                // 7️⃣ Map records
                var responseData = pagedImages.Select(x => new GetEmployeeImageReponseDTO
                {
                    Id = x.Id.ToString(),
                    FilePath = x.FilePath,
                    IsActive = x.IsActive,
                    IsPrimary = x.IsPrimary,
                    CompletionPercentage = completionPercentage
                }).ToList();

                // 8️⃣ Return
                return new PagedResponseDTO<GetEmployeeImageReponseDTO>
                {
                    Items = responseData,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    IsPrimaryMarked = hasPrimary,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                   
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error occurred while adding/fetching image info for EmployeeId: {EmployeeId}",
                    entity?.EmployeeId);

                throw new Exception($"Failed to add or fetch image info: {ex.Message}");
            }
        }



    }


}







       




        #endregion



  




 
 






