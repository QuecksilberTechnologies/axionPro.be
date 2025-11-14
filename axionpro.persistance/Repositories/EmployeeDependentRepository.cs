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

        public async Task<PagedResponseDTO<GetDependentResponseDTO>> GetInfo(
      GetDependentRequestDTO dto,
      long employeeId,
      long id)
        {
            var response = new PagedResponseDTO<GetDependentResponseDTO>();

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✔ Base Query
                var query = context.EmployeeDependents
                    .Where(d => d.IsSoftDeleted != true)
                    .AsQueryable();

                // ✔ Filter by Dependent Id
                if (id > 0)
                    query = query.Where(d => d.Id == id);

                // ✔ Filter by Employee
                if (employeeId > 0)
                    query = query.Where(d => d.EmployeeId == employeeId);

                // ✔ Filter by Name
                if (!string.IsNullOrWhiteSpace(dto.Relation))
                    query = query.Where(d => d.Relation.ToLower().Contains(dto.Relation.ToLower()));

                // ✔ Filter by Active
                if (dto.IsActive.HasValue)
                    query = query.Where(d => d.IsActive == dto.IsActive.Value);

                // ✔ Sorting
                query = dto.SortBy?.ToLower() switch
                {
                    "dependentname" => dto.SortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.DependentName)
                        : query.OrderByDescending(x => x.DependentName),
                    "relation" => dto.SortOrder?.ToLower() == "asc"
                   ? query.OrderBy(x => x.Relation)
                   : query.OrderByDescending(x => x.Relation),

                    _ => query.OrderByDescending(x => x.Id)
                };

                // ✔ Pagination
                var totalRecords = await query.CountAsync();
                var dependents = await query
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .AsNoTracking()
                    .ToListAsync();

                // ⭐ Build Final List with Completion Logic
                var finalList = dependents.Select(dep =>
                {
                    double completion = (
                        new[]
                        {
                    string.IsNullOrEmpty(dep.DependentName) ? 0 : 1,
                    string.IsNullOrEmpty(dep.Relation) ? 0 : 1,
                    dep.DateOfBirth.HasValue ? 1 : 0,
                    dep.IsCoveredInPolicy.HasValue ? 1 : 0,
                    dep.IsMarried.HasValue ? 1 : 0,
                    string.IsNullOrEmpty(dep.Remark) ? 0 : 1,
                    string.IsNullOrEmpty(dep.Description) ? 0 : 1,
                    dep.HasProofUploaded ? 1 : 0
                        }.Sum() / 8.0
                    ) * 100;

                    return new GetDependentResponseDTO
                    {
                        Id = id.ToString(),
                        DependentName = dep.DependentName,
                        Relation = dep.Relation,
                        DateOfBirth = dep.DateOfBirth,
                        IsCoveredInPolicy = dep.IsCoveredInPolicy,
                        IsMarried = dep.IsMarried,
                        Remark = dep.Remark,
                        Description = dep.Description,
                        HasProofUploaded = dep.HasProofUploaded,
                        CompletionPercentage = completion,
                        HasUploadedAll = dep.HasProofUploaded
                    };
                }).ToList();

                // ⭐ Fill Paged Response
                response.Items = finalList;
                response.TotalCount = totalRecords;
                response.PageNumber = dto.PageNumber;
                response.PageSize = dto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / dto.PageSize);


                // ⭐ Average Completion
                response.CompletionPercentage = finalList.Count > 0
                    ? finalList.Average(x => x.CompletionPercentage)
                    : 0;

                // ⭐ Check if all dependents uploaded proof
                response.HasUploadedAll = finalList.All(x => x.HasProofUploaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching dependents");
                response.Items = new List<GetDependentResponseDTO>();
            }

            return response;
        }


        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }


}




 
 






