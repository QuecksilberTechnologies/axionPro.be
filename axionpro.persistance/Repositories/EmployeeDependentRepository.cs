using AutoMapper;
using axionpro.application.Common.Helpers.PercentageHelper;
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

                // ✅ Insert
                await _context.EmployeeDependents.AddAsync(entity);
                await _context.SaveChangesAsync();

                // ✅ Base query
                var query = _context.EmployeeDependents
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == entity.EmployeeId &&
                        x.IsSoftDeleted != true &&
                        x.IsActive == true)
                    .OrderByDescending(x => x.Id);

                var totalRecords = await query.CountAsync();

                int pageSize = 10;
                var records = await query.Take(pageSize).ToListAsync();

                // ✅ Map
                var responseData = _mapper.Map<List<GetDependentResponseDTO>>(records);

                // ✅ Completion calculation (FAST & CLEAN)
                foreach (var item in responseData)
                {
                    item.CompletionPercentage =
                        CompletionCalculatorHelper.DependentPropCalculate(item);
                    
                }

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
                _logger.LogError(
                    ex,
                    "❌ Error occurred while adding/fetching Dependent info for EmployeeId: {EmployeeId}",
                    entity.EmployeeId);

                throw new Exception($"Failed to add or fetch Dependent info: {ex.Message}");
            }
        }

        public async Task<PagedResponseDTO<GetDependentResponseDTO>> GetInfo(
       GetDependentRequestDTO dto)
        {
            try
            {
              
                // 🔹 Fetch ONLY latest 10 dependents for employee
                var records = await _context.EmployeeDependents
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == dto.Prop.EmployeeId &&
                        x.IsSoftDeleted != true &&
                        x.IsActive == true)
                    .OrderByDescending(x => x.Id)   // ✅ Latest first
                    .Take(10)                       // ✅ Only 10 records
                    .ToListAsync();

                // 🔹 Map to DTO
                var responseData = _mapper.Map<List<GetDependentResponseDTO>>(records);

                // 🔹 Completion calculation (STANDARD helper)
                foreach (var item in responseData)
                {
                    item.CompletionPercentage =
                        CompletionCalculatorHelper.DependentPropCalculate(item);
                }

                double averageCompletion = responseData.Any()
                    ? Math.Round(responseData.Average(x => x.CompletionPercentage), 0)
                    : 0;

                return new PagedResponseDTO<GetDependentResponseDTO>
                {
                    Items = responseData,
                    TotalCount = responseData.Count, // max 10
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 1,
                    CompletionPercentage = averageCompletion,
                    HasUploadedAll = responseData.All(x => x.HasProofUploaded)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error fetching dependents for EmployeeId: {EmployeeId}",
                    dto.Prop.EmployeeId);

                return new PagedResponseDTO<GetDependentResponseDTO>
                {
                    Items = new List<GetDependentResponseDTO>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0,
                    CompletionPercentage = 0,
                    HasUploadedAll = false
                };
            }
        }


        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }


}




 
 






