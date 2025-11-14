

using AutoMapper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{

    public class EmployeeIdentityRepository : IEmployeeIdentityRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeIdentityRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeIdentityRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeIdentityRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }



        public async Task<GetIdentityResponseDTO> CreateAsync(EmployeePersonalDetail entity)
        {
            try
            {
                // 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity), "Personal info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // 2️⃣ Record Insert
                await _context.EmployeePersonalDetails.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 3️⃣ Fetch the latest record (since 1:1 relation, only one record expected)
                var record = await _context.EmployeePersonalDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (record == null)
                    throw new Exception("Personal info record not found after insert.");

                // 4️⃣ Mapping to DTO
                var responseData = _mapper.Map<GetIdentityResponseDTO>(record);

                // 5️⃣ Return Response
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while adding/fetching personal info for EmployeeId: {EmployeeId}", entity.EmployeeId);
                throw new Exception($"Failed to add or fetch personal info: {ex.Message}");
            }
        }

        public async Task<PagedResponseDTO<GetIdentityResponseDTO>> GetInfo(GetIdentityRequestDTO dto, long employeeId, long id)
        {
            try
            {
                // 🧭 Base Query (Active & SoftDelete check)
                var baseQuery = _context.EmployeePersonalDetails
                    .AsNoTracking()
                    .Where(identity => identity.EmployeeId == employeeId
                                       && identity.IsActive == dto.IsActive
                                       && identity.IsSoftDeleted != true);

                // 🗺️ Optional Filters
                if (!string.IsNullOrWhiteSpace(dto.Id) && long.TryParse(dto.Id, out long parsedId) && parsedId > 0)
                    baseQuery = baseQuery.Where(x => x.Id == parsedId);

                if (!string.IsNullOrWhiteSpace(dto.BloodGroup))
                    baseQuery = baseQuery.Where(x => x.BloodGroup.ToLower().Contains(dto.BloodGroup.ToLower()));

                if (!string.IsNullOrWhiteSpace(dto.MaritalStatus))
                    baseQuery = baseQuery.Where(x => x.MaritalStatus.ToLower().Contains(dto.MaritalStatus.ToLower()));

                if (!string.IsNullOrWhiteSpace(dto.Nationality))
                    baseQuery = baseQuery.Where(x => x.Nationality.ToLower().Contains(dto.Nationality.ToLower()));

                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName))
                    baseQuery = baseQuery.Where(x => x.EmergencyContactName.ToLower().Contains(dto.EmergencyContactName.ToLower()));

                // 🧩 Aadhaar / PAN / Passport Uploaded Filters
                if (dto.HasAadhaarIdUploaded.HasValue)
                    baseQuery = baseQuery.Where(x => x.HasAadhaarIdUploaded == dto.HasAadhaarIdUploaded.Value);

                if (dto.HasPanIdUploaded.HasValue)
                    baseQuery = baseQuery.Where(x => x.HasPanIdUploaded == dto.HasPanIdUploaded.Value);

                if (dto.HasPassportIdUploaded.HasValue)
                    baseQuery = baseQuery.Where(x => x.HasPassportIdUploaded == dto.HasPassportIdUploaded.Value);

                // 🔍 Keyword Search
                if (!string.IsNullOrEmpty(dto.SortBy))
                {
                    var keyword = dto.SortBy.Trim().ToLower();
                    baseQuery = baseQuery.Where(x =>
                        (x.BloodGroup != null && x.BloodGroup.ToLower().Contains(keyword)) ||
                        (x.MaritalStatus != null && x.MaritalStatus.ToLower().Contains(keyword)) ||
                        (x.Nationality != null && x.Nationality.ToLower().Contains(keyword)) ||
                        (x.EmergencyContactName != null && x.EmergencyContactName.ToLower().Contains(keyword)) ||
                        (x.PanNumber != null && x.PanNumber.ToLower().Contains(keyword)) ||
                        (x.AadhaarNumber != null && x.AadhaarNumber.ToLower().Contains(keyword)) ||
                        (x.PassportNumber != null && x.PassportNumber.ToLower().Contains(keyword)));
                }

                // 🔽 Sorting
                bool isDescending = string.Equals(dto.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

                baseQuery = !string.IsNullOrEmpty(dto.SortBy)
                    ? dto.SortBy.ToLower() switch
                    {
                        "bloodgroup" => isDescending
                            ? baseQuery.OrderByDescending(x => x.BloodGroup)
                            : baseQuery.OrderBy(x => x.BloodGroup),

                        "maritalstatus" => isDescending
                            ? baseQuery.OrderByDescending(x => x.MaritalStatus)
                            : baseQuery.OrderBy(x => x.MaritalStatus),

                        "nationality" => isDescending
                            ? baseQuery.OrderByDescending(x => x.Nationality)
                            : baseQuery.OrderBy(x => x.Nationality),

                        "hasaadhariduploaded" => isDescending
                            ? baseQuery.OrderByDescending(x => x.HasAadhaarIdUploaded)
                            : baseQuery.OrderBy(x => x.HasAadhaarIdUploaded),

                        "haspaniduploaded" => isDescending
                            ? baseQuery.OrderByDescending(x => x.HasPanIdUploaded)
                            : baseQuery.OrderBy(x => x.HasPanIdUploaded),

                        "haspassportiduploaded" => isDescending
                            ? baseQuery.OrderByDescending(x => x.HasPassportIdUploaded)
                            : baseQuery.OrderBy(x => x.HasPassportIdUploaded),

                        _ => isDescending
                            ? baseQuery.OrderByDescending(x => x.Id)
                            : baseQuery.OrderBy(x => x.Id)
                    }
                    : baseQuery.OrderByDescending(x => x.Id);

                // 📄 Total Count
                var totalRecords = await baseQuery.CountAsync();

                // 🧩 Projection to DTO
                var query = from identity in baseQuery
                            select new GetIdentityResponseDTO
                            {
                                Id = identity.Id.ToString(),
                                EmployeeId = identity.EmployeeId.ToString(),
                                BloodGroup = identity.BloodGroup,
                                MaritalStatus = identity.MaritalStatus,
                                Nationality = identity.Nationality,
                                EmergencyContactName = identity.EmergencyContactName,
                                EmergencyContactNumber = identity.EmergencyContactNumber,
                                PanNumber = identity.PanNumber,
                                AadhaarNumber = identity.AadhaarNumber,
                                PassportNumber = identity.PassportNumber,
                                DrivingLicenseNumber = identity.DrivingLicenseNumber,
                                hasPassportIdUploaded = identity.HasPassportIdUploaded,
                                hasAadharIdUploaded = identity.HasAadhaarIdUploaded,
                                hasPanIdUploaded = identity.HasPanIdUploaded,
                                aadharDocPath = identity.AadhaarDocPath,
                                panDocPath = identity.PanDocPath,
                                passportDocPath = identity.PassportDocPath,
                            };

                // 📜 Pagination
                var pagedRecords = await query
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToListAsync();

                // 📦 Final Response
                return new PagedResponseDTO<GetIdentityResponseDTO>
                {
                    Items = pagedRecords ?? new List<GetIdentityResponseDTO>(),
                    TotalCount = totalRecords,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize,
                    CompletionPercentage = 75.5,
                    HasUploadedAll= false,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching identity info for EmployeeId: {EmployeeId}", employeeId);
                throw new Exception($"Failed to fetch identity info: {ex.Message}");
            }
        }

        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }

        Task<GetIdentityResponseDTO> IEmployeeIdentityRepository.GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }
    }
}




 
 






