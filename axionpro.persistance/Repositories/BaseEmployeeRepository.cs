using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
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
using System.Diagnostics.Metrics;
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
#if DEBUG
            var entityType = _context.Model.FindEntityType(typeof(Employee));

            if (entityType != null)
            {
                foreach (var p in entityType.GetProperties())
                {
                    Console.WriteLine($"{p.Name} | Shadow: {p.IsShadowProperty()}");
                }
            }
#endif

        }

        public Task<long> AutoCreated(Employee entity)
        {
            throw new NotImplementedException();
        }



        public async Task<Employee> CreateEmployeeAsync( Employee employee, LoginCredential loginCredential)
        {
            await _context.Employees.AddAsync(employee);

            var employeeImage = new EmployeeImage
            {
                Employee = employee,
                TenantId = employee.TenantId,
                IsPrimary = true,
                HasImageUploaded = false,
                IsActive = true,
                AddedById = employee.AddedById,
                AddedDateTime = DateTime.UtcNow,
                FileType = 1
            };

            await _context.EmployeeImages.AddAsync(employeeImage);
            await _context.LoginCredentials.AddAsync(loginCredential);
            await _context.SaveChangesAsync();
            return employee;
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
                             join country in _context.Countries
                              on e.CountryId equals (int)country.Id into countryJoin
                             from c in countryJoin.DefaultIfEmpty()

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

                                 GenderId = e.GenderId??0,
                                 GenderName = g.GenderName,
                                  CountryId = e.CountryId,
                                 Nationality= e.Country.CountryName,
                                 DesignationId = e.DesignationId ?? 0,
                                 DesignationName = d.DesignationName,

                                 DepartmentId = e.DepartmentId ?? 0,
                                 DepartmentName = dep.DepartmentName,

                                
                                 Type = et.TypeName,


                                 //  RoleName = r.RoleName,
                                 OfficialEmail = e.OfficialEmail,
                                 EmployeeTypeId = e.EmployeeTypeId ?? 0,

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

        /// <summary>
        /// Common fun to for all tabs to update IsEditAllowed status
        /// </summary>
        /// <param name="tabInfoType"></param>
        /// <param name="employeeId"></param>
        /// <param name="userEmployeeId"></param>
        /// <param name="isVerified"></param>
        /// <param name="ct"></param>
        /// <returns></returns>   
       
        public async Task<bool> UpdateEditableStatusByEntityAsync(
         int tabInfoType,
    long employeeId,
    long userEmployeeId,
    bool isVerified,
    CancellationToken ct)
        {
            if (employeeId <= 0)
                return false;

            if (!Enum.IsDefined(typeof(TabInfoType), tabInfoType))
                return false;

            DateTime now = DateTime.UtcNow;
            int affected = 0;

            switch ((TabInfoType)tabInfoType)
            {
                case TabInfoType.Employee:
                    affected = await _context.Employees
                        .Where(x => x.Id == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;

                case TabInfoType.Bank:
                    affected = await _context.EmployeeBankDetails
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;

                case TabInfoType.Contact:
                    affected = await _context.EmployeeContacts
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;

                case TabInfoType.Experience:
                    affected = await _context.EmployeeExperienceDetails
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;

                case TabInfoType.Identity:
                    affected = await _context.EmployeeIdentities
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;

                case TabInfoType.Education:
                    affected = await _context.EmployeeEducations
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;

                case TabInfoType.Dependent:
                    affected = await _context.EmployeeDependents
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsEditAllowed, isVerified)
                            .SetProperty(p => p.UpdatedById, userEmployeeId)
                            .SetProperty(p => p.UpdatedDateTime, now), ct);
                    break;
            }

            return affected > 0;
        }

        public async Task<bool> UpdateVerificationStatusByTabAsync(
    int tabInfoType,
    long employeeId,
    long userEmployeeId,
    bool isVerified,
    CancellationToken ct)
        {
            if (employeeId <= 0)
                return false;

            if (!Enum.IsDefined(typeof(TabInfoType), tabInfoType))
                return false;

            DateTime now = DateTime.UtcNow;
            int affected = 0;

            switch ((TabInfoType)tabInfoType)
            {
                case TabInfoType.Employee:
                    affected = await _context.Employees
                        .Where(x => x.Id == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;

                case TabInfoType.Bank:
                    affected = await _context.EmployeeBankDetails
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;

                case TabInfoType.Contact:
                    affected = await _context.EmployeeContacts
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;

                case TabInfoType.Experience:
                    affected = await _context.EmployeeExperienceDetails
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;

                case TabInfoType.Identity:
                    affected = await _context.EmployeeIdentities
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;

                case TabInfoType.Education:
                    affected = await _context.EmployeeEducations
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;

                case TabInfoType.Dependent:
                    affected = await _context.EmployeeDependents
                        .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.InfoVerifiedById, userEmployeeId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now), ct);
                    break;
            }

            return affected > 0;
        }


        //public async Task<bool> UpdateVerificationStatus(
        //        long employeeId,
        //         long userId,
        //         bool status)
        //{
        //    var employee = await _context.Employees
        //        .FirstOrDefaultAsync(x =>
        //            x.Id == employeeId &&
        //            x.IsSoftDeleted != true);

        //    if (employee == null)
        //        return false;

        //    employee.IsInfoVerified = status;
        //    employee.InfoVerifiedById = userId;
        //    employee.InfoVerifiedDateTime = DateTime.UtcNow;

        //    var rowsAffected = await _context.SaveChangesAsync();

        //    return rowsAffected > 0;
        //}

    //    public async Task<bool> UpdateEditStatus(
    //long employeeId,
    //long userId,
    //bool status)
    //    {
    //        var employee = await _context.Employees
    //            .FirstOrDefaultAsync(x =>
    //                x.Id == employeeId &&
    //                x.IsSoftDeleted != true);

    //        if (employee == null)
    //            return false;

    //        employee.IsEditAllowed = status;
    //        employee.UpdatedById = userId;
    //        employee.UpdatedDateTime = DateTime.UtcNow;

    //        var rowsAffected = await _context.SaveChangesAsync();

    //        return rowsAffected > 0;
    //    }

        public async Task<bool> UpdateSectionVerifyStatusAsync(
           int tabInfoType,
           long employeeId,
           long tenantId,
           bool isVerified,
           bool isEditAllowed,
           long userId,
           CancellationToken ct)
        {
            if (employeeId <= 0)
                return false;

            if (!Enum.IsDefined(typeof(TabInfoType), tabInfoType))
                return false;

            DateTime now = DateTime.UtcNow;
            int affected = 0;

            switch ((TabInfoType)tabInfoType)
            {
                // ================= EDUCATION =================
                case TabInfoType.Education:
                    affected = await _context.EmployeeEducations
                        .Where(x =>
                            x.EmployeeId == employeeId &&
                            x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.IsEditAllowed, isEditAllowed)
                            .SetProperty(p => p.InfoVerifiedById, userId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now),
                            ct);
                    break;

                // ================= BANK =================
                case TabInfoType.Bank:
                    affected = await _context.EmployeeBankDetails
                        .Where(x =>
                            x.EmployeeId == employeeId &&
                            x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.IsEditAllowed, isEditAllowed)
                            .SetProperty(p => p.InfoVerifiedById, userId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now),
                            ct);
                    break;

                // ================= EXPERIENCE =================
                case TabInfoType.Experience:
                    // 🔥 DETAILS TABLE
                    affected = await _context.EmployeeExperienceDetails
                        .Where(x =>
                            x.EmployeeId == employeeId &&
                            x.IsSoftDeleted != true)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsInfoVerified, isVerified)
                            .SetProperty(p => p.IsEditAllowed, isEditAllowed)
                            .SetProperty(p => p.InfoVerifiedById, userId)
                            .SetProperty(p => p.InfoVerifiedDateTime, now),
                            ct);
                    break;
            }

            return affected > 0;
        }


        //public async Task<bool> UpdateVerifyEditStatusAsyncisVerified, bool? isEditAllowed, bool? isActive, long userId)
        //{(string sectionType, long employeeId, bool? 
        //    if (employeeId <= 0 || string.IsNullOrWhiteSpace(sectionType))
        //        return false;

        //    sectionType = sectionType.Trim().ToLowerInvariant();

        //    DateTime now = DateTime.UtcNow;

        //    // 🔥 COMMON LOCAL FUNCTION — ALL TABLES FOLLOW SAME PATTERN
        //    async Task<bool> UpdateMainTable<TEntity>(DbSet<TEntity> dbSet)
        //        where TEntity : class
        //    {
        //        var rows = await dbSet
        //            .Where(x => EF.Property<long>(x, "EmployeeId") == employeeId &&
        //                        (bool?)EF.Property<bool?>(x, "IsSoftDeleted") != true)
        //            .ToListAsync();

        //        if (rows.Count == 0)
        //            return false;

        //        foreach (var row in rows)
        //        {
        //            // NULL → FALSE conversion here
        //            typeof(TEntity).GetProperty("IsInfoVerified")?
        //                .SetValue(row, isVerified ?? false);

        //            typeof(TEntity).GetProperty("IsEditAllowed")?
        //                .SetValue(row, isEditAllowed ?? false);

        //            typeof(TEntity).GetProperty("IsActive")?
        //                .SetValue(row, isActive ?? false);

        //            typeof(TEntity).GetProperty("InfoVerifiedById")?
        //                .SetValue(row, userId);

        //            typeof(TEntity).GetProperty("InfoVerifiedDateTime")?
        //                .SetValue(row, now);

        //            //  typeof(TEntity).GetProperty("UpdatedById")?
        //            //   .SetValue(row, userId);

        //            //    typeof(TEntity).GetProperty("UpdatedDateTime")?
        //            // .SetValue(row, now);
        //        }



        //        return true;
        //    }

        //    bool mainUpdated = false;

        //    switch (sectionType)
        //    {
        //        case "education":
        //            mainUpdated = await UpdateMainTable(_context.EmployeeEducations);
        //            break;

        //        case "experience":
        //            mainUpdated = await UpdateMainTable(_context.EmployeeExperiences);

        //            if (mainUpdated)
        //            {
        //                // 🔥 ExperienceDetails update — SAME PATTERN — ONE extra hit only
        //                var details = await _context.EmployeeExperienceDetails
        //                    .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
        //                    .ToListAsync();

        //                if (details.Count > 0)
        //                {
        //                    foreach (var d in details)
        //                    {
        //                        if (isVerified.HasValue) d.IsInfoVerified = isVerified.Value;
        //                        if (isEditAllowed.HasValue) d.IsEditAllowed = isEditAllowed.Value;
        //                        if (isActive.HasValue) d.IsActive = isActive.Value;
        //                        d.UpdatedById = d.InfoVerifiedById = userId;
        //                        d.UpdatedDateTime = d.InfoVerifiedDateTime = now;

        //                    }
        //                }
        //            }
        //            break;

        //        case "bank":
        //            mainUpdated = await UpdateMainTable(_context.EmployeeBankDetails);
        //            break;

        //        default:
        //            return false;
        //    }

        //    if (mainUpdated)
        //        await _context.SaveChangesAsync();

        //    return mainUpdated;
        //}



        public async Task<bool> ActivateAllEmployeeAsync(Employee employee, bool isActive)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (employee == null)
                    throw new Exception("Employee not found.");

                // 🔹 EMPLOYEE (MASTER)
                employee.IsActive = isActive;
                employee.IsEditAllowed = false;
                employee.IsInfoVerified = false;
                employee.UpdatedById = employee.Id;
                employee.UpdatedDateTime = DateTime.UtcNow;

                // 🔹 BANK DETAILS
                var bankDetails = await _context.EmployeeBankDetails
                    .Where(x => x.EmployeeId == employee.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var bank in bankDetails)
                {
                    bank.IsActive = isActive;
                    bank.IsEditAllowed = false;
                    bank.IsInfoVerified = false;
                    bank.UpdatedById = employee.Id;
                    bank.UpdatedDateTime = DateTime.UtcNow;
                }

                // 🔹 CONTACT DETAILS
                var contacts = await _context.EmployeeContacts
                    .Where(x => x.EmployeeId == employee.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var contact in contacts)
                {
                    contact.IsActive = isActive;
                    contact.IsEditAllowed = false;
                    contact.IsInfoVerified = false;
                    contact.UpdatedById = employee.Id;
                    contact.UpdatedDateTime = DateTime.UtcNow;
                }

                // 🔹 DEPENDENT DETAILS
                var dependents = await _context.EmployeeDependents
                    .Where(x => x.EmployeeId == employee.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var dep in dependents)
                {
                    dep.IsActive = isActive;
                    dep.IsEditAllowed = false;
                    dep.IsInfoVerified = false;
                    dep.UpdatedById = employee.Id;
                    dep.UpdatedDateTime = DateTime.UtcNow;
                }

                // 🔹 IMAGE DETAILS
                var images = await _context.EmployeeImages
                    .Where(x => x.EmployeeId == employee.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var img in images)
                {
                    img.IsActive = isActive;
                    img.UpdateById = employee.Id;
                    img.UpdatedDateTime = DateTime.UtcNow;
                }

                // 🔹 EDUCATION DETAILS
                var educations = await _context.EmployeeEducations
                    .Where(x => x.EmployeeId == employee.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var edu in educations)
                {
                    edu.IsActive = isActive;
                    edu.IsEditAllowed = false;
                    edu.IsInfoVerified = false;
                    edu.UpdatedById = employee.Id;
                    edu.UpdatedDateTime = DateTime.UtcNow;
                }

                // 🔹 PERSONAL / IDENTITY DETAILS
                var identities = await _context.EmployeePersonalDetails
                    .Where(x => x.EmployeeId == employee.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var iden in identities)
                {
                    iden.IsActive = isActive;
                    iden.IsEditAllowed = false;
                    iden.IsInfoVerified = false;
                    iden.UpdatedById = employee.Id;
                    iden.UpdatedDateTime = DateTime.UtcNow;
                }

                // 🔹 SAVE ONCE
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"Employee {employee.Id} {(isActive ? "activated" : "deactivated")} successfully."
                );

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Failed to update IsActive status for employee {employee?.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(Employee employee)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {


                if (employee == null)
                    throw new Exception("Employee not found.");

                employee.IsSoftDeleted = true;
                employee.SoftDeletedById = employee.Id;
                employee.DeletedDateTime = DateTime.UtcNow;
                employee.IsActive = false;
                employee.IsEditAllowed = false;
                employee.IsInfoVerified = false;

                // 🔹 BANK DETAILS
                var bankDetails = await _context.EmployeeBankDetails
                    .Where(x => x.EmployeeId == employee.Id
                        && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var bank in bankDetails)
                {
                    bank.IsSoftDeleted = employee.IsSoftDeleted;
                    bank.SoftDeletedById = employee.SoftDeletedById;
                    bank.DeletedDateTime = employee.DeletedDateTime;
                    bank.IsActive = employee.IsActive;
                    bank.IsEditAllowed = employee.IsEditAllowed ?? false;
                    bank.IsInfoVerified = employee.IsInfoVerified ?? false;

                }

                // 🔹 CONTACT DETAILS
                var contacts = await _context.EmployeeContacts
                    .Where(x => x.EmployeeId == employee.Id
                        && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var contact in contacts)
                {
                    contact.IsSoftDeleted = employee.IsSoftDeleted;
                    contact.SoftDeletedById = employee.SoftDeletedById;
                    contact.DeletedDateTime = employee.DeletedDateTime;
                    contact.IsActive = employee.IsActive;
                    contact.IsEditAllowed = employee.IsEditAllowed ?? false;
                    contact.IsInfoVerified = employee.IsInfoVerified ?? false;
                }

                // 🔹 Dependant DETAILS
                var dependants = await _context.EmployeeDependents
                    .Where(x => x.EmployeeId == employee.Id
                        && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var dep in dependants)
                {
                    dep.IsSoftDeleted = employee.IsSoftDeleted;
                    dep.SoftDeletedById = employee.SoftDeletedById;
                    dep.DeletedDateTime = employee.DeletedDateTime;
                    dep.IsActive = employee.IsActive;
                    dep.IsEditAllowed = employee.IsEditAllowed ?? false;
                    dep.IsInfoVerified = employee.IsInfoVerified ?? false;
                }
                // 🔹 Images DETAILS
                var images = await _context.EmployeeImages
                    .Where(x => x.EmployeeId == employee.Id
                        && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var img in images)
                {
                    img.IsSoftDeleted = employee.IsSoftDeleted ?? false;
                    img.SoftDeletedById = employee.SoftDeletedById;
                    img.DeletedDateTime = employee.DeletedDateTime;
                    img.IsActive = employee.IsActive;

                }

                // 🔹 Education DETAILS
                var educations = await _context.EmployeeEducations
                    .Where(x => x.EmployeeId == employee.Id
                        && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var edu in educations)
                {
                    edu.IsSoftDeleted = employee.IsSoftDeleted;
                    edu.SoftDeletedById = employee.SoftDeletedById;
                    edu.DeletedDateTime = employee.DeletedDateTime;
                    edu.IsActive = employee.IsActive;
                    edu.IsEditAllowed = employee.IsEditAllowed ?? false;
                    edu.IsInfoVerified = employee.IsInfoVerified ?? false;
                }
                // 🔹 Identity DETAILS
                var identites = await _context.EmployeePersonalDetails
                    .Where(x => x.EmployeeId == employee.Id
                        && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                foreach (var iden in identites)
                {
                    iden.IsSoftDeleted = employee.IsSoftDeleted;
                    iden.SoftDeletedById = employee.SoftDeletedById;
                    iden.DeletedDateTime = employee.DeletedDateTime;
                    iden.IsActive = employee.IsActive;
                    iden.IsEditAllowed = employee.IsEditAllowed ?? false;
                    iden.IsInfoVerified = employee.IsInfoVerified ?? false;
                }

                // 🔹 SAVE ONCE
                await _context.SaveChangesAsync();
                //  await transaction.CommitAsync();

                _logger.LogInformation($"Employee {employee.SoftDeletedById} soft deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Failed to soft delete employee {employee.SoftDeletedById}");
                throw;
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
        public async Task<GetEmployeeImageReponseDTO> AddImageAsync(EmployeeImage entity)
        {
            try
            {
                // ---------------------------------------------
                // 1️⃣ If this image is primary, remove current primary
                // ---------------------------------------------
                if (entity.IsPrimary)
                {
                    var existingPrimary = await _context.EmployeeImages
                        .Where(x =>
                            x.EmployeeId == entity.EmployeeId &&
                            x.IsPrimary == true &&
                            x.IsSoftDeleted != true)
                        .ToListAsync();

                    foreach (var img in existingPrimary)
                    {
                        img.IsPrimary = false;
                        img.UpdatedDateTime = DateTime.UtcNow;
                        img.UpdateById = entity.AddedById;
                    }
                }

                // ---------------------------------------------
                // 2️⃣ Insert new image
                // ---------------------------------------------
                await _context.EmployeeImages.AddAsync(entity);
                await _context.SaveChangesAsync();

                // ---------------------------------------------
                // 3️⃣ Fetch final primary image (ONLY ONE)
                // ---------------------------------------------
                var primaryImage = await _context.EmployeeImages
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == entity.EmployeeId &&
                        x.IsSoftDeleted != true &&
                        x.IsPrimary == true)
                    .OrderByDescending(x => x.AddedDateTime)
                    .FirstOrDefaultAsync();

                // ---------------------------------------------
                // 4️⃣ Build completion %
                // ---------------------------------------------
                double completionPercentage = primaryImage != null ? 100 : 0;

                // ---------------------------------------------
                // 5️⃣ Prepare response
                // ---------------------------------------------
                if (primaryImage == null)
                    primaryImage = entity; // fallback if no primary (rare case)

                return new GetEmployeeImageReponseDTO
                {
                    Id = primaryImage.Id,
                    EmployeeId = primaryImage.EmployeeId.ToString(),
                    FileName = primaryImage.FileName,
                    FilePath = primaryImage.FilePath,
                    IsActive = primaryImage.IsActive,
                    IsPrimary = primaryImage.IsPrimary,
                    HasImageUploaded = primaryImage.HasImageUploaded,
                    CompletionPercentage = completionPercentage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error occurred in AddImageAsync | EmployeeId={EmployeeId}",
                    entity?.EmployeeId);

                throw new Exception($"Failed to add / fetch image info: {ex.Message}");
            }
        }

        public async Task<GetEmployeeImageReponseDTO> GetImage(GetEmployeeImageRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("Fetching employee primary image | EmpId={Emp} | Tenant={Tenant}",
                    dto.Prop.EmployeeId, dto.Prop.TenantId);

                // ---------------------------------------------
                // 1️⃣ Base Query
                // ---------------------------------------------
                var query = _context.EmployeeImages
                    .AsNoTracking()
                    .Where(x =>
                        x.TenantId == dto.Prop.TenantId &&
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        x.IsSoftDeleted != true &&
                        x.IsPrimary == true);   // always fetch only primary

                // ---------------------------------------------
                // 2️⃣ Optional Active Filter
                // ---------------------------------------------
                if (dto.IsActive)
                    query = query.Where(x => x.IsActive == dto.IsActive);

                // ---------------------------------------------
                // 3️⃣ Fetch Single Primary Image
                // ---------------------------------------------
                var img = await query
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (img == null)
                {
                    // Safe fallback (no crash)
                    return new GetEmployeeImageReponseDTO
                    {
                        EmployeeId = dto.Prop.EmployeeId.ToString(),
                        IsActive = false,
                        IsPrimary = false,
                        HasImageUploaded = false,
                        CompletionPercentage = 0,
                        FilePath = null,
                        FileName = null
                    };
                }

                // ---------------------------------------------
                // 4️⃣ Completion Percentage Logic
                // ---------------------------------------------
                double completion = img.HasImageUploaded ? 100 : 0;

                // ---------------------------------------------
                // 5️⃣ Final Single Object Return
                // ---------------------------------------------
                return new GetEmployeeImageReponseDTO
                {
                    Id = img.Id,
                    EmployeeId = img.EmployeeId.ToString(),
                    FileName = img.FileName,
                    FilePath = img.FilePath,
                    IsActive = img.IsActive,
                    IsPrimary = img.IsPrimary,
                    HasImageUploaded = img.HasImageUploaded,
                    CompletionPercentage = completion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error fetching primary image | EmployeeId={Emp}",
                    dto.Prop.EmployeeId);

                // SAFE fallback
                return new GetEmployeeImageReponseDTO
                {
                    EmployeeId = dto.Prop.EmployeeId.ToString(),
                    CompletionPercentage = 0
                };
            }
        }


        public async Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(
      GetBaseEmployeeRequestDTO dto)
        {
            try
            {
                // 🧩 Step 1: Pagination defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // 🧩 Step 2: Base employee query (NO tracking = faster)
                var query = _context.Employees
                    .AsNoTracking()
                    .Where(x =>
                        x.TenantId == dto.Prop.TenantId &&
                        (x.IsSoftDeleted == null || x.IsSoftDeleted == false));

                // 🧩 Step 3: Dynamic filters (SAFE & SAME RESULT)
                if (dto.Prop.EmployeeId > 0)
                    query = query.Where(x => x.Id == dto.Prop.EmployeeId);

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

                if (dto.TypeId > 0)
                    query = query.Where(x => x.EmployeeTypeId == dto.TypeId);

                if (dto.HasPermanent.HasValue)
                    query = query.Where(x => x.HasPermanent == dto.HasPermanent);

                if (dto.IsEditAllowed.HasValue)
                    query = query.Where(x => x.IsEditAllowed == dto.IsEditAllowed);

                if (dto.IsInfoVerified.HasValue)
                    query = query.Where(x => x.IsInfoVerified == dto.IsInfoVerified);

                // 🧩 Step 4: Sorting
                bool isAsc = string.Equals(dto.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);
                query = dto.SortBy?.ToLower() switch
                {
                    "firstname" => isAsc ? query.OrderBy(x => x.FirstName) : query.OrderByDescending(x => x.FirstName),
                    "lastname" => isAsc ? query.OrderBy(x => x.LastName) : query.OrderByDescending(x => x.LastName),
                    "employementcode" => isAsc ? query.OrderBy(x => x.EmployementCode) : query.OrderByDescending(x => x.EmployementCode),
                    _ => isAsc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id)
                };

                // 🧩 Step 5: Total count (pagination metadata)
                int totalCount = await query.CountAsync();

                // 🧩 Step 6: Fetch paged employees ONLY (lightweight)
                var employees = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        Employee = x,
                        Country = x.Country
                    })
                    .ToListAsync();

                // ----------------------------------------------------
                // ⚡ PERFORMANCE MAGIC STARTS HERE
                // ----------------------------------------------------

                bool hasPrimaryImage = await _context.EmployeeImages
                        .AsNoTracking()
                         .AnyAsync(x =>
                         x.EmployeeId == dto.Prop.EmployeeId &&
                             x.IsPrimary &&
                              x.HasImageUploaded &&
                             x.IsSoftDeleted != true);


                // 🧩 Step 8: ONE-TIME lookup tables (Dictionary = O(1))
                var designationLookup = await _context.Designations
                    .ToDictionaryAsync(x => x.Id, x => x.DesignationName);

                var departmentLookup = await _context.Departments
                    .ToDictionaryAsync(x => x.Id, x => x.DepartmentName);

                var typeLookup = await _context.EmployeeTypes
                    .ToDictionaryAsync(x => x.Id, x => x.TypeName);

                var genderLookup = await _context.Genders
                    .ToDictionaryAsync(x => x.Id, x => x.GenderName);

                // 🧩 Step 9: Final DTO mapping + IN-MEMORY completion
                var records = employees.Select(x =>
                {
                 //   bool hasPrimary = hasPrimaryImageLookup.Contains(x.Employee.Id);

                    double completion =
                        CompletionCalculatorHelper.EmployeePropCalculate(
                            x.Employee,
                            hasPrimaryImage);

                    return new GetBaseEmployeeResponseDTO
                    {
                        Id = x.Employee.Id.ToString(),
                        EmployementCode = x.Employee.EmployementCode,
                        FirstName = x.Employee.FirstName,
                        MiddleName = x.Employee.MiddleName,
                        LastName = x.Employee.LastName,

                        GenderId = x.Employee.GenderId ?? 0,
                        GenderName = genderLookup.GetValueOrDefault(x.Employee.GenderId ?? 0),

                        DesignationId = x.Employee.DesignationId ?? 0,
                        DesignationName = designationLookup.GetValueOrDefault(x.Employee.DesignationId ?? 0),

                        DepartmentId = x.Employee.DepartmentId ?? 0,
                        DepartmentName = departmentLookup.GetValueOrDefault(x.Employee.DepartmentId ?? 0),

                        EmployeeTypeId = x.Employee.EmployeeTypeId ?? 0,
                        Type = typeLookup.GetValueOrDefault(x.Employee.EmployeeTypeId ?? 0),
                        DateOfBirth = x.Employee.DateOfBirth,
                        DateOfOnBoarding = x.Employee.DateOfOnBoarding,
                        CountryId = x.Country.Id,
                        CountryCode = x.Country.CountryCode,
                        Nationality = x.Country.CountryName,

                        OfficialEmail = x.Employee.OfficialEmail,
                        MobileNumber = x.Employee.MobileNumber,

                        EmergencyContactPerson = x.Employee.EmergencyContactPerson,
                        EmergencyContactNumber = x.Employee.EmergencyContactNumber,
                        BloodGroup = x.Employee.BloodGroup,
                        Relation = x.Employee.Relation,

                        HasPermanent = x.Employee.HasPermanent,
                        IsActive = x.Employee.IsActive,
                        IsEditAllowed = x.Employee.IsEditAllowed,
                        IsInfoVerified = x.Employee.IsInfoVerified,
                        IsMarried = x.Employee.IsMarried,


                        // ⭐ SINGLE SOURCE OF TRUTH
                        CompletionPercentage = completion
                    };
                }).ToList();

                // 🧩 Step 10: Overall completion (AVG, fast, accurate)
                double overallCompletion =
                    records.Any()
                        ? Math.Round(records.Average(x => x.CompletionPercentage), 2)
                        : 0;

                // 🧩 Step 11: Final response
                return new PagedResponseDTO<GetBaseEmployeeResponseDTO>
                {
                    Items = records,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    CompletionPercentage = overallCompletion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching base employee info.");
                throw;
            }
        }

        public async Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(GetAllEmployeeInfoRequestDTO dto)
        {
            try
            {
              
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;
                
                // ----------------------------------------------------
                // 1️⃣ BASE QUERY (LEFT JOIN + AS NO TRACKING)
                // ----------------------------------------------------
                var baseQuery =
                    from emp in _context.Employees.AsNoTracking()

                    join gender in _context.Genders
                        on emp.GenderId equals (long?)gender.Id into genderJoin
                    from g in genderJoin.DefaultIfEmpty()

                    join designation in _context.Designations
                        on emp.DesignationId equals (long?)designation.Id into desigJoin
                    from d in desigJoin.DefaultIfEmpty()

                    join empType in _context.EmployeeTypes
                        on emp.EmployeeTypeId equals (long?)empType.Id into typeJoin
                    from et in typeJoin.DefaultIfEmpty()

                    join department in _context.Departments
                        on emp.DepartmentId equals (long?)department.Id into deptJoin
                    from dep in deptJoin.DefaultIfEmpty()

                     join country in _context.Countries
                        on emp.CountryId equals (int)country.Id into countryJoin
                    from c in countryJoin.DefaultIfEmpty()

                    join contact in _context.EmployeeContacts
                            .Where(ec => ec.IsPrimary == true)
                           on emp.Id equals contact.EmployeeId into contactJoin
                    from cont in contactJoin.DefaultIfEmpty()

                    join district in _context.Districts
                    on cont.DistrictId equals district.Id into districtJoin
                    from dist in districtJoin.DefaultIfEmpty()

                    where emp.TenantId == dto.Prop.TenantId
                          && emp.IsSoftDeleted != true


                    select new
                    {
                        emp,
                        cont,
                        dist,
                        GenderName = g != null ? g.GenderName : "",
                        DesignationName = d != null ? d.DesignationName : "",
                        EmployeeTypeName = et != null ? et.TypeName : "",
                        DepartmentName = dep != null ? dep.DepartmentName : "",
                        CountryName = c != null ? c.CountryName : "",
                        CountryCode = c != null ? c.CountryCode : ""   // ✅ ADD THIS
                    };



                // ----------------------------------------------------
                // 2️⃣ FILTERS
                // ----------------------------------------------------
                if (dto.Prop.EmployeeId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.Id == dto.Prop.EmployeeId);

                if (!string.IsNullOrWhiteSpace(dto.EmailId))
                    baseQuery = baseQuery.Where(x => x.emp.OfficialEmail == dto.EmailId.Trim());

                if (dto.DepartmentId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.DepartmentId == dto.DepartmentId);

                if (dto.DesignationId > 0)
                    baseQuery = baseQuery.Where(x => x.emp.DesignationId == dto.DesignationId);

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
                // 3️⃣ PAGING (FIXED ORDERING)
                // ----------------------------------------------------
                var pagedEmployees = await baseQuery
                    .OrderByDescending(x => x.emp.AddedDateTime)
                    .ThenBy(x => x.emp.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // ----------------------------------------------------
                // 4️⃣ IMAGES
                // ----------------------------------------------------
                var ids = pagedEmployees.Select(x => x.emp.Id).ToList();

                var images = await _context.EmployeeImages
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
                // 5️⃣ BUILD RESPONSE
                // ----------------------------------------------------
                var result = pagedEmployees.Select(x =>
                {
                    imgLookup.TryGetValue(x.emp.Id, out var img);
                    bool hasPrimary = hasPrimaryLookup.Contains(x.emp.Id);

                    double completionPercentage = CompletionCalculatorHelper.EmployeePropCalculate(x.emp, hasPrimary);

                    SummaryEmployeeInfo summaryEmployeeInfo = new SummaryEmployeeInfo
                    {

                        EmergencyContactPerson = x.emp.EmergencyContactPerson,
                        EmergencyContactNumber = x.emp.EmergencyContactNumber,
                        BloodGroup = x.emp?.BloodGroup,
                        MobileNumber = x.emp?.MobileNumber,
                        Relation = x.emp?.Relation,
                        CountryCode = x.CountryCode,
                        IsActive = x.emp.IsActive,
                        IsMarried = x.emp?.IsMarried,
                        EmployeeCode= x.emp.EmployementCode,

                        OnlineStatus = null, // 🔴 Redis / SignalR se aayega (DB se nahi)
                        LastLoginDateTime = DateTime.UtcNow,

                        CurrentSalaryStatusId = 1,               // ❗ salary join nahi hai
                        CurrentSalaryStatusRemark = null,

                        RoleId = 1,                              // ❗ role join nahi hai
                        RoleType = null,

                        DesignationId = x.emp.DesignationId ?? 0,
                        Designation = x.DesignationName,

                        DepartmentId = x.emp.DepartmentId ?? 0,
                        Department = x.DepartmentName,
                       
                        ProfileImage = !string.IsNullOrWhiteSpace(img?.FilePath) ? img.FilePath : null,
                        City = x.dist?.DistrictName,   // ✅ District table se
                                                       // ✅ FIXED
                        Address = x.cont?.Address,

                        DateOfJoining = x.emp.DateOfOnBoarding,

                        EmployeeTypeId = x.emp.EmployeeTypeId ?? 0,
                        EmployeeTypeName = x.EmployeeTypeName
                    };

                   
                    return new GetAllEmployeeInfoResponseDTO
                    {
                        EmployeeId = x.emp.Id.ToString(),
                        EmployementCode = x.emp.EmployementCode,
                        FirstName = x.emp.FirstName,
                        LastName = x.emp.LastName,
                        DateOfOnBoarding = x.emp.DateOfOnBoarding?.ToString(),
                        DateOfBirth = x.emp.DateOfBirth?.ToString(),
                        CountryId = x.emp.CountryId,
                        Nationality = x.CountryName,   // ✅ FIXED (NO navigation access)
                        CountryCode = x.CountryCode,
                        EmergencyContactPerson = x.emp.EmergencyContactPerson,
                        GenderId = x.emp.GenderId ?? 0,
                        EmployeeTypeId = x.emp.EmployeeTypeId ?? 0,
                        DesignationId = x.emp.DesignationId ?? 0,
                        DepartmentId = x.emp.DepartmentId ?? 0,
                        GenderName = x.GenderName,
                        MobileNumber  = x.emp?.MobileNumber,                        
                        EmployeeTypeName = x.EmployeeTypeName,
                        DesignationName = x.DesignationName,
                        DepartmentName = x.DepartmentName,
                        OfficialEmail = x.emp.OfficialEmail,
                        EmployeeImagePath = img?.FilePath,
                        HasImagePicUploaded = hasPrimary,
                        IsActive = x.emp.IsActive,
                        SummaryEmployeeInfo = summaryEmployeeInfo  , // ✅ THIS WAS MISSING
                        CompletionPercentage = completionPercentage
                    };
                }).ToList();

                return new PagedResponseDTO<GetAllEmployeeInfoResponseDTO>
                {
                    Items = result,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    

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
     

     
        public async Task<List<CompletionSectionDTO>> GetEmployeeCompletionAsync(long employeeId)
        {
            try
            {
                // ❗If bad employeeId, return empty list safely
                if (employeeId <= 0)
                    return new List<CompletionSectionDTO>();
                #region Contact Details

                var contactList = await _context.EmployeeContacts
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                        x.IsSoftDeleted == false &&
                        x.IsActive == true
                    )
                    .Select(x => new GetContactResponseDTO
                    {
                        Id = x.Id,
                        ContactType = x.ContactType,
                        Relation = x.Relation,
                        ContactName = x.ContactName,
                        ContactNumber = x.ContactNumber,
                        AlternateNumber = x.AlternateNumber,
                     //   Email = x.Email,
                        Address = x.Address,
                        CountryId = x.CountryId,
                        StateId = x.StateId,
                        DistrictId = x.DistrictId,
                        IsPrimary = x.IsPrimary,
                        IsEditAllowed = x.IsEditAllowed,
                        IsInfoVerified = x.IsInfoVerified,
                      //  Description = x.Description
                    })
                    .ToListAsync();

                #endregion

                #region Education Details
                var eduList = await _context.EmployeeEducations
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                    .Select(x => new GetEducationResponseDTO
                    {
                        Degree = x.Degree,
                        InstituteName = x.InstituteName,
                        ScoreType = x.ScoreType.ToString(),
                        HasEducationDocUploded = x.HasEducationDocUploded,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,                       
                        IsEditAllowed =x.IsEditAllowed,
                        IsInfoVerified =x.IsInfoVerified,
                       

                    })
                    .ToListAsync();
                #endregion
                #region Bank Details
                var bankList = await _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted != true)
                    .Select(x => new GetBankResponseDTO
                    {
                        AccountNumber = x.AccountNumber,    
                        BankName = x.BankName,  
                        IFSCCode = x.IFSCCode,  
                        BranchName = x.BranchName,
                        AccountType = x.AccountType,
                        HasChequeDocUploaded = x.HasChequeDocUploaded,
                        IsPrimaryAccount = x.IsPrimaryAccount,
                        IsEditAllowed = x.IsEditAllowed,
                        IsInfoVerified = x.IsInfoVerified,
                        FileName = x.FileName,
                        FilePath = x.FilePath,
                        


                    })
                    .ToListAsync();
                #endregion
                // 🧮 Calculate completion %
                var educationSection = eduList.CalculateEducationCompletionDTO();
                 var bankSection = bankList.CalculateBankCompletionDTO();
                 var contactSection = contactList.CalculateContactCompletionDTO();

                return new List<CompletionSectionDTO> { educationSection, bankSection, contactSection };
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
                       join nc in _context.Countries
                      on emp.CountryId equals nc.Id into countryJoin
                     from nationCountry in countryJoin.DefaultIfEmpty()
                   
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
                        TenantId = emp.TenantId ?? 0,
                        FirstName = emp.FirstName,
                        MiddleName = emp.MiddleName,
                        LastName = emp.LastName,
                        EmployementCode = emp.EmployementCode,
                        EmployeeTypeId = emp.EmployeeTypeId ?? 0,
                        EmployeeTypeName = et.TypeName,
                         CountryId = nationCountry.Id,
                         Nationality =nationCountry.CountryName ,
                        DateOfOnBoarding = emp.DateOfOnBoarding,
                        DateOfBirth = emp.DateOfBirth,
                        EmergencyContactNumber = emp.EmergencyContactNumber,
                        EmergencyContactPerson = emp.EmergencyContactPerson,
                        MobileNumber = emp.MobileNumber,
                         CountryCode= emp.Country.CountryCode,
                        IsActive = emp.IsActive,
                        HasPermanent = emp.HasPermanent,
                        GenderId = emp.GenderId ?? 0,
                        GenderName = gt.GenderName,
                        DepartmentId = emp.DepartmentId ?? 0,
                        DepartmentName = dept.DepartmentName,  // ✅ added
                        DesignationId = emp.DesignationId ?? 0,
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

        public async Task<EmployeeImage?> IsImageExist(long id, bool isActive)
        {
            try
            {
               
                return await _context.EmployeeImages
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
                
                _context.EmployeeImages.Update(employeeImageInfo);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error while updating employee image (Id: {employeeImageInfo?.Id})");
                return false;
            }
        }


       
      public async Task<GetBaseEmployeeResponseDTO> CreateEmployeeAsync( Employee employee, LoginCredential loginCredential, UserRole userRole)
{
    // 1️⃣ Save entities
    await _context.Employees.AddAsync(employee);

    var employeeImage = new EmployeeImage
    {
        Employee = employee,
        TenantId = employee.TenantId,
        IsPrimary = true,
        HasImageUploaded = false,
        IsActive = true,
        AddedById = employee.AddedById,
        AddedDateTime = DateTime.UtcNow,
        FileType = 1
    };
          
     await _context.EmployeeImages.AddAsync(employeeImage);
    await _context.LoginCredentials.AddAsync(loginCredential);
    await _context.UserRoles.AddAsync(userRole);

    await _context.SaveChangesAsync();

    // 2️⃣ Fetch with joins (Single source of truth)
    var result =
        await (from emp in _context.Employees.AsNoTracking()
               where emp.Id == employee.Id

               join d in _context.Designations
                   on emp.DesignationId equals d.Id into desigJoin
               from desig in desigJoin.DefaultIfEmpty()

               join g in _context.Genders
                   on emp.GenderId equals g.Id into genderJoin
               from gender in genderJoin.DefaultIfEmpty()

               join dept in _context.Departments
                   on emp.DepartmentId equals dept.Id into deptJoin
               from department in deptJoin.DefaultIfEmpty()

               join ur in _context.UserRoles
                   on emp.Id equals ur.EmployeeId

                join nc in _context.Countries
                 on emp.CountryId equals nc.Id into countryJoin
                from nationCountry in countryJoin.DefaultIfEmpty()


               join r in _context.Roles
                   on ur.RoleId equals r.Id

               join t in _context.EmployeeTypes
                   on emp.EmployeeTypeId equals t.Id into empTypeJoin

               from empType in empTypeJoin.DefaultIfEmpty()


               select new GetBaseEmployeeResponseDTO
               {
                   Id = emp.Id.ToString(),
                   EmployementCode = emp.EmployementCode,
                   FirstName = emp.FirstName,
                   MiddleName = emp.MiddleName,
                   LastName = emp.LastName,
                   OfficialEmail = emp.OfficialEmail,

                   GenderId = emp.GenderId ?? 0,
                   GenderName = gender != null ? gender.GenderName : null,

                   DesignationId = emp.DesignationId ?? 0,
                   DesignationName = desig != null ? desig.DesignationName : null,

                   DepartmentId = emp.DepartmentId ?? 0,
                   DepartmentName = department != null ? department.DepartmentName : null,

                   RoleId    = r.Id,

                   RoleType = r.RoleType,
                   RoleName = r.RoleName,
                    CountryId= emp.CountryId,
                    MobileNumber= emp.MobileNumber,
                   CountryCode = nationCountry != null ? nationCountry.CountryCode : null,
                   Nationality = nationCountry != null ? nationCountry.CountryName : null,
                   EmergencyContactPerson = emp.EmergencyContactPerson,
                   EmergencyContactNumber = emp.EmergencyContactNumber,
                   EmployeeTypeId = emp.EmployeeTypeId ?? 0,
                   Type = empType != null ? empType.TypeName : null,
                   DateOfBirth = emp.DateOfBirth,
                   DateOfOnBoarding = emp.DateOfOnBoarding,
                   DateOfExit = emp.DateOfExit,
                   BloodGroup = emp.BloodGroup,
                   HasPermanent = emp.HasPermanent,
                   IsActive = emp.IsActive,
                   IsEditAllowed = emp.IsEditAllowed,
                   IsInfoVerified = emp.IsInfoVerified,
               }).FirstOrDefaultAsync();
            if (result != null)
            {
                bool hasPrimaryImage = await _context.EmployeeImages
                      .AsNoTracking()
                      .AnyAsync(x =>
                          x.EmployeeId == employee.Id &&      // ✅ FIX
                          x.IsPrimary == true &&
                          x.HasImageUploaded == true &&
                          x.IsSoftDeleted != true);

                // =====================================================
                // 4️⃣ COMPLETION % (SINGLE SOURCE OF TRUTH)
                // =====================================================
                 result.CompletionPercentage =    CompletionCalculatorHelper.EmployeePropCalculate(employee, hasPrimaryImage);

                         
            }


            return result!;
}




    
        public Task<Employee?> IsEmployeeExist(string EmployeeCode, long tenantId, bool track = true)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateVerificationStatusByEntity(int TabInfoType, long EmployeeId, long UserId, bool Status, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }


}   




        #endregion



  




 
 






