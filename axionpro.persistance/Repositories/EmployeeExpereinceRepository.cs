
using AutoMapper;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Experience;
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
    public class EmployeeExpereinceRepository : IEmployeeExpereinceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeExpereinceRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeExpereinceRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeExpereinceRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        //public async Task<PagedResponseDTO<GetExperienceResponseDTO>> CreateAsync(EmployeeExperience entity)
        //{
        //    try
        //    {
        //        // 1️⃣ Validation
        //        if (entity == null)
        //            throw new ArgumentNullException(nameof(entity), "Experience entity cannot be null.");

        //        if (entity.EmployeeId <= 0)
        //            throw new ArgumentException("Invalid EmployeeId provided.");

        //        // 2️⃣ Insert Record
        //        await _context.EmployeeExperiences.AddAsync(entity);
        //        await _context.SaveChangesAsync();

        //        // 3️⃣ Fetch Updated List (Latest Records)
        //        var baseQuery = _context.EmployeeExperiences
        //            .AsNoTracking()
        //            .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
        //            .OrderByDescending(x => x.Id);

        //        var totalRecords = await baseQuery.CountAsync();

        //        var records = await baseQuery
        //            .Take(10)
        //            .ToListAsync();

        //        // 4️⃣ Map to DTO
        //        var responseData = _mapper.Map<List<GetExperienceResponseDTO>>(records);

        //        // 5️⃣ Prepare PagedResponse
        //        return new PagedResponseDTO<GetExperienceResponseDTO>
        //        {
        //            Items = responseData,
        //            TotalCount = totalRecords,
        //            PageNumber = 1,
        //            PageSize = 10
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "❌ Error occurred while adding Experience info for EmployeeId: {EmployeeId}", entity.EmployeeId);
        //        throw new Exception($"Failed to add or fetch Experience info: {ex.Message}");
        //    }
        //}
       
        //public async Task<PagedResponseDTO<GetExperienceResponseDTO>> GetInfo(GetExperienceRequestDTO dto, long EmployeeId, long Id)
        //{
        //    try
        //    {
        //        // 🔹 Pagination & Sorting Defaults
        //        int pageNumber = dto.PageNumber > 0 ? dto.PageNumber : 1;
        //        int pageSize = dto.PageSize > 0 ? dto.PageSize : 10;
        //        string sortBy = !string.IsNullOrWhiteSpace(dto.SortBy) ? dto.SortBy.ToLower() : "id";
        //        string sortOrder = !string.IsNullOrWhiteSpace(dto.SortOrder) ? dto.SortOrder.ToLower() : "desc";

        //        // 🧭 Base Query with Mandatory Filters
        //        var baseQuery = _context.EmployeeExperiences
        //            .AsNoTracking()
        //            .Where(exp => exp.EmployeeId == EmployeeId && exp.IsSoftDeleted != true);

        //        // 🧩 Optional Filters

        //        long id = 0;
        //        if (!string.IsNullOrWhiteSpace(dto.Id))
        //        {
        //            long.TryParse(dto.Id, out id);
        //        }
        //        if (id > 0)
        //            baseQuery = baseQuery.Where(x => x.Id == id);


        //        if (!string.IsNullOrEmpty(dto.CompanyName))
        //            baseQuery = baseQuery.Where(x => x.CompanyName.ToLower().Contains(dto.CompanyName.ToLower()));

        //        if (dto.IsActive.HasValue)
        //            baseQuery = baseQuery.Where(x => x.IsActive == dto.IsActive);

        //        if (dto.IsExperienceVerified.HasValue)
        //            baseQuery = baseQuery.Where(x => x.IsExperienceVerified == dto.IsExperienceVerified);

        //        if (dto.IsExperienceVerifiedByMail.HasValue)
        //            baseQuery = baseQuery.Where(x => x.IsExperienceVerifiedByMail == dto.IsExperienceVerifiedByMail);

        //        if (dto.IsExperienceVerifiedByCall.HasValue)
        //            baseQuery = baseQuery.Where(x => x.IsExperienceVerifiedByCall == dto.IsExperienceVerifiedByCall);

        //        if (dto.ExperienceTypeId.HasValue)
        //            baseQuery = baseQuery.Where(x => x.ExperienceTypeId == dto.ExperienceTypeId);

        //        if (dto.IsEditAllowed.HasValue)
        //            baseQuery = baseQuery.Where(x => x.IsEditAllowed == dto.IsEditAllowed);

        //        // 🔽 Dynamic Sorting
        //        bool isDescending = sortOrder == "desc";
        //        baseQuery = sortBy switch
        //        {
        //            "companyname" => isDescending
        //                ? baseQuery.OrderByDescending(x => x.CompanyName)
        //                : baseQuery.OrderBy(x => x.CompanyName),

        //            "experiencetypeid" => isDescending
        //                ? baseQuery.OrderByDescending(x => x.ExperienceTypeId)
        //                : baseQuery.OrderBy(x => x.ExperienceTypeId),

        //            "addedatetime" => isDescending
        //                ? baseQuery.OrderByDescending(x => x.AddedDateTime)
        //                : baseQuery.OrderBy(x => x.AddedDateTime),

        //            _ => isDescending
        //                ? baseQuery.OrderByDescending(x => x.Id)
        //                : baseQuery.OrderBy(x => x.Id)
        //        };

        //        // 📄 Total Count
        //        var totalRecords = await baseQuery.CountAsync();

        //        // 🧩 Projection to DTO
        //        var query = baseQuery.Select(exp => new GetExperienceResponseDTO
        //        {
        //            // 🆔 IDs
        //            Id = exp.Id.ToString(),
        //            EmployeeId = exp.EmployeeId.ToString(),

        //            // 🏢 Experience Info
        //            CompanyName = exp.CompanyName,
        //            ExperienceTypeId = exp.ExperienceTypeId,
        //            CTC = exp.Ctc,

        //            // ✅ Verification Flags
        //            IsExperienceVerified = exp.IsExperienceVerified,
        //            IsExperienceVerifiedByMail = exp.IsExperienceVerifiedByMail,
        //            IsExperienceVerifiedByCall = exp.IsExperienceVerifiedByCall,

        //            // ⚙️ Flags
        //            IsEditAllowed = exp.IsEditAllowed,
        //            IsActive = exp.IsActive,

        //            // 🕒 Audit Fields
        //            AddedById = exp.AddedById.ToString(),
        //            AddedDateTime = exp.AddedDateTime,
        //            UpdatedById = exp.UpdatedById.ToString(),
        //            UpdatedDateTime = exp.UpdatedDateTime
        //        });

        //        // 📜 Pagination
        //        var pagedRecords = await query
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        // 📦 Final Response
        //        return new PagedResponseDTO<GetExperienceResponseDTO>(
        //            pagedRecords ?? new List<GetExperienceResponseDTO>(),
        //            totalRecords,
        //            pageNumber,
        //            pageSize
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "❌ Error occurred while fetching Experience info for EmployeeId: {EmployeeId}", EmployeeId);
        //        throw new Exception($"Failed to fetch Experience info: {ex.Message}");
        //    }
        //}

        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEmployeeFieldAsync(long Id, string entity, string fieldName, object? fieldValue, long updatedById)
        {
            throw new NotImplementedException();
        }
    }

}




 
 






