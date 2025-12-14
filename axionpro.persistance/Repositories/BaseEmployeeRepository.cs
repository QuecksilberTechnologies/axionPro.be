using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Extentions;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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


                // 2️⃣ Insert record'
                // Add Image before save (same transaction)
                var employeeImage = new EmployeeImage
                {
                    Employee = entity,  // 🔥 instead of EmployeeId = entity.Id
                    TenantId = entity.TenantId,
                    IsPrimary = true,
                    HasImageUploaded = false,
                    IsActive = true,
                    AddedById = entity.AddedById,
                    AddedDateTime = DateTime.UtcNow,
                    FileType = 1

                };

                await _context.EmployeeImages.AddAsync(employeeImage);
                await _context.SaveChangesAsync();

                // 🔹 Pagination Setup
                const int pageNumber = 1;
                const int pageSize = 10;

                // 🔹 Base query (WITH JOINS + SELECT DTO)
                var query = (from e in _context.Employees

                             join d in _context.Designations on e.DesignationId equals d.Id into des
                             from d in des.DefaultIfEmpty()

                             join dep in _context.Departments on e.DepartmentId equals dep.Id into dept
                             from dep in dept.DefaultIfEmpty()

                             //join r in _context.Roles on e.UserRoles.FirstOrDefault equals r.Id into roleTbl
                             //from r in roleTbl.DefaultIfEmpty()

                             join g in _context.Genders on e.GenderId equals g.Id into gen
                             from g in gen.DefaultIfEmpty()

                             where e.TenantId == entity.TenantId && e.IsSoftDeleted != true
                             orderby e.Id descending
                             join et in _context.EmployeeTypes  on e.EmployeeTypeId equals et.Id into empType
                             from et in empType.DefaultIfEmpty()


                             select new GetBaseEmployeeResponseDTO
                             {
                                 Id = e.Id.ToString(),
                                 EmployementCode = e.EmployementCode,
                                 FirstName = e.FirstName,
                                 LastName = e.LastName,
                                 MiddleName = e.MiddleName,

                                 GenderId = e.GenderId,
                                 GenderName = g.GenderName,

                                 DesignationId = e.DesignationId,
                                 DesignationName = d.DesignationName,

                                 DepartmentId = e.DepartmentId,
                                 DepartmentName = dep.DepartmentName,

                                
                                 Type = et.TypeName,


                                 //  RoleName = r.RoleName,
                                 OfficialEmail = e.OfficialEmail,
                                 EmployeeTypeId = e.EmployeeTypeId,

                                 DateOfBirth = e.DateOfBirth,
                                 DateOfOnBoarding = e.DateOfOnBoarding,
                                 DateOfExit = e.DateOfExit,

                                 IsActive = e.IsActive,
                                 HasPermanent = e.HasPermanent,
                                 IsEditAllowed = e.IsEditAllowed,
                                 IsInfoVerified = e.IsInfoVerified,
                             })
                             .AsNoTracking();


                // 🔹 Count
                int totalCount = await query.CountAsync();

                // 🔹 Paging
                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 👉 NOTE: Yaha AutoMapper nahi chahiye, data already DTO me hai.
                // var mappedData = _mapper.Map<List<GetBaseEmployeeResponseDTO>>(pagedData);

                // 🔹 Prepare paged response
                var result = new PagedResponseDTO<GetBaseEmployeeResponseDTO>
                {
                    Items = pagedData,
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


        public async Task<GetBaseEmployeeResponseDTO> CreateEmployeeAsync(Employee entity)
        {
            try
            {
                // 🔹 Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                if (string.IsNullOrWhiteSpace(entity.FirstName))
                    throw new ArgumentException("First name is required.");

                // 🔹 Insert Employee
                await _context.Employees.AddAsync(entity);

                // 🔹 Insert Employee Image (same transaction)
                var employeeImage = new EmployeeImage
                {
                    Employee = entity,   // 🔥 Correct
                    TenantId = entity.TenantId,
                    IsPrimary = true,
                    HasImageUploaded = false,
                    IsActive = true,
                    AddedById = entity.AddedById,
                    AddedDateTime = DateTime.UtcNow,
                    FileType = 1
                };

                await _context.EmployeeImages.AddAsync(employeeImage);

                // 🔹 Save once
                await _context.SaveChangesAsync();

                // 🔹 Fetch ONLY newly created employee
                var result = await (
                    from e in _context.Employees

                    join d in _context.Designations on e.DesignationId equals d.Id into des
                    from d in des.DefaultIfEmpty()

                    join dep in _context.Departments on e.DepartmentId equals dep.Id into dept
                    from dep in dept.DefaultIfEmpty()

                    join g in _context.Genders on e.GenderId equals g.Id into gen
                    from g in gen.DefaultIfEmpty()

                    where e.Id == entity.Id && e.TenantId == entity.TenantId

                    select new GetBaseEmployeeResponseDTO
                    {
                        Id = e.Id.ToString(),
                        EmployementCode = e.EmployementCode,

                        FirstName = e.FirstName,
                        MiddleName = e.MiddleName,
                        LastName = e.LastName,

                        GenderId = e.GenderId,
                        GenderName = g.GenderName,

                        DesignationId = e.DesignationId,
                        DesignationName = d.DesignationName,

                        DepartmentId = e.DepartmentId,
                        DepartmentName = dep.DepartmentName,

                        OfficialEmail = e.OfficialEmail,
                        EmployeeTypeId = e.EmployeeTypeId,
                        
                        DateOfBirth = e.DateOfBirth,
                        DateOfOnBoarding = e.DateOfOnBoarding,
                        DateOfExit = e.DateOfExit,

                        IsActive = e.IsActive,
                        HasPermanent = e.HasPermanent,
                        IsEditAllowed = e.IsEditAllowed,
                        IsInfoVerified = e.IsInfoVerified
                    }
                ).AsNoTracking().FirstOrDefaultAsync();

                if (result == null)
                    throw new Exception("Employee created but fetch failed.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while creating employee for TenantId {TenantId}", entity?.TenantId);
                throw;
            }
        }


        public async Task<bool> UpdateVerifyEditStatusAsync(string sectionType,long employeeId,bool? isVerified,bool? isEditAllowed,bool? isActive,long userId)
        {
            if (employeeId <= 0 || string.IsNullOrWhiteSpace(sectionType))
                return false;   

            sectionType = sectionType.Trim().ToLowerInvariant();

            DateTime now = DateTime.UtcNow;

            // 🔥 COMMON LOCAL FUNCTION — ALL TABLES FOLLOW SAME PATTERN
            async Task<bool> UpdateMainTable<TEntity>(DbSet<TEntity> dbSet)
                where TEntity : class
             {
                var rows = await dbSet
                    .Where(x => EF.Property<long>(x, "EmployeeId") == employeeId &&
                                (bool?)EF.Property<bool?>(x, "IsSoftDeleted") != true)
                    .ToListAsync();

                if (rows.Count == 0)
                    return false;

                foreach (var row in rows)
                {
                    // NULL → FALSE conversion here
                    typeof(TEntity).GetProperty("IsInfoVerified")?
                        .SetValue(row, isVerified ?? false);

                    typeof(TEntity).GetProperty("IsEditAllowed")?
                        .SetValue(row, isEditAllowed ?? false);

                    typeof(TEntity).GetProperty("IsActive")?
                        .SetValue(row, isActive ?? false);

                    typeof(TEntity).GetProperty("InfoVerifiedById")?
                        .SetValue(row, userId);

                    typeof(TEntity).GetProperty("InfoVerifiedDateTime")?
                        .SetValue(row, now);

                  //  typeof(TEntity).GetProperty("UpdatedById")?
                     //   .SetValue(row, userId);

                //    typeof(TEntity).GetProperty("UpdatedDateTime")?
                       // .SetValue(row, now);
                }



                return true;
            }

            bool mainUpdated = false;

            switch (sectionType)
            {
                case "education":
                    mainUpdated = await UpdateMainTable(_context.EmployeeEducations);
                    break;

                case "experience":
                    mainUpdated = await UpdateMainTable(_context.EmployeeExperiences);

                    if (mainUpdated)
                    {
                        // 🔥 ExperienceDetails update — SAME PATTERN — ONE extra hit only
                        var details = await _context.EmployeeExperienceDetails
                            .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                            .ToListAsync();

                        if (details.Count > 0)
                        {
                            foreach (var d in details)
                            {
                                if (isVerified.HasValue) d.IsInfoVerified = isVerified.Value;
                                if (isEditAllowed.HasValue) d.IsEditAllowed = isEditAllowed.Value;
                                if (isActive.HasValue) d.IsActive = isActive.Value;
                                d.UpdatedById= d.InfoVerifiedById = userId;
                                d.UpdatedDateTime = d.InfoVerifiedDateTime = now;

                            }
                        }
                    }
                    break;

                case "bank":
                    mainUpdated = await UpdateMainTable(_context.EmployeeBankDetails);
                    break;

                default:
                    return false;
            }

            if (mainUpdated)
                await _context.SaveChangesAsync();

            return mainUpdated;
        }


        private async Task<bool> UpdateTableAsync<TEntity>( DbSet<TEntity> table,  long employeeId,  bool? isVerified,  bool? isEditAllowed,  bool? isActive, long? userId) where TEntity : class
        {
            // FILTER → EmployeeId match + IsSoftDeleted != true
            var entity = await table.FirstOrDefaultAsync(x =>
                EF.Property<long>(x, "EmployeeId") == employeeId &&
                (EF.Property<bool?>(x, "IsSoftDeleted") != true)
            );

            if (entity == null)
                return false;

            // --- Update IsInfoVerified ---
            if (isVerified.HasValue)
            {
                typeof(TEntity).GetProperty("IsInfoVerified")?.SetValue(entity, isVerified.Value);

                if (isVerified.Value == true)
                {
                    typeof(TEntity).GetProperty("InfoVerifiedDateTime")?.SetValue(entity, DateTime.UtcNow);
                    typeof(TEntity).GetProperty("InfoVerifiedById")?.SetValue(entity, userId ?? 0);
                }
                else
                {
                    typeof(TEntity).GetProperty("InfoVerifiedDateTime")?.SetValue(entity, null);
                    typeof(TEntity).GetProperty("InfoVerifiedById")?.SetValue(entity, null);
                }
            }

            // --- Update IsEditAllowed ---
            if (isEditAllowed.HasValue)
                typeof(TEntity).GetProperty("IsEditAllowed")?.SetValue(entity, isEditAllowed.Value);

            // --- Update IsActive ---
            if (isActive.HasValue)
                typeof(TEntity).GetProperty("IsActive")?.SetValue(entity, isActive.Value);

            // --- UpdatedDateTime ---
            typeof(TEntity).GetProperty("UpdatedDateTime")?.SetValue(entity, DateTime.UtcNow);

            await _context.SaveChangesAsync();
            return true;
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

        public async Task<string?> ProfileImage(long employeeId)
        {
            try
            {
                // Fetch latest primary active image
                var image = await _context.EmployeeImages
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId
                            && x.IsActive == true
                            && x.IsPrimary == true
                            && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.AddedDateTime) // latest preference
                    .Select(x => x.FilePath) // Only fetch needed column (performance)
                    .FirstOrDefaultAsync();

                // If no image found → return default placeholder
                return string.IsNullOrWhiteSpace(image)
                    ? null
                    : image;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error fetching profile image for EmployeeId: {employeeId}");

                // Fail-safe fallback → never throw
                return "/assets/images/default-profile.png";
            }
        }



        public async Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> GetImage(GetEmployeeImageRequestDTO dto)
        {
            try
            {
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                var query = _context.EmployeeImages
                    .AsNoTracking()
                    .Where(x =>
                        x.IsSoftDeleted != true &&
                        x.TenantId == dto.Prop.TenantId &&
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        x.IsPrimary == true);

                if (dto.IsActive)
                    query = query.Where(x => x.IsActive == dto.IsActive);

                int totalCount = await query.CountAsync();

                // 🚨 No Data Condition — Safe Return
                if (totalCount == 0)
                {
                    return new PagedResponseDTO<GetEmployeeImageReponseDTO>
                    {
                        Items = new List<GetEmployeeImageReponseDTO>(),
                        TotalCount = 0,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        IsPrimaryMarked = false
                    };
                }

                bool hasPrimary = await query.AnyAsync(x => x.HasImageUploaded == true);

                var pagedDataRaw = await query
                    .OrderByDescending(x => x.Id)
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
                    HasImageUploaded = x.HasImageUploaded ,
                    FileName = x.FileName
                }).ToList();

                return new PagedResponseDTO<GetEmployeeImageReponseDTO>
                {
                    Items = pagedData,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    IsPrimaryMarked = hasPrimary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching employee images.");

                // 🔥 SAFE FALLBACK RETURN (no crash)
                return new PagedResponseDTO<GetEmployeeImageReponseDTO>
                {
                    Items = new List<GetEmployeeImageReponseDTO>(),
                    TotalCount = 0,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize,
                    IsPrimaryMarked = false
                };
            }
        }


        //public async Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> GetImage(GetEmployeeImageRequestDTO dto, long decryptedTenantId)
        //{
        //    await using var context = await _contextFactory.CreateDbContextAsync();

        //    try
        //    {
        //        // Pagination defaults
        //        int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
        //        int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

        //        // Base query
        //        var query = context.EmployeeImages
        //            .AsNoTracking()
        //            .Where(x => x.IsSoftDeleted != true &&
        //                        x.TenantId == decryptedTenantId);

        //        if (dto.IsActive)
        //            query = query.Where(x => x.IsActive == dto.IsActive);

        //        if (dto._EmployeeId > 0)
        //            query = query.Where(x => x.EmployeeId == dto._EmployeeId);

        //        if (dto.Id_long > 0)
        //            query = query.Where(x => x.Id == dto.Id_long);

        //        // Safe sorting
        //        bool isAscending =
        //            !string.IsNullOrWhiteSpace(dto.SortOrder) &&
        //            dto.SortOrder.Trim().Equals("asc", StringComparison.OrdinalIgnoreCase);

        //        query = isAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);

        //        int totalCount = await query.CountAsync();

        //        // Just check primary, don't load all rows
        //        bool hasPrimary = await query.AnyAsync(x => x.IsPrimary == true);

        //        double completionPercentage = hasPrimary ? 100 : 0;

        //        // Paging
        //        var pagedDataRaw = await query
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        var pagedData = pagedDataRaw.Select(x => new GetEmployeeImageReponseDTO
        //        {
        //            EmployeeId = x.EmployeeId.ToString(),
        //            Id = x.Id.ToString(),
        //            FilePath = x.FilePath,
        //            IsActive = x.IsActive,
        //            IsPrimary = x.IsPrimary,
        //            CompletionPercentage = completionPercentage
        //        }).ToList();

        //        return new PagedResponseDTO<GetEmployeeImageReponseDTO>
        //        {
        //            Items = pagedData,
        //            TotalCount = totalCount,
        //            PageNumber = pageNumber,
        //            PageSize = pageSize,
        //            CompletionPercentage = completionPercentage,
        //            IsPrimaryMarked = hasPrimary
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "❌ Error occurred while fetching employee images.");
        //        throw;
        //    }
        //}


        public async Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(GetBaseEmployeeRequestDTO dto)
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
                    .Where(x => x.TenantId == dto.Prop.TenantId && x.IsSoftDeleted == null || x.IsSoftDeleted==false);
                
                // 🧩 Step 3: Safe parse helpers (convert string → long safely)
                   
                int designationId = SafeParser.TryParseInt(dto.DesignationId);


                // 🧩 Step 4: Dynamic filters (null-safe + condition-based)
                if (dto.Prop.EmployeeId > 0)
                    query = query.Where(x => x.Id == dto.Prop.EmployeeId);
                // yaha par id_long check kar rahe hai or 0 record aa raha hai, 

                if (dto.IsActive)
                    query = query.Where(x => x.IsActive == dto.IsActive);

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

                if (dto.TypeId > 0)
                    query = query.Where(x => x.EmployeeTypeId == dto.TypeId);

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
         GenderId = x.GenderId,
         DateOfBirth = x.DateOfBirth,
         DateOfOnBoarding = x.DateOfOnBoarding,
         DateOfExit = x.DateOfExit,
         DesignationId = x.DesignationId,
         EmployeeTypeId = x.EmployeeTypeId,
         DepartmentId = x.DepartmentId,
         OfficialEmail = x.OfficialEmail,
         HasPermanent = x.HasPermanent,
         IsActive = x.IsActive,
         IsEditAllowed = x.IsEditAllowed,
         IsInfoVerified = x.IsInfoVerified,

         // ⭐ New Fields (JOIN base lookup)
         DesignationName = context.Designations
             .Where(d => d.Id == x.DesignationId)
             .Select(d => d.DesignationName)
             .FirstOrDefault(),

         DepartmentName = context.Departments
             .Where(dep => dep.Id == x.DepartmentId)
             .Select(dep => dep.DepartmentName)
             .FirstOrDefault(),
         Type = context.EmployeeTypes
             .Where(ty => ty.Id == x.EmployeeTypeId)
             .Select(name => name.TypeName)
             .FirstOrDefault(),
         // ⭐ New Fields (JOIN base lookup)
         GenderName = context.Genders
             .Where(g => g.Id == x.GenderId)
             .Select(g => g.GenderName)
             .FirstOrDefault(),

         //RoleName = context.Roles
         //    .Where(r => r.Id == x.ro)
         //    .Select(r => r.Name)
         // .FirstOrDefault()
         // 🔥 Base Employee Completion Calculation
         CompletionPercentage = Math.Round(
             (
                 (string.IsNullOrWhiteSpace(x.FirstName) ? 0 : 1) +
                 (string.IsNullOrWhiteSpace(x.LastName) ? 0 : 1) +
                 (x.GenderId > 0 ? 1 : 0) +
                 (x.DateOfBirth != null ? 1 : 0) +
                 (x.DateOfOnBoarding != null ? 1 : 0) +
                 (x.DesignationId > 0 ? 1 : 0) +
                 (x.DepartmentId > 0 ? 1 : 0) +
                 (!string.IsNullOrWhiteSpace(x.OfficialEmail) ? 1 : 0) +
                 (x.HasPermanent ? 1 : 0) +
                 (x.IsActive ? 1 : 0)
             ) / 10.0 * 100, 0)
     })
     .ToListAsync();


                // 🧩 Step 7: Final paged response
                return new PagedResponseDTO<GetBaseEmployeeResponseDTO>
                {
                    Items = records,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalCount/pageSize),
                    CompletionPercentage = 0, // Placeholder
                   

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching base employee info.");
                throw new Exception($"Failed to fetch base employee info: {ex.Message}");
            }
        }

        public async Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(GetAllEmployeeInfoRequestDTO dto)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // ----------------------------------------------------
                // 1️⃣ BASE QUERY (LEFT JOIN + ASNO TRACKING)
                // ----------------------------------------------------
                var baseQuery =
                    from emp in context.Employees.AsNoTracking()

                    join gender in context.Genders
                        on emp.GenderId equals (long?)gender.Id into genderJoin
                    from g in genderJoin.DefaultIfEmpty()

                    join designation in context.Designations
                        on emp.DesignationId equals (long?)designation.Id into desigJoin
                    from d in desigJoin.DefaultIfEmpty()

                    join empType in context.EmployeeTypes
                        on emp.EmployeeTypeId equals (long?)empType.Id into typeJoin
                    from et in typeJoin.DefaultIfEmpty()

                    join department in context.Departments
                        on emp.DepartmentId equals (long?)department.Id into deptJoin
                    from dep in deptJoin.DefaultIfEmpty()

                    where emp.TenantId == dto.Prop.TenantId && emp.IsSoftDeleted != true

                    select new
                    {
                        emp,
                        GenderName = g.GenderName ?? "",
                        DesignationName = d.DesignationName ?? "",
                        EmployeeTypeName = et.TypeName ?? "",
                        DepartmentName = dep.DepartmentName ?? ""
                    };

                // ----------------------------------------------------
                // 2️⃣ FILTERS  (Optimized)
                // ----------------------------------------------------
                if (dto.Prop.EmployeeId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.Id == dto.Prop.EmployeeId);

                if (!string.IsNullOrWhiteSpace(dto.EmailId))
                    baseQuery = baseQuery.Where(x => x.emp.OfficialEmail == dto.EmailId.Trim());

                if (dto.DepartmentId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.DepartmentId == dto.DepartmentId);

                if (dto.DesignationId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.DesignationId == dto.DesignationId);

                // ⭐⭐⭐ IMPORTANT FIX ⭐⭐⭐
                if (dto.EmployeeTypeId.HasValue && dto.EmployeeTypeId.Value > 0)
                    baseQuery = baseQuery.Where(x => x.emp.EmployeeTypeId == dto.EmployeeTypeId.Value);

                if (dto.GenderId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.GenderId == dto.GenderId);

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    baseQuery = baseQuery.Where(x => x.emp.FirstName.Contains(dto.FirstName));

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                    baseQuery = baseQuery.Where(x => x.emp.LastName.Contains(dto.LastName));

                if (!string.IsNullOrWhiteSpace(dto.EmployementCode))
                    baseQuery = baseQuery.Where(x => x.emp.EmployementCode.Contains(dto.EmployementCode));

                int totalCount = await baseQuery.CountAsync();

                // ----------------------------------------------------
                // 3️⃣ PAGING
                // ----------------------------------------------------
                var pagedEmployees = await baseQuery
                    .OrderBy(x => x.emp.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // ----------------------------------------------------
                // 4️⃣ IMAGES (Optimized)
                // ----------------------------------------------------
                var ids = pagedEmployees.Select(x => x.emp.Id).ToList();

                var images = await context.EmployeeImages
                    .Where(i => ids.Contains(i.EmployeeId) && i.IsSoftDeleted != true)
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenByDescending(i => i.HasImageUploaded)
                    .ThenByDescending(i => i.Id)
                    .ToListAsync();

                var imgLookup = images
                    .GroupBy(i => i.EmployeeId)
                    .ToDictionary(g => g.Key, g => g.FirstOrDefault());

                var hasPrimaryLookup = images
                    .Where(i => i.IsPrimary == true)
                    .Select(i => i.EmployeeId)
                    .ToHashSet();

                // ----------------------------------------------------
                // 5️⃣ BUILD OUTPUT
                // ----------------------------------------------------
                var result = pagedEmployees.Select(x =>
                {
                    imgLookup.TryGetValue(x.emp.Id, out var img);
                    bool hasPrimary = hasPrimaryLookup.Contains(x.emp.Id);

                    int completed = new[]
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
                hasPrimary ? 1 : 0
            }.Sum();

                    return new GetAllEmployeeInfoResponseDTO
                    {
                        EmployeeId = x.emp.Id.ToString(),
                        EmployementCode = x.emp.EmployementCode,
                        FirstName = x.emp.FirstName,
                        LastName = x.emp.LastName,
                        DateOfOnBoarding = x.emp.DateOfOnBoarding?.ToString(),

                        GenderId = x.emp.GenderId,
                        EmployeeTypeId = x.emp.EmployeeTypeId,
                        DesignationId = x.emp.DesignationId,
                        DepartmentId = x.emp.DepartmentId,

                        GenderName = x.GenderName,
                        EmployeeTypeName = x.EmployeeTypeName,
                        DesignationName = x.DesignationName,
                        DepartmentName = x.DepartmentName,

                        OfficialEmail = x.emp.OfficialEmail,
                        EmployeeImagePath = img?.FilePath,
                        HasImagePicUploaded = hasPrimary,

                        CompletionPercentage = (completed / 10.0) * 100
                    };
                }).ToList();

                return new PagedResponseDTO<GetAllEmployeeInfoResponseDTO>
                {
                    Items = result,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FAILED in GetAllInfo");

                return new PagedResponseDTO<GetAllEmployeeInfoResponseDTO>
                {
                    Items = new List<GetAllEmployeeInfoResponseDTO>(),
                    TotalCount = 0,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize,
                    TotalPages = 0
                };
            }
        }



        //public async Task<EmployeeProfileCompletionDTO> GetEmployeeCompletionAsync(long employeeId)
        //{
        //    if (employeeId <= 0)
        //        return new EmployeeProfileCompletionDTO();

        //    // Run all queries in parallel (fastest)
        //    var bankTask = _context.EmployeeBankDetails
        //        .AsNoTracking()
        //        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
        //        .OrderByDescending(x => x.IsPrimaryAccount)
        //        .Select(x => new
        //        {
        //            x.BankName,
        //            x.AccountNumber,
        //            x.IFSCCode,
        //            x.BranchName,
        //            x.AccountType,
        //            x.IsPrimaryAccount,
        //            x.HasChequeDocUploaded,
        //            x.IsInfoVerified,
        //            x.IsEditAllowed
        //        })
        //        .FirstOrDefaultAsync();

        //    var eduTask = _context.EmployeeEducations
        //        .AsNoTracking()
        //        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
        //        .Select(x => new
        //        {
        //            x.Degree,
        //            x.InstituteName,
        //            x.StartDate,
        //            x.EndDate,
        //            x.IsInfoVerified,
        //            x.IsEditAllowed
        //        })
        //        .ToListAsync();

        //    //var expTask = _context.EmployeeExperienceDetails
        //    //    .AsNoTracking()
        //    //    .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
        //    //    .Select(x => new
        //    //    {
        //    //        x.CompanyName,
        //    //        x.StartDate,
        //    //        x.EndDate,
        //    //        x.HasExpLetterUploaded,
        //    //        x.IsInfoVerified,
        //    //        x.IsEditAllowed
        //    //    })
        //    //    .ToListAsync();

        //    await Task.WhenAll(bankTask, eduTask);

        //    return new EmployeeProfileCompletionDTO
        //    {
        //        Bank = _context.EmployeeEducations.CalculateEducationCompletion(bankTask.Result),
        //        Education = _context.EmployeeEducations.c(eduTask.Result),
        //        Experience = CalculateExperiencePercentage(expTask.Result)
        //    };
        //}

        public async Task<List<CompletionSectionDTO>> GetEmployeeCompletionAsync(long employeeId)
        {
            try
            {
                // ❗If bad employeeId, return empty list safely
                if (employeeId <= 0)
                    return new List<CompletionSectionDTO>();

                // 📚 Fetch education rows
                var eduList = await _context.EmployeeEducations
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                    .Select(x => new EducationRowDTO
                    {
                        Degree = x.Degree,
                        InstituteName = x.InstituteName,
                        ScoreType = x.ScoreType,
                        HasEducationDocUploded = x.HasEducationDocUploded,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        IsEditAllowed = x.IsEditAllowed,
                        IsInfoVerified = x.IsInfoVerified

                    })
                    .ToListAsync();

                // 🧮 Calculate completion %
                var educationSection = eduList.CalculateEducationCompletionDTO();

                return new List<CompletionSectionDTO> { educationSection };
            }
            catch (Exception ex)
            {
                // 🔥 Log error safely
                _logger.LogError(ex,
                    "Error in GetEmployeeCompletionAsync for EmployeeId: {EmployeeId}",
                    employeeId);

                // ❗Never throw — return empty list to avoid API crash
                return new List<CompletionSectionDTO>();
            }
        }




        // single object
        //    public async Task<List<CompletionSectionDTO>> GetEmployeeCompletionAsync(long employeeId)
        //    {
        //        if (employeeId <= 0)
        //            return new List<CompletionSectionDTO>();

        //        var eduTask = _context.EmployeeEducations
        //            .AsNoTracking()
        //            .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted!=true)
        //            .Select(x => new
        //            {
        //                x.Degree,
        //                x.InstituteName,
        //                x.ScoreType,
        //                x.HasEducationDocUploded,
        //                x.StartDate,
        //                x.EndDate,
        //                x.IsInfoVerified,
        //                x.IsEditAllowed
        //            })
        //            .FirstOrDefaultAsync();

        //        await Task.WhenAll(eduTask);

        //        var edu = await eduTask;

        //        return new List<CompletionSectionDTO>
        //            {
        //                  edu != null
        //                   ? new List<dynamic> { edu }.CalculateEducationCompletion()
        //                   : new CompletionSectionDTO
        //            {
        //            SectionName = "Education",
        //            CompletionPercent = 0,
        //            IsInfoVerified = false,
        //            IsEditAllowed = true,
        //            IsSectionCreate = false
        //        }
        //};
        //    }


        //public async Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(GetAllEmployeeInfoRequestDTO dto,
        //        long decryptedTenantId)
        // {
        //    await using var context = await _contextFactory.CreateDbContextAsync();
        //    try
        //    {
        //        int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
        //        int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

        //        // -----------------------------------
        //        // 1️⃣ BASE QUERY
        //        // -----------------------------------
        //        var baseQuery =
        //            from emp in context.Employees.AsNoTracking()
        //            where emp.TenantId == decryptedTenantId && emp.IsSoftDeleted != true

        //            join g in context.Genders on emp.GenderId equals g.Id into genderJoin
        //            from gender in genderJoin.DefaultIfEmpty()

        //            join d in context.Designations on emp.DesignationId equals d.Id into desigJoin
        //            from designation in desigJoin.DefaultIfEmpty()

        //            join et in context.EmployeeTypes on emp.EmployeeTypeId equals et.Id into typeJoin
        //            from empType in typeJoin.DefaultIfEmpty()

        //            join dep in context.Departments on emp.DepartmentId equals dep.Id into deptJoin
        //            from department in deptJoin.DefaultIfEmpty()


        //        select new
        //            {
        //                emp,
        //                GenderName = gender.GenderName ?? "",
        //                DesignationName = designation.DesignationName ?? "",
        //                EmployeeTypeName = empType.TypeName ?? "",
        //                DepartmentName = department.DepartmentName ?? ""
        //            };

        //        // -----------------------------------
        //        // 2️⃣ FILTERS
        //        // -----------------------------------
        //        if (dto.Id_long > 0)
        //            baseQuery = baseQuery.Where(x => x.emp.Id == dto.Id_long);

        //        if (!string.IsNullOrWhiteSpace(dto.EmailId))
        //            baseQuery = baseQuery.Where(x => x.emp.OfficialEmail == dto.EmailId);

        //        if (dto._DepartmentId > 0)
        //            baseQuery = baseQuery.Where(x => x.emp.DepartmentId == dto._DepartmentId);

        //        if (dto._DesignationId > 0)
        //            baseQuery = baseQuery.Where(x => x.emp.DesignationId == dto._DesignationId);

        //        if (dto._EmployeeTypeId > 0)
        //            baseQuery = baseQuery.Where(x => x.emp.EmployeeTypeId == dto._EmployeeTypeId);

        //        if (dto._GenderId > 0)
        //            baseQuery = baseQuery.Where(x => x.emp.GenderId == dto._GenderId);

        //        if (!string.IsNullOrWhiteSpace(dto.FirstName))
        //            baseQuery = baseQuery.Where(x => x.emp.FirstName.Contains(dto.FirstName));

        //        if (!string.IsNullOrWhiteSpace(dto.LastName))
        //            baseQuery = baseQuery.Where(x => x.emp.LastName.Contains(dto.LastName));

        //        if (!string.IsNullOrWhiteSpace(dto.EmployementCode))
        //            baseQuery = baseQuery.Where(x => x.emp.EmployementCode.Contains(dto.EmployementCode));

        //        if (!string.IsNullOrWhiteSpace(dto.EmployeeTypeId))
        //            baseQuery = baseQuery.Where(x => x.emp.EmployementCode.Contains(dto.EmployementCode));


        //        int totalCount = await baseQuery.CountAsync();

        //        // -----------------------------------
        //        // 3️⃣ PAGING
        //        // -----------------------------------
        //        var pagedEmployees = await baseQuery
        //            .OrderBy(x => x.emp.Id)
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        // -----------------------------------
        //        // 4️⃣ FETCH IMAGES
        //        // -----------------------------------
        //        var ids = pagedEmployees.Select(x => x.emp.Id).ToList();

        //        var images = await context.EmployeeImages
        //            .Where(i => ids.Contains(i.EmployeeId) && i.IsSoftDeleted != true)
        //            .ToListAsync();

        //        // -----------------------------------
        //        // 5️⃣ BUILD OUTPUT
        //        // -----------------------------------
        //        var result = pagedEmployees.Select(x =>
        //        {
        //            var img = images
        //                .Where(i => i.EmployeeId == x.emp.Id)
        //                .OrderByDescending(i => i.IsPrimary)
        //                .ThenByDescending(i => i.HasImageUploaded)
        //                .ThenByDescending(i => i.Id)
        //                .FirstOrDefault();

        //            bool hasPrimary = images.Any(i =>
        //                i.EmployeeId == x.emp.Id &&
        //                i.IsPrimary == true &&
        //                i.IsSoftDeleted != true);

        //            int completed = new[]
        //            {
        //        string.IsNullOrEmpty(x.emp.FirstName) ? 0 : 1,
        //        string.IsNullOrEmpty(x.emp.LastName) ? 0 : 1,
        //        x.emp.DateOfOnBoarding == null ? 0 : 1,
        //        x.emp.DesignationId > 0 ? 1 : 0,
        //        x.emp.DepartmentId > 0 ? 1 : 0,
        //        string.IsNullOrEmpty(x.emp.OfficialEmail) ? 0 : 1,
        //        x.emp.IsActive == true ? 1 : 0,
        //        x.emp.IsEditAllowed == true ? 1 : 0,
        //        x.emp.IsInfoVerified == true ? 1 : 0,
        //        hasPrimary ? 1 : 0
        //    }.Sum();

        //            return new GetAllEmployeeInfoResponseDTO
        //            {
        //                EmployeeId = x.emp.Id.ToString(),
        //                EmployementCode = x.emp.EmployementCode,
        //                FirstName = x.emp.FirstName,
        //                LastName = x.emp.LastName,
        //                DateOfOnBoarding = x.emp.DateOfOnBoarding?.ToString(),


        //                GenderId = x.emp.GenderId.ToString(),
        //                EmployeeTypeId = x.emp.EmployeeTypeId.ToString(),

        //                DesignationId = x.emp.DesignationId.ToString(),
        //                DepartmentId = x.emp.DepartmentId.ToString(),

        //                GenderName = x.GenderName,
        //                EmployeeTypeName = x.EmployeeTypeName,
        //                DesignationName = x.DesignationName,
        //                DepartmentName = x.DepartmentName,
        //                OfficialEmail = x.emp.OfficialEmail,


        //                EmployeeImagePath = img?.FilePath,
        //                HasImagePicUploaded = hasPrimary,

        //                CompletionPercentage = (completed / 10.0) * 100
        //            };
        //        }).ToList();

        //        return new PagedResponseDTO<GetAllEmployeeInfoResponseDTO>
        //        {
        //            Items = result,
        //            TotalCount = totalCount,
        //            PageNumber = pageNumber,
        //            PageSize = pageSize,
        //            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),


        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "FAILED in GetAllInfo");

        //        return new PagedResponseDTO<GetAllEmployeeInfoResponseDTO>
        //        {
        //            Items = new List<GetAllEmployeeInfoResponseDTO>(),
        //            TotalCount = 0,
        //            PageNumber = dto.PageNumber,
        //            PageSize = dto.PageSize,
        //            TotalPages = 0,
        //              // ⭐ ALWAYS RETURNS VALID ERROR
        //        };
        //    }
        //}


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


        public async Task<bool> UpdateEmployeeAsync(Employee entity, long tenantId)      
        {
            _context.Employees.Update(entity);
            await _context.SaveChangesAsync();
            return true;
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

        public async Task<EmployeeImage?> IsImageExist(long? id, bool isActive)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                return await context.EmployeeImages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(img =>
                        img.Id == id &&
                        img.IsActive == isActive &&
                        img.IsSoftDeleted != true &&
                        img.IsPrimary == true
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error while checking image existence for Id: {id}");
                return null;
            }
        }

        public async Task<Employee?> GetByIdAsync(long id, long tenantId, bool track = true)
        {
            IQueryable<Employee> query = _context.Employees.Where(x => x.TenantId == tenantId && x.IsSoftDeleted!=true);

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateProfileImage(EmployeeImage employeeImageInfo)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                context.EmployeeImages.Update(employeeImageInfo);

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error while updating employee image (Id: {employeeImageInfo?.Id})");
                return false;
            }
        }





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



  




 
 






