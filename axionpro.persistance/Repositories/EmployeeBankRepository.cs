using AutoMapper;
using axionpro.application.DTOS.Employee.Bank;
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
                // 🔹 Step 1: Record insert
                await _context.EmployeeBankDetails.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 🔹 Step 2: Default pagination setup
                int pageNumber = 1;
                int pageSize = 10;

                // 🔹 Step 3: Fetch latest records (orderby Id desc)
                var baseQuery = _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId)
                    .OrderByDescending(x => x.Id);

                var totalRecords = await baseQuery.CountAsync();

                // 🔹 Step 4: Pagination logic
                var pagedData = await baseQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(bank => new GetBankResponseDTO
                    {
                        Id = bank.Id.ToString(),
                        EmployeeId = bank.EmployeeId.ToString(),
                        BankName = bank.BankName,
                        AccountNumber = bank.AccountNumber,
                        AccountType = bank.AccountType,
                        IsPrimaryAccount = bank.IsPrimaryAccount,
                        IsInfoVerified = bank.IsInfoVerified,
                        IFSCCode = bank.IFSCCode,
                        BranchName = bank.BranchName,
                        IsPrimary = bank.IsPrimaryAccount,
                        IsEditAllowed = bank.IsEditAllowed,
                        IsActive = bank.IsActive,
                        HasChequeDocUploaded = bank.HasChequeDocUploaded,
                        FilePath = bank.FilePath,
                      
                    })
                    .ToListAsync();

                // 🔹 Step 5: Final Response DTO
                return new PagedResponseDTO<GetBankResponseDTO>
                {
                    Items = pagedData ?? new List<GetBankResponseDTO>(),
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating bank record for EmployeeId: {EmployeeId}", entity.EmployeeId);
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

        public async Task<PagedResponseDTO<GetBankResponseDTO>> GetInfoAsync(GetBankReqestDTO dto, int id, long employeeId)
        {
            try
            {
                // ✅ Default pagination setup
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;
                string sortBy = dto.SortBy?.ToLower() ?? "id";
                string sortOrder = dto.SortOrder?.ToLower() ?? "desc";
                bool isDescending = sortOrder == "desc";

                // ✅ Base query - fetch all bank details of the employee which are not soft deleted
                IQueryable<EmployeeBankDetail> query = _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && (x.IsSoftDeleted == false || x.IsSoftDeleted == null));

                // ✅ Dynamic Filters
                if (id > 0)
                    query = query.Where(x => x.Id == id);

                if (dto.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);

                if (dto.HasChequeDocUploaded == true)
                    query = query.Where(x => x.HasChequeDocUploaded == dto.HasChequeDocUploaded);

                if (dto.IsPrimaryAccount.HasValue)
                    query = query.Where(x => x.IsPrimaryAccount == dto.IsPrimaryAccount.Value);

                if (dto.IsInfoVerified.HasValue)
                    query = query.Where(x => x.IsInfoVerified == dto.IsInfoVerified.Value);

                if (dto.IsEditAllowed.HasValue)
                    query = query.Where(x => x.IsEditAllowed == dto.IsEditAllowed.Value);

                if (!string.IsNullOrEmpty(dto.AccountType))
                    query = query.Where(x => x.AccountType.ToLower().Contains(dto.AccountType.ToLower()));

                // ✅ Dynamic Sorting
                query = sortBy switch
                {
                    "bankname" => isDescending ? query.OrderByDescending(x => x.BankName) : query.OrderBy(x => x.BankName),
                    "accountnumber" => isDescending ? query.OrderByDescending(x => x.AccountNumber) : query.OrderBy(x => x.AccountNumber),
                    "branchname" => isDescending ? query.OrderByDescending(x => x.BranchName) : query.OrderBy(x => x.BranchName),
                    "accounttype" => isDescending ? query.OrderByDescending(x => x.AccountType) : query.OrderBy(x => x.AccountType),
                    "haschequedocuploaded" => isDescending ? query.OrderByDescending(x => x.HasChequeDocUploaded) : query.OrderBy(x => x.HasChequeDocUploaded),
                    _ => isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
                };

                // ✅ Total record count before pagination
                int totalCount = await query.CountAsync();

                // 📄 Apply pagination + projection + conditional completion %
                // 📄 Apply pagination + projection + conditional completion %
                var records = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetBankResponseDTO
                    {
                        Id = x.Id.ToString(),
                        EmployeeId = x.EmployeeId.ToString(),
                        BankName = x.BankName,
                        AccountNumber = x.AccountNumber,
                        IFSCCode = x.IFSCCode,
                        IsPrimary = x.IsPrimaryAccount,
                        UPIId = x.UPIId,
                        BranchName = x.BranchName,
                        AccountType = x.AccountType,
                        IsPrimaryAccount = x.IsPrimaryAccount,
                        IsActive = x.IsActive,
                        IsInfoVerified = x.IsInfoVerified,
                        IsEditAllowed = x.IsEditAllowed,
                        HasChequeDocUploaded = x.HasChequeDocUploaded,
                        FilePath = x.FilePath,

                        // ✅ Completion Percentage Calculation:
                        // Primary account → total 7 checks (including cheque upload)
                        // Non-primary account → total 6 checks (excluding cheque upload)
                        CompletionPercentage = Math.Round(
                            (
                                x.IsPrimaryAccount
                                ? (new[]
                                {
                    // --- Fields considered for primary account completeness ---
                    string.IsNullOrEmpty(x.BankName) ? 0 : 1,
                    string.IsNullOrEmpty(x.AccountNumber) ? 0 : 1,
                    string.IsNullOrEmpty(x.IFSCCode) ? 0 : 1,
                    string.IsNullOrEmpty(x.BranchName) ? 0 : 1,
                    string.IsNullOrEmpty(x.AccountType) ? 0 : 1,
                    x.HasChequeDocUploaded ? 1 : 0, /* ✅ Only for primary */      1 // ✅ Itself is primary
                                }).Sum() / 7.0
                                : (new[]
                                {
                    // --- Fields considered for non-primary account completeness ---
                    string.IsNullOrEmpty(x.BankName) ? 0 : 1,
                    string.IsNullOrEmpty(x.AccountNumber) ? 0 : 1,
                    string.IsNullOrEmpty(x.IFSCCode) ? 0 : 1,
                    string.IsNullOrEmpty(x.BranchName) ? 0 : 1,
                    string.IsNullOrEmpty(x.AccountType) ? 0 : 1,
                    1 // ✅ Account is valid but non-primary → exclude cheque
                                }).Sum() / 6.0
                            ) * 100, 0)
                    })
                    .ToListAsync();

                // --- post-processing section ---

                // ✅ Calculate overall average percentage (nullable if no record)
                double? averagePercentage = records.Any()
                    ? records.Average(r => r.CompletionPercentage)
                    : (double?)null;

                // ✅ Determine if all required cheque docs are uploaded for primary accounts only
                bool? hasUploadedAllDocs;
                var primaryRecords = records.Where(r => r.IsPrimaryAccount == true).ToList();
                if (primaryRecords.Any())
                {
                    // true only if every primary account has uploaded cheque document
                    hasUploadedAllDocs = primaryRecords.All(r => r.HasChequeDocUploaded == true);
                }
                else
                {
                    // no primary account present => treat as false
                    hasUploadedAllDocs = false;
                }

                // 📦 Return paged response
                return new PagedResponseDTO<GetBankResponseDTO>
                {
                    Items = records,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasUploadedAll = hasUploadedAllDocs,
                    CompletionPercentage = averagePercentage
                };
            }
            catch (Exception ex)
            {
                // ❌ Exception logging
                _logger.LogError(ex, "❌ Error fetching bank info for EmployeeId {EmployeeId}", employeeId);
                throw new Exception($"Failed to fetch bank information: {ex.Message}");
            }
        }






        public async Task<GetBankResponseDTO> GetSingleRecordAsync(int id, bool isActive)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🔹 Record fetch karna with filters
                var entity = await context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(b => b.Id == id && b.IsActive == isActive && (b.IsSoftDeleted !=true))
                    .Select(b => new GetBankResponseDTO
                    {
                        Id = b.Id.ToString(),
                        EmployeeId = b.EmployeeId.ToString(),
                        BankName = b.BankName,
                        AccountNumber = b.AccountNumber,
                        IFSCCode = b.IFSCCode,
                        BranchName = b.BranchName,
                        AccountType = b.AccountType,                       
                        IsPrimaryAccount = b.IsPrimaryAccount,
                        IsActive = b.IsActive,
                       
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

    }
}