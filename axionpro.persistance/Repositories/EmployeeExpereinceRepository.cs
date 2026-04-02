
using AutoMapper;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class EmployeeExpereinceRepository : IEmployeeExperienceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeExpereinceRepository> _logger;

        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        private readonly IFileStorageService _fileStorageService;
        public EmployeeExpereinceRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeExpereinceRepository> logger,
            IPasswordService passwordService, IEncryptionService encryptionService,IFileStorageService fileStorageService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;

            _passwordService = passwordService;
            _encryptionService = encryptionService;
            _fileStorageService = fileStorageService;

        }
        // ===============================
        // 🔹 CREATE
        // ===============================
        public async Task<GetEmployeeExperienceResponseDTO> AddAsync(EmployeeExperience entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // 🔥 ONLY ADD (NO SAVE)
            await _context.EmployeeExperiences.AddAsync(entity);

            // ❌ REMOVE THIS LINE
            // await _context.SaveChangesAsync();

            // 👉 Just return empty or minimal DTO (optional)
            return new GetEmployeeExperienceResponseDTO();
        }
        //public async Task<GetEmployeeExperienceResponseDTO> AddAsync(EmployeeExperience entity)
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    // 🔹 Add
        //    await _context.EmployeeExperiences.AddAsync(entity);

        //    // 🔹 Save (IMPORTANT 🔥)
        //    await _context.SaveChangesAsync();

        //    // 🔹 Map to DTO
        //    var result = new GetEmployeeExperienceResponseDTO
        //    {
        //        Id = entity.Id,
        //        EmployeeId = entity.EmployeeId.ToString(),

        //        CompanyName = entity.CompanyName,
        //        Designation = entity.Designation,
        //        EmployeeIdOfCompany = entity.EmployeeIdOfCompany,
        //        Ctc = entity.Ctc,

        //        StartDate = entity.StartDate,
        //        EndDate = entity.EndDate,
        //        Experience = entity.Experience,

        //        IsWFH = entity.IsWFH,

        //        WorkingCountryId = entity.WorkingCountryId,
        //        WorkingStateId = entity.WorkingStateId,
        //        WorkingDistrictId = entity.WorkingDistrictId,

        //        IsForeignExperience = entity.IsForeignExperience,

        //        ReasonForLeaving = entity.ReasonForLeaving,
        //        Remark = entity.Remark,

        //        ColleagueName = entity.ColleagueName,
        //        ColleagueDesignation = entity.ColleagueDesignation,
        //        ColleagueContactNumber = entity.ColleagueContactNumber,

        //        ReportingManagerName = entity.ReportingManagerName,
        //        ReportingManagerNumber = entity.ReportingManagerNumber,

        //        VerificationEmail = entity.VerificationEmail,

        //        IsAnyGap = entity.IsAnyGap,
        //        ReasonOfGap = entity.ReasonOfGap,
        //        GapYearFrom = entity.GapYearFrom,
        //        GapYearTo = entity.GapYearTo,

        //        IsEditAllowed = entity.IsEditAllowed,
        //        IsInfoVerified = entity.IsInfoVerified,

        //        Documents = new List<GetEmployeeExperienceDocumentDTO>()
        //    };

        //    // 🔥 Completion calculate
        //    result.CompletionPercentage =
        //        CompletionCalculatorHelper.ExperiencePropCalculate(result);

        //    return result;
        //}

        // ===============================
        // 🔹 UPDATE
        // ===============================
        public async Task<bool> UpdateAsync(EmployeeExperience entity)
        {
            _context.EmployeeExperiences.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteAsync(EmployeeExperience entity)
        {
            // 🔹 Parent Soft Delete
            entity.IsSoftDeleted = true;
            entity.IsActive = false;

            // 🔹 Child Documents Soft Delete
            if (entity.EmployeeExperienceDocuments != null && entity.EmployeeExperienceDocuments.Any())
            {
                foreach (var doc in entity.EmployeeExperienceDocuments)
                {
                    doc.IsSoftDeleted = true;
                    doc.IsActive = false;
                    doc.DeletedDateTime = DateTime.UtcNow;
                    doc.SoftDeletedById = entity.SoftDeletedById; // Assuming the same user is performing the delete action
                }
            }

            _context.EmployeeExperiences.Update(entity);

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> SoftDeleteDocAsync(EmployeeExperienceDocument entity)
        {

            _context.EmployeeExperienceDocuments.Update(entity);

            return await _context.SaveChangesAsync() > 0;
        }

        // ===============================
        // 🔹 GET BY ID
        // ===============================
        public async Task<EmployeeExperience?> GetByIdAsync(long id, long employeeId)
        {
            return await _context.EmployeeExperiences
                .Include(x => x.EmployeeExperienceDocuments) // ✅ direct include
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.EmployeeId == employeeId &&
                    !x.IsSoftDeleted);
        }
        // ===============================
        // 🔹 GET LIST (PAGINATED)
        // ===============================

        public async Task<PagedResponseDTO<GetEmployeeExperienceResponseDTO>> GetByEmployeeIdWithDocumentsAsync(GetExperienceRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🚀 START GetExperience | EmployeeId: {EmployeeId}", dto?.Prop?.EmployeeId);

                // -----------------
                // Pagination defaults
                // -----------------
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                string sortBy = dto.SortBy?.ToLower() ?? "id";
                bool isDescending = (dto.SortOrder?.ToLower() ?? "desc") == "desc";

                _logger.LogInformation("📄 Pagination | Page: {Page}, Size: {Size}, SortBy: {Sort}, Desc: {Desc}",
                    pageNumber, pageSize, sortBy, isDescending);

                // -----------------
                // Base Query
                // -----------------
                var baseQuery = _context.EmployeeExperiences
                    .AsNoTracking()
                    .Include(x => x.EmployeeExperienceDocuments)
                    .Where(x =>
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        x.IsActive &&
                        !x.IsSoftDeleted
                    );

                _logger.LogInformation("🔍 BaseQuery applied | EmployeeId: {EmployeeId}", dto.Prop.EmployeeId);

                // -----------------
                // Sorting
                // -----------------
                baseQuery = sortBy switch
                {
                    "companyname" => isDescending ? baseQuery.OrderByDescending(x => x.CompanyName) : baseQuery.OrderBy(x => x.CompanyName),
                    "designation" => isDescending ? baseQuery.OrderByDescending(x => x.Designation) : baseQuery.OrderBy(x => x.Designation),
                    "startdate" => isDescending ? baseQuery.OrderByDescending(x => x.StartDate) : baseQuery.OrderBy(x => x.StartDate),
                    "enddate" => isDescending ? baseQuery.OrderByDescending(x => x.EndDate) : baseQuery.OrderBy(x => x.EndDate),
                    _ => isDescending ? baseQuery.OrderByDescending(x => x.Id) : baseQuery.OrderBy(x => x.Id)
                };

                _logger.LogInformation("📊 Sorting applied");

                // -----------------
                // Count
                // -----------------
                var totalRecords = await baseQuery.CountAsync();
                _logger.LogInformation("📦 Total Records: {Count}", totalRecords);

                // -----------------
                // Fetch
                // -----------------
                var expList = await baseQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("📥 Records Fetched: {Count}", expList.Count);

                // -----------------
                // Mapping + Completion
                // -----------------
                List<GetEmployeeExperienceResponseDTO> finalList = new();
                double totalPercentage = 0;

                foreach (var exp in expList)
                {
                    _logger.LogInformation("➡️ Mapping Experience Id: {Id}", exp.Id);

                    var dtoItem = new GetEmployeeExperienceResponseDTO
                    {
                        Id = exp.Id,

                        CompanyName = exp.CompanyName,
                        Designation = exp.Designation,
                        EmployeeIdOfCompany = exp.EmployeeIdOfCompany,
                        Ctc = exp.Ctc,

                        StartDate = exp.StartDate,
                        EndDate = exp.EndDate,
                        Experience = exp.Experience,

                        IsWFH = exp.IsWFH,

                        WorkingCountryId = exp.WorkingCountryId,
                        WorkingStateId = exp.WorkingStateId,
                        WorkingDistrictId = exp.WorkingDistrictId,

                        IsForeignExperience = exp.IsForeignExperience,

                        ReasonForLeaving = exp.ReasonForLeaving,
                        Remark = exp.Remark,

                        ColleagueName = exp.ColleagueName,
                        ColleagueDesignation = exp.ColleagueDesignation,
                        ColleagueContactNumber = exp.ColleagueContactNumber,

                        ReportingManagerName = exp.ReportingManagerName,
                        ReportingManagerNumber = exp.ReportingManagerNumber,

                        VerificationEmail = exp.VerificationEmail,

                        IsAnyGap = exp.IsAnyGap,
                        ReasonOfGap = exp.ReasonOfGap,
                        GapYearFrom = exp.GapYearFrom,
                        GapYearTo = exp.GapYearTo,

                        IsEditAllowed = exp.IsEditAllowed,
                        IsInfoVerified = exp.IsInfoVerified,

                        Documents = exp.EmployeeExperienceDocuments.Select(d => new GetEmployeeExperienceDocumentDTO
                        {
                            Id = d.Id,
                            DocumentType = d.DocumentType,
                            HasExperienceDocUploaded = d.HasExperienceDocUploaded,
                            FileName = d.FileName,
                            FilePath = !string.IsNullOrEmpty(d.FilePath)
                                ? _fileStorageService.GetFileUrl(d.FilePath)
                                : null,
                            Remark = d.Remark
                        }).ToList()
                    };

                    dtoItem.CompletionPercentage = CompletionCalculatorHelper.ExperiencePropCalculate(dtoItem);

                    totalPercentage += dtoItem.CompletionPercentage;

                    finalList.Add(dtoItem);
                }

                _logger.LogInformation("🧮 Mapping Completed | Items: {Count}", finalList.Count);

                // -----------------
                // Section Completion
                // -----------------
                double averagePercentage = finalList.Count > 0
                    ? Math.Round(totalPercentage / finalList.Count, 0)
                    : 0;

                _logger.LogInformation("📊 Completion %: {Percent}", averagePercentage);

                // -----------------
                // Has Uploaded All Docs
                // -----------------
                bool hasUploadedAll = finalList.All(x =>
                    x.Documents != null && x.Documents.Any());

                _logger.LogInformation("📁 HasUploadedAll: {Flag}", hasUploadedAll);

                // -----------------
                // Final Response
                // -----------------
                _logger.LogInformation("✅ END GetExperience SUCCESS");

                return new PagedResponseDTO<GetEmployeeExperienceResponseDTO>
                {
                    Items = finalList,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    CompletionPercentage = averagePercentage,
                    HasUploadedAll = hasUploadedAll
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ ERROR GetExperience | EmployeeId: {EmployeeId}",
                    dto?.Prop?.EmployeeId);

                throw;
            }
        }


    }

}


 
 






