using AutoMapper;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Extentions;
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
                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Dependent info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // ✅ 2️⃣ Record insert karo
                await context.EmployeeEducations.AddAsync(entity);
                await context.SaveChangesAsync();

                // ✅ 3️⃣ Fetch updated list (latest record ke sath)
                var query = context.EmployeeEducations
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

        public async Task<PagedResponseDTO<GetEducationResponseDTO>> GetInfo(GetEducationRequestDTO dto)
        {
            double averagePercentage = 0;
            bool hasUploadedAll = false;

            try
            {
                // -----------------
                // Pagination defaults
                // -----------------
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                string sortBy = dto.SortBy?.ToLower() ?? "id";
                bool isDescending = (dto.SortOrder?.ToLower() ?? "desc") == "desc";

                // -----------------
                // Base Query
                // -----------------
                var baseQuery = _context.EmployeeEducations
                    .AsNoTracking()
                    .Where(edu =>
                        edu.EmployeeId == dto.Prop.EmployeeId &&
                        (dto.IsActive == null || edu.IsActive == dto.IsActive) &&
                        (edu.IsSoftDeleted != true)
                    );

                // -----------------
                // Filters
                // -----------------

                //if (dto.Prop.RowId > 0)
                //    baseQuery = baseQuery.Where(x => x.Id == dto.Prop.RowId);

                if (!string.IsNullOrEmpty(dto.InstituteName))
                    baseQuery = baseQuery.Where(x => x.InstituteName.Contains(dto.InstituteName));

                if (!string.IsNullOrEmpty(dto.Degree))
                    baseQuery = baseQuery.Where(x => x.Degree.Contains(dto.Degree));

                if (dto.EducationGap.HasValue)
                    baseQuery = baseQuery.Where(x => x.EducationGap == dto.EducationGap);

                if (dto.IsInfoVerified.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsInfoVerified == dto.IsInfoVerified);

                if (dto.IsEditAllowed.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsEditAllowed == dto.IsEditAllowed);

                // -----------------
                // Sorting
                // -----------------
                baseQuery = sortBy switch
                {
                    "degree" => isDescending ? baseQuery.OrderByDescending(x => x.Degree) : baseQuery.OrderBy(x => x.Degree),
                    "institutename" => isDescending ? baseQuery.OrderByDescending(x => x.InstituteName) : baseQuery.OrderBy(x => x.InstituteName),
                    "haseducationdocuploded" => isDescending ? baseQuery.OrderByDescending(x => x.HasEducationDocUploded) : baseQuery.OrderBy(x => x.HasEducationDocUploded),
                    "startdate" => isDescending ? baseQuery.OrderByDescending(x => x.StartDate) : baseQuery.OrderBy(x => x.StartDate),
                    "enddate" => isDescending ? baseQuery.OrderByDescending(x => x.EndDate) : baseQuery.OrderBy(x => x.EndDate),
                    _ => isDescending ? baseQuery.OrderByDescending(x => x.Id) : baseQuery.OrderBy(x => x.Id)
                };

                // -----------------
                // Count & Fetch
                // -----------------
                var totalRecords = await baseQuery.CountAsync();

                var eduList = await baseQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // -----------------
                // Mapping + Per Record Completion Calculation
                // -----------------
                List<GetEducationResponseDTO> finalList = new();
                double totalPercentage = 0;

                foreach (var edu in eduList)
                {
                    var dtoItem = new GetEducationResponseDTO
                    {
                        Id = edu.Id.ToString(),
                        EmployeeId = edu.EmployeeId.ToString(),
                        Degree = edu.Degree,
                        InstituteName = edu.InstituteName,
                        Remark = edu.Remark,
                        ScoreValue = edu.ScoreValue,
                        ScoreType = edu.ScoreType?.ToString(),
                        GradeDivision = edu.GradeDivision,
                        EducationGap = edu.EducationGap,
                        ReasonOfEducationGap = edu.ReasonOfEducationGap,
                        StartDate = edu.StartDate,
                        EndDate = edu.EndDate,
                        FilecPath = edu.FilePath,
                        FileType = edu.FileType?.ToString(),
                        FileName = edu.FileName,
                        IsActive = edu.IsActive,
                        IsEditAllowed = edu.IsEditAllowed,
                        IsInfoVerified = edu.IsInfoVerified,
                        InfoVerifiedById = edu.InfoVerifiedById?.ToString(),
                        HasEducationDocUploded = edu.HasEducationDocUploded
                    };

                    // -----------------
                    // Per-record completion calculation
                    // -----------------
                    dtoItem.CompletionPercentage = PercentageHelper.CalculateEducationCompletion(dtoItem);

                    totalPercentage += dtoItem.CompletionPercentage;

                    finalList.Add(dtoItem);
                }

                // -----------------
                // Average Percentage
                // -----------------
                averagePercentage = finalList.Count > 0 ? Math.Round(totalPercentage / finalList.Count, 0) : 0;

                // Section-level completion
                var educationSection = eduList.CalculateEducationCompletion();

                // All documents uploaded?
                hasUploadedAll = finalList.All(x => x.HasEducationDocUploded == true);

                // -----------------
                // Final Paged Response
                // -----------------
                return new PagedResponseDTO<GetEducationResponseDTO>
                {
                    Items = finalList,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    CompletionPercentage = educationSection.CompletionPercent,
                    HasUploadedAll = hasUploadedAll
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching education info for EmployeeId: {EmployeeId}", dto.Prop.EmployeeId);
                throw new Exception($"Failed: {ex.Message}");
            }
        }


        public async Task<bool> UpdateEmployeeFieldAsync(EmployeeEducation entity)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                context.EmployeeEducations.Update(entity);

                int affectedRows = await context.SaveChangesAsync();

                return affectedRows > 0;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "⚠ Concurrency conflict while updating EmployeeEducation Id={Id}", entity.Id);
                return false;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "⚠ Database update failure for EmployeeEducation Id={Id}", entity.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error while updating EmployeeEducation Id={Id}", entity.Id);
                return false;
            }
        }


        public async Task<EmployeeEducation?> GetSingleRecordAsync(long Id, bool IsActive)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                return await context.EmployeeEducations
                    .Where(x => x.Id == Id && x.IsActive == IsActive && x.IsSoftDeleted!=true)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching education record. Id: {Id}, Error: {ex.Message}");
                throw;
            }
        }




       public async Task<bool> DeleteAsync(long id, long UserEmployeeId)
{
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // 🔹 Fetch record
        var employee = await _context.EmployeeEducations
            .FirstOrDefaultAsync(e => e.Id == id && e.IsSoftDeleted != true);

        if (employee == null)
        {
            _logger.LogWarning($"⚠️ Record with ID {id} not found or already deleted.");
            return false;
        }

        // 🔹 Update record
        employee.IsSoftDeleted = true;
        employee.SoftDeletedById = UserEmployeeId;
        employee.UpdatedDateTime = DateTime.UtcNow; // optional audit tracking

        _context.EmployeeEducations.Update(employee);

        // 🔹 Check affected rows
        int affected = await _context.SaveChangesAsync();

        if (affected > 0)
        {
            await transaction.CommitAsync();
            _logger.LogInformation($"✅ Record {id} successfully soft deleted.");
            return true;  // update hua → true
        }

        await transaction.RollbackAsync();
        _logger.LogWarning($"⚠️ No rows updated for ID: {id}");
        return false;
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        _logger.LogError(ex, $"❌ Delete failed for ID {id}");
        throw new Exception($"Failed to delete record {id}: {ex.Message}", ex);
    }
}



 



    }



}




 
 






