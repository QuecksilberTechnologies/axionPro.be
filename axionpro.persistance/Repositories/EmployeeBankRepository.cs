using AutoMapper;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
 
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using static QRCoder.PayloadGenerator;

namespace axionpro.persistance.Repositories
{
    public class EmployeeBankRepository : IEmployeeBankRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeBankRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeBankRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeBankRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }
        public async Task<PagedResponseDTO<GetBankResponseDTO>> CreateAsync(EmployeeBankDetail entity)
        {
            try
            {
                // -----------------------------
                // 1️⃣ Insert Record
                // -----------------------------
              bool isSet =await  ResetPrimaryAccountAsync(entity.EmployeeId , entity.AddedById);
                await _context.EmployeeBankDetails.AddAsync(entity);
                await _context.SaveChangesAsync();

                // -----------------------------
                // 2️⃣ Pagination defaults
                // -----------------------------
                int pageNumber = 1;
                int pageSize = 10;

                // -----------------------------
                // 3️⃣ Base Query (latest first)
                // -----------------------------
                var baseQuery = _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == entity.EmployeeId &&
                        (x.IsSoftDeleted !=true)
                    )
                    .OrderByDescending(x => x.Id);

                int totalRecords = await baseQuery.CountAsync();

                // -----------------------------
                // 4️⃣ Pagination + Projection
                // -----------------------------
                var records = await baseQuery
                   .Skip((pageNumber - 1) * pageSize)
                      .Take(pageSize)
                       .Select(bank => new GetBankResponseDTO
      {
          Id = bank.Id ?? 0,
          EmployeeId = bank.EmployeeId.ToString(),
          BankName = bank.BankName,
          AccountNumber = bank.AccountNumber,
          AccountType = bank.AccountType,
          IFSCCode = bank.IFSCCode,
          BranchName = bank.BranchName,
          IsPrimaryAccount = bank.IsPrimaryAccount,
          IsInfoVerified = bank.IsInfoVerified,
          IsEditAllowed = bank.IsEditAllowed,
          IsActive = bank.IsActive,
          HasChequeDocUploaded = bank.HasChequeDocUploaded,
          FilePath = bank.FilePath
            })
      .ToListAsync();


                // =========================================================
                // 7️⃣ Detect PRIMARY existence (LIST LEVEL)
                // =========================================================
                bool hasAtLeastOnePrimary = records.Any(x => x.IsPrimaryAccount);

                // =========================================================
                // 8️⃣ Per-record Completion (CORRECT LOGIC)
                // =========================================================
                foreach (var item in records)
                {
                    item.CompletionPercentage = hasAtLeastOnePrimary
                        ? CompletionCalculatorHelper.BankPropCalculate(item)
                        : CompletionCalculatorHelper.BankPropCalculate_NoPrimary(item);
                }

                // =========================================================
                // 9️⃣ Section-level Mandatory Rules
                // =========================================================
                bool hasUploadedAllDocs = hasAtLeastOnePrimary &&
                    records
                        .Where(x => x.IsPrimaryAccount)
                        .All(x => x.HasChequeDocUploaded);

                // =========================================================
                // 🔹 UI Average Completion (ALWAYS show average)
                // =========================================================
                double uiAverageCompletion = records.Any()
                    ? Math.Round(records.Average(x => x.CompletionPercentage), 0)
                    : 0;

                // =========================================================
                // 🔹 Final Completion (Business Rule Applied)
                // =========================================================
                double finalCompletionPercentage = uiAverageCompletion;

                // 🔥 HARD BUSINESS RULE
                // No primary ⇒ section invalid
                if (!hasAtLeastOnePrimary)
                {
                    finalCompletionPercentage = 0;
                }


                // -----------------------------
                // 7️⃣ Final Response
                // -----------------------------
                return new PagedResponseDTO<GetBankResponseDTO>
                {
                    Items = records,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),

                    // 🔥 IMPORTANT (same as GetInfo)
                    CompletionPercentage = uiAverageCompletion,
                    HasUploadedAll = hasUploadedAllDocs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error occurred while creating bank record for EmployeeId: {EmployeeId}",
                    entity.EmployeeId
                );
                throw new Exception($"Failed to create bank info: {ex.Message}");
            }
        }
        public async Task<bool> UpdateVerificationStatus(
     long employeeId,
     long userId,
     bool status)
        {
            int affectedRows = await _context.EmployeeBankDetails
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.IsSoftDeleted != true)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsInfoVerified, status)
                    .SetProperty(x => x.InfoVerifiedById, userId)
                    .SetProperty(x => x.InfoVerifiedDateTime, DateTime.UtcNow)
                );

            return affectedRows > 0;
        }
        /*

         */

        public async Task<bool> UpdateEditStatus(
    long employeeId,
    long userId,
    bool status)
        {
            var affectedRows = await _context.EmployeeBankDetails
                   .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.IsSoftDeleted != true)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsEditAllowed, status)
                    .SetProperty(x => x.UpdatedById, userId)
                    .SetProperty(x => x.InfoVerifiedDateTime, DateTime.UtcNow)
                );

            return affectedRows > 0;
        }


      
        public async Task<bool> DeleteAsync(EmployeeBankDetail employeeBankDetail)
        {
            try
            {
                if (employeeBankDetail == null)
                {
                    _logger.LogWarning("⚠️ Delete failed: EmployeeBankDetail entity is null");
                    return false;
                }

                // Entity already tracked hai (GetSingleRecordAsync(track=true) se aaya)
                _context.EmployeeBankDetails.Update(employeeBankDetail);

                int affectedRows = await _context.SaveChangesAsync();

                if (affectedRows > 0)
                {
                    _logger.LogInformation(
                        "✅ Bank record soft-deleted successfully | BankId: {Id}",
                        employeeBankDetail.Id);

                    return true;
                }

                _logger.LogWarning(
                    "⚠️ No rows affected while deleting bank record | BankId: {Id}",
                    employeeBankDetail.Id);

                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Concurrency issue while deleting bank record | BankId: {Id}",
                    employeeBankDetail.Id);

                throw new Exception(
                    "Record could not be deleted due to concurrent update. Please retry.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Database error while deleting bank record | BankId: {Id}",
                    employeeBankDetail.Id);

                throw new Exception(
                    "Database error occurred while deleting bank record.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Unexpected error while deleting bank record | BankId: {Id}",
                    employeeBankDetail.Id);

                throw;
            }
        }

        public async Task<bool> ResetPrimaryAccountAsync(long employeeId,  long byUserId)
        {
            try
            {
                var primaries = await _context.EmployeeBankDetails
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                        x.IsPrimaryAccount == true &&
                        x.IsSoftDeleted != true)
                    .ToListAsync();

                if (!primaries.Any())
                    return true; // nothing to reset

                foreach (var item in primaries)
                {
                    item.IsPrimaryAccount = false;
                    item.UpdatedDateTime = DateTime.UtcNow;
                      item.UpdatedById = byUserId; // if available
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // optional logger
                _logger?.LogError(ex,
                    "❌ Failed to reset primary bank accounts for EmployeeId: {EmployeeId}",
                    employeeId);

                return false;
            }
        }

        public async Task<PagedResponseDTO<GetBankResponseDTO>> GetInfoAsync(GetBankReqestDTO dto)
        {
            try
            {
                // =========================================================
                // 1️⃣ Pagination & Sorting
                // =========================================================
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                string sortBy = dto.SortBy?.ToLower() ?? "id";
                bool isDescending = (dto.SortOrder?.ToLower() ?? "desc") == "desc";

                // =========================================================
                // 2️⃣ Base Query
                // =========================================================
                IQueryable<EmployeeBankDetail> query = _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        (x.IsSoftDeleted == false || x.IsSoftDeleted == null)
                    );

                // =========================================================
                // 3️⃣ Filters
                // =========================================================
                if (dto.Id.HasValue && dto.Id.Value > 0)
                    query = query.Where(x => x.Id == dto.Id.Value);

                if (dto.IsActive)
                    query = query.Where(x => x.IsActive == dto.IsActive);

                if (dto.HasChequeDocUploaded)
                    query = query.Where(x => x.HasChequeDocUploaded == dto.HasChequeDocUploaded);

                if (dto.IsPrimaryAccount.HasValue)
                    query = query.Where(x => x.IsPrimaryAccount == dto.IsPrimaryAccount.Value);

                if (dto.IsInfoVerified.HasValue)
                    query = query.Where(x => x.IsInfoVerified == dto.IsInfoVerified.Value);

                if (dto.IsEditAllowed.HasValue)
                    query = query.Where(x => x.IsEditAllowed == dto.IsEditAllowed.Value);

                if (!string.IsNullOrWhiteSpace(dto.AccountType))
                    query = query.Where(x => x.AccountType.Contains(dto.AccountType));

                // =========================================================
                // 4️⃣ Sorting
                // =========================================================
                query = sortBy switch
                {
                    "bankname" => isDescending ? query.OrderByDescending(x => x.BankName) : query.OrderBy(x => x.BankName),
                    "accountnumber" => isDescending ? query.OrderByDescending(x => x.AccountNumber) : query.OrderBy(x => x.AccountNumber),
                    "branchname" => isDescending ? query.OrderByDescending(x => x.BranchName) : query.OrderBy(x => x.BranchName),
                    "accounttype" => isDescending ? query.OrderByDescending(x => x.AccountType) : query.OrderBy(x => x.AccountType),
                    "haschequedocuploaded" => isDescending ? query.OrderByDescending(x => x.HasChequeDocUploaded) : query.OrderBy(x => x.HasChequeDocUploaded),
                    _ => isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
                };

                // =========================================================
                // 5️⃣ Count
                // =========================================================
                int totalCount = await query.CountAsync();

                // =========================================================
                // 6️⃣ Pagination + Projection
                // =========================================================
                var records = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetBankResponseDTO
                    {
                        Id = x.Id ?? 0,
                        EmployeeId = x.EmployeeId.ToString(),
                        BankName = x.BankName,
                        AccountNumber = x.AccountNumber,
                        IFSCCode = x.IFSCCode,
                        BranchName = x.BranchName,
                        AccountType = x.AccountType,
                        IsPrimaryAccount = x.IsPrimaryAccount,
                        IsActive = x.IsActive,
                        IsInfoVerified = x.IsInfoVerified,
                        IsEditAllowed = x.IsEditAllowed,
                        HasChequeDocUploaded = x.HasChequeDocUploaded,
                        FilePath = x.FilePath,
                        UPIId = x.UPIId
                    })
                    .ToListAsync();

                // =========================================================
                // 7️⃣ Detect PRIMARY existence (LIST LEVEL)
                // =========================================================
                bool hasAtLeastOnePrimary = records.Any(x => x.IsPrimaryAccount);

                // =========================================================
                // 8️⃣ Per-record Completion (CORRECT LOGIC)
                // =========================================================
                foreach (var item in records)
                {
                    item.CompletionPercentage = hasAtLeastOnePrimary
                        ? CompletionCalculatorHelper.BankPropCalculate(item)
                        : CompletionCalculatorHelper.BankPropCalculate_NoPrimary(item);
                }

                // =========================================================
                // 9️⃣ Section-level Mandatory Rules
                // =========================================================
                bool hasUploadedAllDocs = hasAtLeastOnePrimary &&
                    records
                        .Where(x => x.IsPrimaryAccount)
                        .All(x => x.HasChequeDocUploaded);
 
                // =========================================================
                // 🔹 UI Average Completion (ALWAYS show average)
                // =========================================================
                double uiAverageCompletion = records.Any()
                    ? Math.Round(records.Average(x => x.CompletionPercentage), 0)
                    : 0;

                // =========================================================
                // 🔹 Final Completion (Business Rule Applied)
                // =========================================================
                double finalCompletionPercentage = uiAverageCompletion;

                // 🔥 HARD BUSINESS RULE
                // No primary ⇒ section invalid
                if (!hasAtLeastOnePrimary)
                {
                    finalCompletionPercentage = 0;
                }



                // =========================================================
                // 1️⃣1️⃣ Final Response
                // =========================================================
                return new PagedResponseDTO<GetBankResponseDTO>
                {
                    Items = records,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),

                    // 👇 UI ko yeh dikhao (progress bar)
                    CompletionPercentage = uiAverageCompletion,             

                    HasUploadedAll = hasUploadedAllDocs
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error fetching bank info for EmployeeId: {EmployeeId}",
                    dto.Prop.EmployeeId
                );

                throw new Exception($"Failed to fetch bank information: {ex.Message}");
            }
        }



        public async Task<bool> UpdateAsync(UpdateBankReqestDTO dto)
        {
            // -----------------------------
            // 1️⃣ Basic validation
            // -----------------------------
            if (dto == null || dto.Id <= 0)
                throw new ArgumentException("Invalid bank request.");

            // -----------------------------
            // 2️⃣ Fetch existing record
            // -----------------------------
            var existingBank = await _context.EmployeeBankDetails
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.Id &&
                    (x.IsSoftDeleted == null || x.IsSoftDeleted == false));

            if (existingBank == null)
                throw new InvalidOperationException("Employee bank record not found.");

            // -----------------------------
            // 3️⃣ Update ONLY provided fields
            // -----------------------------
            if (!string.IsNullOrWhiteSpace(dto.BankName))
                existingBank.BankName = dto.BankName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.AccountNumber))
                existingBank.AccountNumber = dto.AccountNumber.Trim();

            if (!string.IsNullOrWhiteSpace(dto.IFSCCode))
                existingBank.IFSCCode = dto.IFSCCode.Trim();

            if (!string.IsNullOrWhiteSpace(dto.BranchName))
                existingBank.BranchName = dto.BranchName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.AccountType))
                existingBank.AccountType = dto.AccountType.Trim();

            if (!string.IsNullOrWhiteSpace(dto.UPIId))
                existingBank.UPIId = dto.UPIId.Trim();

            if (!string.IsNullOrWhiteSpace(dto.FileName))
                existingBank.FileName = dto.FileName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.FilePath))
            {
                existingBank.FilePath = dto.FilePath.Trim();
                existingBank.HasChequeDocUploaded = true;
                existingBank.FileType = 1;
            }
                        // ----------------------------
            // 4️⃣ Primary & Cheque flags
            // -----------------------------
            if (dto.IsPrimaryAccount)
            {
                // 🔹 Step 1: Reset all existing primary accounts of this employee
                var existingPrimaries = await _context.EmployeeBankDetails
                    .Where(x =>
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        (x.IsSoftDeleted  !=true) &&
                        x.IsPrimaryAccount == true)
                    .ToListAsync();

                foreach (var item in existingPrimaries)
                {
                    item.IsPrimaryAccount = false;
                }

                // 🔹 Step 2: Mark current record as primary
                existingBank.IsPrimaryAccount = true;
            }
            else
                existingBank.IsPrimaryAccount = dto.IsPrimaryAccount;


            // CancelledChequeFile aaya hai matlab document uploaded
            if (dto.CancelledChequeFile != null && dto.CancelledChequeFile.Length > 0)
                existingBank.HasChequeDocUploaded = true;

            // -----------------------------
            // 5️⃣ Audit fields
            // -----------------------------
            existingBank.UpdatedById = dto.Prop.UserEmployeeId;
            existingBank.UpdatedDateTime = DateTime.UtcNow;

            // -----------------------------
            // 6️⃣ Save
            // -----------------------------
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<EmployeeBankDetail?> GetSingleRecordAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _context.EmployeeBankDetails
                    .Where(b =>
                        b.Id == id &&
                        b.IsActive == isActive &&
                        b.IsSoftDeleted != true)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    _logger.LogWarning(
                        "⚠️ No active bank info found | Id: {Id}, IsActive: {IsActive}",
                        id, isActive);

                    return null;
                }

                _logger.LogInformation(
                    "✅ Bank info record fetched successfully | Id: {Id}",
                    id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching single bank record | Id: {Id}",
                    id);

                throw;
            }
        }

        public async  Task<CompletionSectionDTO> GetBankCompletionPercentageAsync(long employeeId)
        {
            try
            {
                var record = await _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && x.IsSoftDeleted !=true)
                    .OrderByDescending(x => x.IsPrimaryAccount)
                    .Select(x => new
                    {
                        x.BankName,
                        x.AccountNumber,
                        x.IFSCCode,
                        x.BranchName,
                        x.AccountType,
                        x.IsPrimaryAccount,
                        x.HasChequeDocUploaded,
                        x.IsInfoVerified,
                        x.IsEditAllowed
                    })
                    .FirstOrDefaultAsync();

                if (record == null)
                {
                    return new CompletionSectionDTO
                    {
                        SectionName = "Bank",
                        CompletionPercent = 0,                       
                        IsInfoVerified = false,
                        IsEditAllowed = true
                    };
                }

              //  double completion = CalculateBankPercentage(record);

                return new CompletionSectionDTO
                {
                    SectionName = "Bank",
                  //  CompletionPercent = completion,                   
                    IsInfoVerified = record.IsInfoVerified,
                    IsEditAllowed = record.IsEditAllowed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating bank completion");
                return new CompletionSectionDTO
                {
                    SectionName = "Bank",
                    CompletionPercent = 0,
                   
                    IsInfoVerified = false,
                    IsEditAllowed = true
                };
            }
        }


       


       


      
    }
}
