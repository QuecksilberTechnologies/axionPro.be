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
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
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
                        IsActive = bank.IsActive
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


        public async Task<PagedResponseDTO<GetBankResponseDTO>> GetInfo( GetBankReqestDTO dto, long tenantId, long EmployeeId)
        {
            // 🧭 Base query
            var query = _context.EmployeeBankDetails
                .AsNoTracking()
                .Where(x => x.EmployeeId == EmployeeId &&
                            x.IsActive == dto.IsActive &&
                            x.IsSoftDeleted != true);

            // 🔍 Apply search
            if (!string.IsNullOrEmpty(dto.SortOrder))
            {
                query = query.Where(x =>
                    x.BankName.Contains(dto.SortOrder) ||
                    x.AccountNumber.Contains(dto.SortOrder));
            }

            // 🔽 Apply sorting
            bool isDescending = string.Equals(dto.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(dto.SortBy))
            {
                query = dto.SortBy.ToLower() switch
                {
                    "bankname" => isDescending ? query.OrderByDescending(x => x.BankName) : query.OrderBy(x => x.BankName),
                    "accountnumber" => isDescending ? query.OrderByDescending(x => x.AccountNumber) : query.OrderBy(x => x.AccountNumber),
                    _ => isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
                };
            }
            else
            {
                query = isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
            }

            // 📄 Pagination
            var totalRecords = await query.CountAsync();

            var records = await query
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new GetBankResponseDTO
                {
                    // 🔐 Encrypt sensitive identifiers here
                    Id = x.Id.ToString(),
                    EmployeeId =x.EmployeeId.ToString(),

                    // 🏦 Business data
                    BankName = x.BankName,
                    AccountNumber = x.AccountNumber,
                    IFSCCode = x.IFSCCode,
                    BranchName = x.BranchName,
                    IsPrimaryAccount = x.IsPrimaryAccount,
                    IsActive = x.IsActive,
                    IsInfoVerified = x.IsInfoVerified,
                    IsEditAllowed = x.IsEditAllowed
                })
                .ToListAsync();

            // 📦 Return paged response
            return new PagedResponseDTO<GetBankResponseDTO>
            {
                Items = records,
                TotalCount = totalRecords,
                 PageNumber= dto.PageNumber,
                PageSize = dto.PageSize,
                
            };
        }

        public Task<EmployeeBankDetail> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }
}