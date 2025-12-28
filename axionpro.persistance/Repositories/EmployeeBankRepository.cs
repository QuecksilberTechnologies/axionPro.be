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
using System.Drawing.Printing;

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

                // 🔥 SAFE + FAST (pageSize = 10)
                foreach (var item in records)
                {
                    item.CompletionPercentage =
                        CompletionCalculatorHelper.BankPropCalculate(item);
                }


                // -----------------------------
                // 6️⃣ Section-level Completion
                // -----------------------------

                // 🔹 Average (UI progress)
                double completionPercentage = records.Any()
                    ? Math.Round(records.Average(x => x.CompletionPercentage), 0)
                    : 0;

                // 🔹 Primary document rule
                bool hasUploadedAllDocs = false;

                var primaryBank = records.FirstOrDefault(x => x.IsPrimaryAccount == true);
                if (primaryBank != null)
                {
                    hasUploadedAllDocs = primaryBank.HasChequeDocUploaded;
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
                    CompletionPercentage = completionPercentage,
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

        public async Task<PagedResponseDTO<GetBankResponseDTO>> AddCreatedAsync(EmployeeBankDetail entity)
        {
            try
            {
                // ✅ 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Bank info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // ✅ 2️⃣ Record insert karo
                await _context.EmployeeBankDetails.AddAsync(entity);
                await _context.SaveChangesAsync();

                // ✅ 3️⃣ Fetch updated list (latest record ke sath)
                var query = _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id);

                var totalRecords = await query.CountAsync();

                // ✅ 4️⃣ Fetch paginated data (default 1 page only since just added)
                var records = await query
                    .Take(10)
                    .ToListAsync();

                // ✅ 5️⃣ Map to DTOs
                var responseData = _mapper.Map<List<GetBankResponseDTO>>(records);

                // ✅ 6️⃣ Prepare PagedResponse
                return new PagedResponseDTO<GetBankResponseDTO>
                {
                    Items = responseData,
                    TotalCount = totalRecords,
                    PageNumber = 1,
                    PageSize = 10,
                     
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while adding/fetching bank info for EmployeeId: {EmployeeId}", entity.EmployeeId);
                throw new Exception($"Failed to add or fetch bank info: {ex.Message}");
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
                // -----------------------------
                // 1️⃣ Pagination & Sorting
                // -----------------------------
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                string sortBy = dto.SortBy?.ToLower() ?? "id";
                bool isDescending = (dto.SortOrder?.ToLower() ?? "desc") == "desc";

                // -----------------------------
                // 2️⃣ Base Query
                // -----------------------------
                IQueryable<EmployeeBankDetail> query = _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        (x.IsSoftDeleted == false || x.IsSoftDeleted == null)
                    );

                // -----------------------------
                // 3️⃣ Filters
                // -----------------------------
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

                // -----------------------------
                // 4️⃣ Sorting
                // -----------------------------
                query = sortBy switch
                {
                    "bankname" => isDescending ? query.OrderByDescending(x => x.BankName) : query.OrderBy(x => x.BankName),
                    "accountnumber" => isDescending ? query.OrderByDescending(x => x.AccountNumber) : query.OrderBy(x => x.AccountNumber),
                    "branchname" => isDescending ? query.OrderByDescending(x => x.BranchName) : query.OrderBy(x => x.BranchName),
                    "accounttype" => isDescending ? query.OrderByDescending(x => x.AccountType) : query.OrderBy(x => x.AccountType),
                    "haschequedocuploaded" => isDescending ? query.OrderByDescending(x => x.HasChequeDocUploaded) : query.OrderBy(x => x.HasChequeDocUploaded),
                    _ => isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
                };

                // -----------------------------
                // 5️⃣ Count
                // -----------------------------
                int totalCount = await query.CountAsync();

                // -----------------------------
                // 6️⃣ Pagination + Mapping
                // -----------------------------

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
        FilePath = x.FilePath
    })
    .ToListAsync();

                // CPU-only, safe, readable
                foreach (var item in records)
                {
                    item.CompletionPercentage =
                        CompletionCalculatorHelper.BankPropCalculate(item);
                }



                // -----------------------------

                // 🔹 1. AVERAGE COMPLETION (ALWAYS)
                double completionPercentage = records.Any()
                    ? Math.Round(records.Average(x => x.CompletionPercentage), 2)
                    : 0;

                // 🔹 2. PRIMARY DOCUMENT RULE
                bool hasUploadedAllDocs = false;

                var primaryBank = records.FirstOrDefault(x => x.IsPrimaryAccount == true);

                if (primaryBank != null)
                {
                    hasUploadedAllDocs = primaryBank.HasChequeDocUploaded;
                }
                else
                {
                    // primary hi nahi hai = rule fail
                    hasUploadedAllDocs = false;
                }


                // -----------------------------
                // 9️⃣ Final Response
                // -----------------------------
                return new PagedResponseDTO<GetBankResponseDTO>
                {
                    Items = records,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),

                    // 🔥 IMPORTANT (HRMS RULE)
                    CompletionPercentage = completionPercentage,
                    HasUploadedAll = hasUploadedAllDocs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching bank info for EmployeeId: {EmployeeId}", dto.Prop.EmployeeId);
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


        public async Task<GetBankResponseDTO> GetSingleRecordAsync(int id, bool isActive)
        {
            try
            {
              //  await using var _context = await _contextFactory.CreateDbContextAsync();

                // 🔹 Record fetch karna with filters
                var entity = await _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(b => b.Id == id && b.IsActive == isActive && (b.IsSoftDeleted !=true))
                    .Select(b => new GetBankResponseDTO
                    {
                        Id = b.Id??0,
                        EmployeeId = b.EmployeeId.ToString(),
                        BankName = b.BankName,
                        AccountNumber = b.AccountNumber,
                        IFSCCode = b.IFSCCode,
                        BranchName = b.BranchName,
                        AccountType = b.AccountType,                       
                        IsPrimaryAccount = b.IsPrimaryAccount,
                        IsActive = b.IsActive,
                        HasChequeDocUploaded = b.HasChequeDocUploaded,
                        FilePath = b.FilePath,
                        UPIId = b.UPIId,
                        IsInfoVerified = b.IsInfoVerified,
                        IsEditAllowed = b.IsEditAllowed,
                        FileName = b.FileName,
                        FileType = b.FileType



                    })
                    .FirstOrDefaultAsync();

                // 🔹 Null check handling
                if (entity == null)
                {
                    _logger.LogWarning($"⚠️ No active bank info found for ID {id} with IsActive = {isActive}");
                    return null!;
                }

                _logger.LogInformation($"✅ Bank info record fetched successfully for ID {id}");
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error while fetching single bank record for ID {id}");
                throw new Exception($"Failed to fetch bank record: {ex.Message}", ex);
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
