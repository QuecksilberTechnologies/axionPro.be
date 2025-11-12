using AutoMapper;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{

    public class EmployeeDependentRepository : IEmployeeDependentRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeDependentRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeDependentRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeDependentRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public async Task<PagedResponseDTO<GetDependentResponseDTO>> CreateAsync(EmployeeDependent entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Dependent info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // Add record
                await _context.EmployeeDependents.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Fetch updated list
                var query = _context.EmployeeDependents
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id);

                var totalRecords = await query.CountAsync();

                int pageSize = 10; // fixed or parameter
                var records = await query.Take(pageSize).ToListAsync();

                var responseData = _mapper.Map<List<GetDependentResponseDTO>>(records);

                return new PagedResponseDTO<GetDependentResponseDTO>
                {
                    Items = responseData,
                    TotalCount = totalRecords,
                    PageNumber = 1,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while adding/fetching Dependent info for EmployeeId: {EmployeeId}", entity.EmployeeId);
                throw new Exception($"Failed to add or fetch Dependent info: {ex.Message}");
            }
        }
        public async Task<PagedResponseDTO<GetDependentResponseDTO>> GetInfo(GetDependentRequestDTO dto, long employeeId, long id)
        {
            try
            {
                // 🔹 Pagination defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;
                string sortBy = dto.SortBy?.Trim().ToLower() ?? "id";
                bool isDescending = string.Equals(dto.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

                // 🧭 Base query (Active & Not SoftDeleted)
                var query = _context.EmployeeDependents
                    .AsNoTracking()
                    .Where(dep => dep.EmployeeId == employeeId && dep.IsSoftDeleted != true);

                // 🗺 Optional Filters
                if (dto.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == dto.IsActive);

                if (!string.IsNullOrWhiteSpace(dto.Id) && long.TryParse(dto.Id, out long parsedId) && parsedId > 0)
                    query = query.Where(x => x.Id == parsedId);

                if (!string.IsNullOrWhiteSpace(dto.Relation))
                {
                    string relationFilter = dto.Relation.ToLower();
                    query = query.Where(x => x.Relation.ToLower().Contains(relationFilter));
                }

                if (dto.IsCoveredInPolicy.HasValue)
                    query = query.Where(x => x.IsCoveredInPolicy == dto.IsCoveredInPolicy);

                if (dto.IsMarried.HasValue)
                    query = query.Where(x => x.IsMarried == dto.IsMarried);
                if (dto.HasProofUploaded)
                    query = query.Where(x => x.HasProofUploaded == dto.HasProofUploaded);

                // 🔽 Sorting
                query = sortBy switch
                {
                    "relation" => isDescending ? query.OrderByDescending(x => x.Relation) : query.OrderBy(x => x.Relation),
                    "iscoveredinpolicy" => isDescending ? query.OrderByDescending(x => x.IsCoveredInPolicy) : query.OrderBy(x => x.IsCoveredInPolicy),
                    "ismarried" => isDescending ? query.OrderByDescending(x => x.IsMarried) : query.OrderBy(x => x.IsMarried),
                    "hasproofUploaded" => isDescending ? query.OrderByDescending(x => x.HasProofUploaded) : query.OrderBy(x => x.HasProofUploaded),

                    _ => isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
                };

                // 📄 Total count before pagination
                var totalRecords = await query.CountAsync();

                // 🧩 Projection to DTO (EF-friendly)
                var projectedList = await query
                    .Select(dep => new GetDependentResponseDTO
                    {
                        Id = dep.Id.ToString(),
                        EmployeeId = dep.EmployeeId.ToString(),
                        DependentName = dep.DependentName,
                        Relation = dep.Relation,
                        DateOfBirth = dep.DateOfBirth,
                        IsCoveredInPolicy = dep.IsCoveredInPolicy,
                        IsMarried = dep.IsMarried,
                        HasProofUploaded = dep.HasProofUploaded,
                        ProofDocPath = dep.ProofDocPath,
                        Remark = dep.Remark,
                        Description = dep.Description,
                        IsActive = dep.IsActive,
                        IsInfoVerified = dep.IsInfoVerified,
                        IsEditAllowed = dep.IsEditAllowed,
                        InfoVerifiedById = dep.InfoVerifiedById.ToString(),
                        InfoVerifiedDateTime = dep.InfoVerifiedDateTime
                    })
                    .ToListAsync();

                // 🚫 DistinctBy apply karo ab (C# level pe)
                var distinctList = projectedList
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToList();

                // 📜 Pagination
                var pagedRecords = distinctList
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 📦 Return final paged response
                return new PagedResponseDTO<GetDependentResponseDTO>
                {
                    Items = pagedRecords,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching dependent info for EmployeeId: {EmployeeId}", employeeId);
                throw new Exception($"Failed to fetch dependents: {ex.Message}");
            }
        }

        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }


}




 
 






