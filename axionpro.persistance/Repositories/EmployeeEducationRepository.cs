using AutoMapper;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{

    public class EmployeeEducationRepository : IEmployeeEducationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeEducationRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeEducationRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeEducationRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public async Task<PagedResponseDTO<GetEducationResponseDTO>> CreateAsync(EmployeeEducation entity)
        {
            try
            {
                // ✅ 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Dependent info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // ✅ 2️⃣ Record insert karo
                await _context.EmployeeEducations.AddAsync(entity);
                await _context.SaveChangesAsync();

                // ✅ 3️⃣ Fetch updated list (latest record ke sath)
                var query = _context.EmployeeEducations
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id);

                var totalRecords = await query.CountAsync();

                // ✅ 4️⃣ Fetch paginated data
                var records = await query.Take(10).ToListAsync();

                // ✅ 5️⃣ Map to DTOs
                var responseData = _mapper.Map<List<GetEducationResponseDTO>>(records);

                // ✅ 6️⃣ Prepare PagedResponse
                return  new PagedResponseDTO<GetEducationResponseDTO>
                {
                    Items = responseData,
                    TotalCount = totalRecords,
                    PageNumber = 1,
                    PageSize = 10,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while adding/fetching Dependent info for EmployeeId: {EmployeeId}", entity.EmployeeId);
                throw new Exception($"Failed to add or fetch Dependent info: {ex.Message}");
            }
        }


        public async Task<PagedResponseDTO<GetEducationResponseDTO>> GetInfo(GetEducationRequestDTO dto, long employeeId, long id)
        {
            try
            {
                // 📄 Pagination Defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // 📦 Sorting Defaults (case-insensitive)
                string sortBy = !string.IsNullOrWhiteSpace(dto.SortBy) ? dto.SortBy.ToLower() : "id";
                string sortOrder = !string.IsNullOrWhiteSpace(dto.SortOrder) ? dto.SortOrder.ToLower() : "desc";
                bool isDescending = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

                // 🧭 Base Query (with filters)
                var baseQuery = _context.EmployeeEducations
                    .AsNoTracking()
                    .Where(edu =>
                        edu.EmployeeId == employeeId &&
                        (dto.IsActive == null || edu.IsActive == dto.IsActive) &&
                        (edu.IsSoftDeleted != true)
                    );

                if (id > 0)
                    baseQuery = baseQuery.Where(x => x.Id == id);

                if (!string.IsNullOrEmpty(dto.InstituteName))
                    baseQuery = baseQuery.Where(x => x.InstituteName.ToLower().Contains(dto.InstituteName.ToLower()));

                if (!string.IsNullOrEmpty(dto.Degree))
                    baseQuery = baseQuery.Where(x => x.Degree.ToLower().Contains(dto.Degree.ToLower()));

                if (!string.IsNullOrEmpty(dto.GradeOrPercentage))
                    baseQuery = baseQuery.Where(x => x.GradeOrPercentage.ToLower().Contains(dto.GradeOrPercentage.ToLower()));

                if (dto.EducationGap.HasValue)
                    baseQuery = baseQuery.Where(x => x.EducationGap == dto.EducationGap);

                if (dto.IsInfoVerified.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsInfoVerified == dto.IsInfoVerified);

                if (dto.IsEditAllowed.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsEditAllowed == dto.IsEditAllowed);

                // 🧩 Sorting Logic — clean & consistent
                switch (sortBy)
                {
                    case "degree":
                        baseQuery = isDescending ? baseQuery.OrderByDescending(x => x.Degree) : baseQuery.OrderBy(x => x.Degree);
                        break;

                    case "institutename":
                        baseQuery = isDescending ? baseQuery.OrderByDescending(x => x.InstituteName) : baseQuery.OrderBy(x => x.InstituteName);
                        break;

                    case "haseducationdocuploded":
                        baseQuery = isDescending ? baseQuery.OrderByDescending(x => x.HasEducationDocUploded) : baseQuery.OrderBy(x => x.HasEducationDocUploded);
                        break;

                    case "startdate":
                        baseQuery = isDescending ? baseQuery.OrderByDescending(x => x.StartDate) : baseQuery.OrderBy(x => x.StartDate);
                        break;

                    case "enddate":
                        baseQuery = isDescending ? baseQuery.OrderByDescending(x => x.EndDate) : baseQuery.OrderBy(x => x.EndDate);
                        break;

                    default:
                        baseQuery = isDescending ? baseQuery.OrderByDescending(x => x.Id) : baseQuery.OrderBy(x => x.Id);
                        break;
                }

                // 📊 Total Count (before pagination)
                var totalRecords = await baseQuery.CountAsync();

                // 🧩 Apply projection + pagination on same query (to retain sorting)
                var pagedRecords = await baseQuery
                    .Select(edu => new GetEducationResponseDTO
                    {
                        Id = edu.Id.ToString(),
                        EmployeeId = edu.EmployeeId.ToString(),
                        Degree = edu.Degree,
                        InstituteName = edu.InstituteName,
                        Remark = edu.Remark,
                        GradeOrPercentage = edu.GradeOrPercentage,
                        GPAOrPercentage = edu.GpaorPercentage,
                        EducationGap = edu.EducationGap,
                        ReasonOfEducationGap = edu.ReasonOfEducationGap,
                        StartDate = edu.StartDate,
                        EndDate = edu.EndDate,
                        EducationDocPath = edu.EducationDocPath,
                        DocType = edu.DocType.ToString(),
                        DocName = edu.DocName,
                        IsActive = edu.IsActive
                    })
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 📦 Final Response
                return new PagedResponseDTO<GetEducationResponseDTO>
                {
                    Items = pagedRecords,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)


                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching education info for EmployeeId: {EmployeeId}", employeeId);
                throw new Exception($"Failed to fetch education info: {ex.Message}");
            }
        }

        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEmployeeFieldAsync(long Id, string entity, string fieldName, object? fieldValue, long updatedById)
        {
            throw new NotImplementedException();
        }

        Task<EmployeeEducation> IEmployeeEducationRepository.GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }

}




 
 






