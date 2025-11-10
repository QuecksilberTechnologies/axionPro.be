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
                // 🧭 Base Query (Active & SoftDelete check)
                var baseQuery = _context.EmployeeDependents
                    .AsNoTracking()
                    .Where(dep => dep.EmployeeId == employeeId
                                  && (dto.IsActive == null || dep.IsActive == dto.IsActive)
                                  && dep.IsSoftDeleted != true);

                // 🗺️ Optional Filters
                if (!string.IsNullOrWhiteSpace(dto.Id) && long.TryParse(dto.Id, out long parsedId) && parsedId > 0)
                    baseQuery = baseQuery.Where(x => x.Id == parsedId);

                if (!string.IsNullOrEmpty(dto.Relation))
                    baseQuery = baseQuery.Where(x => x.Relation.ToLower().Contains(dto.Relation.ToLower()));

                if (dto.IsCoveredInPolicy.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsCoveredInPolicy == dto.IsCoveredInPolicy);

                if (dto.IsMarried.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsMarried == dto.IsMarried);

                // 🔍 Keyword Search (on DependentName or Relation)
                if (!string.IsNullOrEmpty(dto.SortBy))
                {
                    var keyword = dto.SortBy.Trim().ToLower();
                    baseQuery = baseQuery.Where(x =>
                        (x.DependentName != null && x.DependentName.ToLower().Contains(keyword)) ||
                        (x.Relation != null && x.Relation.ToLower().Contains(keyword)));
                }

                // 🔽 Sorting
                bool isDescending = string.Equals(dto.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

                baseQuery = !string.IsNullOrEmpty(dto.SortBy)
                    ? dto.SortBy.ToLower() switch
                    {
                        "relation" => isDescending
                            ? baseQuery.OrderByDescending(x => x.Relation)
                            : baseQuery.OrderBy(x => x.Relation),

                        "iscoveredinpolicy" => isDescending
                            ? baseQuery.OrderByDescending(x => x.IsCoveredInPolicy)
                            : baseQuery.OrderBy(x => x.IsCoveredInPolicy),

                        "ismarried" => isDescending
                            ? baseQuery.OrderByDescending(x => x.IsMarried)
                            : baseQuery.OrderBy(x => x.IsMarried),

                        _ => isDescending
                            ? baseQuery.OrderByDescending(x => x.Id)
                            : baseQuery.OrderBy(x => x.Id)
                    }
                    : baseQuery.OrderByDescending(x => x.Id);

                // 📄 Total Count
                var totalRecords = await baseQuery.CountAsync();

                // 🧩 Projection to DTO
                var query = from dep in baseQuery
                            select new GetDependentResponseDTO
                            {
                                // 🔐 Encrypted IDs
                                 Id = dep.Id.ToString(),
                                EmployeeId = dep.EmployeeId.ToString(),

                                // 👪 Dependent Info
                                DependentName = dep.DependentName,
                                Relation = dep.Relation,
                                DateOfBirth = dep.DateOfBirth,
                                IsCoveredInPolicy = dep.IsCoveredInPolicy,
                                IsMarried = dep.IsMarried,
                                Remark = dep.Remark,
                                Description = dep.Description,

                                // 🧾 Status Fields
                                IsActive = dep.IsActive,
                                IsInfoVerified = dep.IsInfoVerified,
                                IsEditAllowed = dep.IsEditAllowed,

                                // 📅 Audit Fields
                               
                                InfoVerifiedById = dep.InfoVerifiedById.ToString(),
                                InfoVerifiedDateTime = dep.InfoVerifiedDateTime
                            };

                // 🚫 Remove Duplicates
                var distinctQuery = query.DistinctBy(x => x.Id);

                // 📜 Pagination
                var pagedRecords = await distinctQuery
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToListAsync();

                // 📦 Final Response
                return new PagedResponseDTO<GetDependentResponseDTO>
                {
                    Items = pagedRecords ?? new List<GetDependentResponseDTO>(),
                    TotalCount = totalRecords,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
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




 
 






